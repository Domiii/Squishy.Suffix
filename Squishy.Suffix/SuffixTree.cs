using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	public class SuffixTree
	{
		internal int lastNodeId = -1;

		public SuffixTree()
		{
			Root = new SuffixNode(this, -1, 0);
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
		/// Returns whether the given searchTerm is contained in this Tree's String
		/// </summary>
		public bool Contains(string searchTerm)
		{
			var curNode = Root;
			for (var i = 0; i < searchTerm.Length; )
			{
				var c = searchTerm[i];
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
					return false;
				}

				// move along the edge towards the child:
				var end = Math.Min(searchTerm.Length - i, curNode.EdgeLength);
				for (var j = 0; j < end; j++, i++)
				{
					if (String[curNode.From + j] != searchTerm[i])
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}