using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Squishy.Suffix
{
	public class SuffixNode
	{
		public readonly SuffixTree Tree;
		public readonly HashSet<SuffixNode> Children = new HashSet<SuffixNode>();
		public readonly int ChildId;

		/// <summary>
		/// Leaf node ctor
		/// </summary>
		internal SuffixNode(SuffixTree tree, int from) :
			this(tree, from, -1)
		{
		}

		internal SuffixNode(SuffixTree tree, int from, int to)
		{
			Tree = tree;
			From = from;
			To = to;
			ChildId = Interlocked.Increment(ref Tree.lastNodeId);
		}

		#region Node Members
		public bool IsLeaf
		{
			get { return Children != null; }
		}

		/// <summary>
		/// Gets the child whose edge has c as the first character (or null if that child does not exist)
		/// </summary>
		public SuffixNode GetChild(char c)
		{
			foreach (var child in Children)
			{
				if (child.FirstEdgeChar == c)
				{
					return child;
				}
			}
			return null;
		}
		#endregion

		#region Edge Members
		/// <summary>
		/// The offset of the first occurance of the following substring within the original string
		/// </summary>
		public int From
		{
			get;
			internal set;
		}

		/// <summary>
		/// The end index of this Edge.
		/// Also represents this Node's "own index".
		/// </summary>
		public int To
		{
			get;
			internal set;
		}

		/// <summary>
		/// The actual index (replaces -1 with the last index in the string)
		/// </summary>
		public int ActualTo
		{
			get { return To == -1 ? Tree.StringLength-1 : To; }
		}

		/// <summary>
		/// First char on the edge between this node's parent and itself
		/// </summary>
		public char FirstEdgeChar
		{
			get { return Tree.String[From]; }
		}

		public int EdgeLength
		{
			get { return ActualTo - From + 1; }
		}

		/// <summary>
		/// Suffix between Parent and this node
		/// </summary>
		public string EdgeString
		{
			get
			{
				var str = Tree.String.Substring(From, EdgeLength);
				return str;
			}
		}

		public char GetEdgeChar(int offset)
		{
			if (offset >= EdgeLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("offset {0} is greater than this edge's length (From {1} To {2})", offset, From, To));
			}
			return Tree.String[From + offset];
		}

		/// <summary>
		/// Returns the partial suffix of the given length, starting from the beginning of this edge (between this node and it's parent)
		/// </summary>
		public string GetEdgeString(int length)
		{
			if (length > EdgeLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("length {0} is greater than this edge's length (From {1} To {2})", length, From, To));
			}
			return Tree.String.Substring(From, length);
		}

		/// <summary>
		/// Set the given location to the end location of the String[suffixOffset+1 .. suffixOffset+len] in the tree.
		/// (Move to-from units down, searching for nodes along the given substring)
		/// </summary>
		public void SetNextSuffixLocation(int from, int to, CharacterLocation location)
		{
			SuffixNode parent = this, curNode = this;			// current Node and it's parent
			var remaining = 0;
			for (var idx = from; idx < to; )
			{
				var c = Tree.String[idx];
				parent = curNode;
				curNode = curNode.GetChild(c);
				var edgeLen = curNode.EdgeLength;
				remaining = to - idx;
				if (remaining > edgeLen)
				{
					// keep going down
					idx += edgeLen;
				}
				else if (remaining == edgeLen)
				{
					// move onto the node
					idx += edgeLen;
					location.MoveTo(curNode);
					return;
				}
				else
				{
					// done
					break;
				}
			}

			location.MoveTo(parent, curNode, remaining);
		}
		#endregion

		public override string ToString()
		{
			if (From == -1)
			{
				return "<Root>";
			}
			return string.Format("Node #{0} ([{1} .. {2}] {3})", ChildId, From, To, EdgeString);
		}
	}
}
