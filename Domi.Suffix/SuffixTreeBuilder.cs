using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domi.Suffix
{
	/// <summary>
	/// TODO: Fix SuffixNode.To (node only goes until k / also consider multiple string combos)
	/// 
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
			builder.BuildTree(s);
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
			Parent = Next = tree.Root;
		}

		public SuffixTree Tree
		{
			get; private set;
		}

		public int DistanceToRoot
		{
			get
			{
				return (Next == Tree.Root ? 0 : Next.From) + Distance;
			}
		}

		public char CurrentChar
		{
			get
			{
				if (Distance == 0)
				{
					throw new SuffixBuildException("CurrentChar does not exist if we have a distance of 0");
				}
				return Next.GetEdgeChar(Distance-1);
			}
		}

		public char NextChar
		{
			get
			{
				if (Distance == 0)
				{
					throw new SuffixBuildException("NextChar does not exist if we have a distance of 0");
				}
				return Next.GetEdgeChar(Distance);
			}
		}

		/// <summary>
		/// Add all nodes for the given string to the Tree
		/// </summary>
		/// <param name="str"></param>
		protected void BuildTree(string str)
		{
#if DEBUG
			Console.WriteLine("Adding String: {0}", str);
#endif
			if (str.Length == 0)
			{
				return;
			}

			// add first node to tree
			Parent  = Next = Tree.Root;
			Distance = 0;

			Tree.Root.Children.Add(new SuffixNode(Tree, 0));
			Tree.StringLength = 1;

			var i = 1;

			// iterate over all prefixes of str
			for (var k = 1; k < str.Length; k++)
			{
				Tree.StringLength++;

				// iterate over all suffixes for each prefix
				var c = str[k];
				var lastJumpLength = -1;
				SuffixNode newLinkStart = null;
#if DEBUG
				Console.WriteLine("k = {0}: {1}", k, str.Substring(0, k+1));
#endif
				for (; i <= k; i++)
				{
#if DEBUG
					Console.WriteLine(" i = {0}", i);
#endif
					
					var lastNode = Next;

					if (TraverseToChild(c))
					{
#if DEBUG
						Console.WriteLine("  Traversed to {0}", c);
#endif
						if (newLinkStart != null)
						{
							// create the link towards the new branch node, from the branch that we created in the last iteration
							AddLink(newLinkStart, lastNode, lastJumpLength);
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
						var branch = Next;
#if DEBUG
						Console.WriteLine("   Added leaf {0} {1}: {2}", c, branchExisted ? "to existing branch" : "to new branch", branch);
#endif

						if (newLinkStart != null)
						{
							// create the link towards the new branch node, from the branch that we created in the last iteration
							AddLink(newLinkStart, branch, lastJumpLength);
						}

						// the length of the next suffix
						var nextSuffixLen = k - i - 1;

						if (branchExisted)
						{
							// we added child to existing branch:

							//  -> don't add a new link
							newLinkStart = null;

							//  -> Travel to linked node and subtract saved length
							nextSuffixLen -= MoveToLink(branch);
						}
						else
						{
							// remember the branch node, so we can add the link in the next iteration
							newLinkStart = branch;
							lastJumpLength = nextSuffixLen;

							// find new link, starting from root
							MoveTo(Tree.Root);
#if DEBUG
							Console.WriteLine("   Moved to Root");
#endif
						}
						
						if (nextSuffixLen > 0)
						{
							// move to the location of the next suffix: S[i+1 .. k]
							Parent.SetNextSuffixLocation(k - nextSuffixLen, k, this);
#if DEBUG
							Console.WriteLine("   Moved to next suffix node {0} -> {1} (+{2})", Parent, Next, Distance);
#endif
						}
						else
						{
							// empty substring -> Nothing to traverse
							if (Parent != Tree.Root)
							{
								throw new SuffixBuildException("Unable to build SuffixTree: Empty suffix not starting at root: {0} to {1}",k , k);
							}
						}
					}
				}
			}

			if (Parent != Tree.Root)
			{
				// something went wrong (str must end with unique Separator)
				throw new Exception("Error while building tree for: " + str);
			}
		}

		/// <summary>
		/// Move to the link that the given branch links to
		/// </summary>
		/// <returns>The length of the substring for that link</returns>
		private int MoveToLink(SuffixNode branch)
		{
			SuffixLink link;
			if (Links.TryGetValue(branch, out link))
			{
				// Move to link and return saved length
				MoveTo(link.Node);
#if DEBUG
				Console.WriteLine("   Moved along link to: {0}", Next);
#endif
				return link.Length;
			}
			else if (Next != Tree.Root)
			{
				throw new SuffixBuildException("Unable to build SuffixTree: Internal node did not have a link: " + Next);
			}
			return 0;
		}

		/// <summary>
		/// Adds a new link
		/// </summary>
		private void AddLink(SuffixNode startNode, SuffixNode linkedNode, int length)
		{
#if DEBUG
			Console.WriteLine("   Added link from {0} to {1}", startNode, linkedNode);
#endif
			var newLink = new SuffixLink
			{
				Node = linkedNode,
				Length = length
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
				foreach (var child in Parent.Children)
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
						Next = node;
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
					if (Distance == Next.EdgeLength-1)
					{
						// move onto a node
						MoveTo(Next);
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
			var parent = Parent;
			if (Distance != 0)
			{
				// Adding branch between two nodes
				var next = Next;

				// split edge & add branch node
				// (next also describes the edge between parent and next)
				var branchNode = new SuffixNode(Tree, next.From, next.From + Distance-1);
				parent.Children.Remove(next);		// Hashset Add/Remove operations are constant
				parent.Children.Add(branchNode);
				branchNode.Children.Add(next);
				next.From += Distance;

				// branchNode becomes parent of the new leaf
				parent = branchNode;
			}
			// else Add branch to existing node

			// add new leaf
			var leaf = new SuffixNode(Tree, k);
			parent.Children.Add(leaf);

			Next = parent;
			return Distance == 0;
		}
	}
}
