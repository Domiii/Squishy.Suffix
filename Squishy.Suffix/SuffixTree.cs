using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	/// <summary>
	/// (Probably) optimal SuffixTree implementation:
	/// Preparation time and space: O(|SuffixTree.String|)
	/// Query time and space: O(|query|)
	/// </summary>
	public class SuffixTree
	{
		internal int lastNodeId = -1;

		public SuffixTree()
		{
			Root = new SuffixNode(this);
		}

		public SuffixNode Root
		{
			get;
			protected set;
		}

		/// <summary>
		/// The String that this Tree is composed of.
		/// TODO: Consider using StringBuilder instead
		/// </summary>
		public string String
		{
			get;
			internal set;
		}

		/// <summary>
		/// Necessary when building the tree.
		/// Is increased with every added character: 
		/// Nodes with To = "-1", have ActualTo set to this value.
		/// </summary>
		internal int StringLength
		{
			get;
			set;
		}

		/// <summary>
		/// The separator will ensure that every suffix ends in a leaf.
		/// TODO: Need to make sure that this is not contained in any string
		/// </summary>
		internal char Separator
		{
			get { return (char)0xFF; }
		}

		/// <summary>
		/// Returns the node/edge on which the given searchTerm terminates, 
		/// or null, if searchTerm is not inside this Tree.
		/// </summary>
		public SuffixNode GetNodeOrEdge(string query)
		{
			var curNode = Root;
			for (var i = 0; i < query.Length; )
			{
				var c = query[i];
				var found = false;

				// find the child that contains the given searchTerm
				foreach (var child in curNode.Children)
				{
					if (child.FirstEdgeChar == c)
					{
						// found child
						curNode = child;
						found = true;
						break;
					}
				}
				if (!found)
				{
					return null;
				}

				// move along the edge towards the child:
				var end = Math.Min(query.Length - i, curNode.EdgeLength);
				for (var j = 0; j < end; j++, i++)
				{
					if (String[curNode.From + j] != query[i])
					{
						return null;
					}
				}
			}
			return curNode;
		}

		/// <summary>
		/// Returns whether the given searchTerm is contained in this Tree's String
		/// </summary>
		public bool Contains(string query)
		{
			return GetNodeOrEdge(query) != null;
		}
	}
}