using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	/// <summary>
	/// Thee Suffix Tree Builder also represents a Growing Point:
	/// The Growing Point lies between Parent and Next, in Distance from Parent.
	/// 
	/// Obviously, in order to parallelize operations on this data-strucutre, one would have to employ
	/// multiple Growing Points.
	/// </summary>
	public class SuffixTreeBuilder : CharacterLocation
	{
		public static SuffixTree Create(string s)
		{
			var tree = new SuffixTree();
			if (!s.EndsWith(tree.Separator + ""))
			{
				s += tree.Separator;
			}
			tree.String = s;
			var builder = new SuffixTreeBuilder(tree);
			builder.BuildTree();
			return tree;
		}

		/// <summary>
		/// Set of SuffixLinks from all internal Nodes to the next node which represents
		/// the suffix of the first node without the head character
		/// </summary>
		public readonly Dictionary<SuffixNode, SuffixLink> Links = new Dictionary<SuffixNode, SuffixLink>();

		public SuffixTreeBuilder(SuffixTree tree)
		{
			Tree = tree;
			CurrentNode = tree.Root;
		}

		public SuffixTree Tree
		{
			get; private set;
		}

		public int DistanceToRoot
		{
			get
			{
				return (CurrentNode == Tree.Root ? 0 : CurrentNode.From) + Distance;
			}
		}

		public char NextChar
		{
			get
			{
				if (Distance == 0)
				{
					throw new SuffixTreeBuilderException("NextChar is ambiguous, if we have a distance of 0");
				}
				return CurrentNode.GetEdgeChar(Distance);
			}
		}

		/// <summary>
		/// Add all nodes of Tree.String
		/// </summary>
		protected internal void BuildTree()
		{
			var str = Tree.String;
#if DEBUG
			Console.WriteLine("Adding String: {0}", str);
#endif
			if (str.Length == 0)
			{
				return;
			}

			CurrentNode = Tree.Root;
			Distance = 0;

			// add first node to tree
			new SuffixNode(Tree.Root, 0);
			Tree.StringLength = 1;

			var i = 1;

			// iterate over all prefixes of str
			for (var k = 1; k < str.Length; k++)
			{
				Tree.StringLength++;

				// iterate over all suffixes for each prefix
				var c = str[k];
				SuffixNode newLinkStart = null;
#if DEBUG
				Console.WriteLine("k = {0}: {1}", k, str.Substring(0, k+1));
#endif
				for (; i <= k; i++)
				{
#if DEBUG
					Console.WriteLine(" i = {0}", i);
#endif
					
					// remember the node that we are on, before going down the tree
					var lastNode = CurrentNode;

					if (TraverseToChild(c))
					{
#if DEBUG
						Console.WriteLine("  Traversed to {0}", c);
#endif
						if (newLinkStart != null)
						{
							// create the link towards the new branch node, from the branch that we created in the last iteration
							AddLink(newLinkStart, lastNode);
							newLinkStart = null;
						}

						// One step along the path, then move to the next prefix
						break;
					}
					else
					{
						// could not find child -> Need to add branch
						// (Q: Why do we not need to add branches in other place of the tree unless the single Growing Point needs to add one?)
						var branchExisted = AddBranch(k);

						// the branch node that is parent to the leaf of character c
						var branch = CurrentNode;

						if (newLinkStart != null)
						{
							// create the link towards the new branch node, from the branch that we created in the last iteration
							AddLink(newLinkStart, branch);
						}

						// the length of the next suffix
						int nextSuffixLen;

						if (branchExisted)
						{
							// we added child to existing branch:

							//  -> don't add a new link
							newLinkStart = null;

							// don't need to step down
							nextSuffixLen = 0;

							//  -> Travel to linked node
							MoveToLink(branch);
						}
						else
						{
							// remember the branch node, so we can add the link in the next iteration
							newLinkStart = branch;

							// step down from the linked node the exact edge, that we now move up
							nextSuffixLen = branch.EdgeLength;

							if (branch.Parent == Tree.Root)
							{
								// Move to root (we should be (almost) done now)
								MoveTo(Tree.Root);

								// If we went back to the root, we need to exclude the first character specifically
								nextSuffixLen--;
							}
							else
							{
								// jump to link of parent of branch
								MoveToLink(branch.Parent);
							}
						}
						
						if (nextSuffixLen > 0)
						{
							// move to the location of the next suffix: S[i+1 .. k]
							LastNode.SetNextSuffixLocation(k - nextSuffixLen, k, this);
#if DEBUG
							Console.WriteLine("   Moved to " + this);
#endif
						}
					}
				}
			}

			if (LastNode != Tree.Root)
			{
				// something went wrong (last step must be adding the unique Separator leaf to the root)
				throw new SuffixTreeBuilderException("Error while building tree for: " + str);
			}
		}

		/// <summary>
		/// Move to the link that the given branch links to
		/// </summary>
		/// <returns>The length of the substring for that link</returns>
		private void MoveToLink(SuffixNode branch)
		{
			SuffixLink link;
			if (Links.TryGetValue(branch, out link))
			{
				// Move to link and return saved length
				MoveTo(link.Node);
#if DEBUG
				Console.WriteLine("   Moved along link to: {0}", CurrentNode);
#endif
			}
			else if (CurrentNode != Tree.Root)
			{
				throw new SuffixTreeBuilderException("Unable to build SuffixTree: Internal node did not have a link: " + CurrentNode);
			}
		}

		/// <summary>
		/// Adds a new link
		/// </summary>
		private void AddLink(SuffixNode startNode, SuffixNode linkedNode)
		{
#if DEBUG
			Console.WriteLine("   Added link from {0} to {1}", startNode, linkedNode);
#endif
			// link is only a simple pointer
			var newLink = new SuffixLink
			{
				Node = linkedNode
			};
			Links.Add(startNode, newLink);
		}

		/// <summary>
		/// Traverse to the next location, that represents the given child (or return false, if child does not exist)
		/// </summary>
		/// <param name="c"></param>
		/// <returns>Whether the given child is neighboring the current location</returns>
		protected bool TraverseToChild(char c)
		{
			if (Distance == 0)
			{
				// GP sits on a node
				SuffixNode node = null;
				foreach (var child in LastNode.Children)
				{
					if (child.FirstEdgeChar == c)
					{
						// has child
						node = child;
						break;
					}
				}
				if (node != null)
				{
					// found child
					if (node.EdgeLength == 1)
					{
						// move onto the node
						MoveTo(node);
					}
					else
					{
						// move in direction of the node
						CurrentNode = node;
						Distance = 1;
					}
					return true;
				}
				// char does not exist: Will need to branch out
				return false;
			}
			else
			{
				// GP sits between two nodes
				if (NextChar == c)
				{
					// Char exists
					if (Distance == CurrentNode.EdgeLength-1)
					{
						// move onto a node
						MoveTo(CurrentNode);
					}
					else
					{
						// move along an edge
						Distance++;
					}
					return true;
				}
				// char does not exist: Will need to branch out
				return false;
			}
		}

		/// <summary>
		/// Adds a new leaf at the current position (also a new branch, if we are not currently on a node), 
		/// and sets Next to the branch node (the leaf's parent).
		/// </summary>
		/// <returns>Whether the branch node already existed</returns>
		protected bool AddBranch(int k)
		{
			var parent = LastNode;
			if (Distance != 0)
			{
				// Adding branch between two nodes
				var next = CurrentNode;

				// split edge & add branch node
				// (next also describes the edge between parent and next)
				var branchNode = new SuffixNode(parent, next.From, next.From + Distance - 1);
				parent.Children.Remove(next);		// HashSet Add/Remove operations are constant
				parent.Children.Add(branchNode);
				branchNode.Children.Add(next);
				next.Parent = branchNode;
				next.From += Distance;

				// branchNode becomes parent of the new leaf
				parent = branchNode;
			}
			// else Add branch to existing node

			// add new leaf
			var leaf = new SuffixNode(parent, k);
			parent.Children.Add(leaf);

#if DEBUG
			Console.WriteLine("   Added leaf {0} {1}: {2}", leaf, Distance == 0 ? "to existing branch" : "to new branch", parent);
#endif

			CurrentNode = parent;
			return Distance == 0;
		}
	}
}
