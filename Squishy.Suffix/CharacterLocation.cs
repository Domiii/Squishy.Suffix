using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	/// <summary>
	/// A character in the tree can either be on a node or on an edge.
	/// The exact position Next.From + Distance
	/// </summary>
	public class CharacterLocation
	{
		/// <summary>
		/// If we are <see cref="CurrentNode"/>, this is equal to <see cref="CurrentNode"/>.
		/// If we are on the edge between <see cref="CurrentNode"/> and it's parent, this is equal to CurrentNode's parent.
		/// </summary>
		public SuffixNode LastNode
		{
			get
			{
				return Distance == 0 ? CurrentNode : CurrentNode.Parent;
			}
		}

		/// <summary>
		/// The node/edge that we are currently on
		/// </summary>
		public SuffixNode CurrentNode
		{
			get;
			internal set;
		}

		/// <summary>
		/// The character at location <see cref="SuffixNode.From">Next.From</see> + <see cref="Distance"/>
		/// </summary>
		public char Character
		{
			get { return CurrentNode.GetEdgeChar(Distance); }
		}

		/// <summary>
		/// Distance from the 
		/// </summary>
		public int Distance
		{
			get; 
			internal set;
		}

		public bool OnNode
		{
			get { return Distance == 0; }
		}

		public void MoveTo(SuffixNode node)
		{
			CurrentNode = node;
			Distance = 0;
		}

		public void MoveTo(CharacterLocation location)
		{
			CurrentNode = location.CurrentNode;
			Distance = location.Distance;
		}

		public void MoveTo(SuffixNode node, int distanceFromNode)
		{
			CurrentNode = node;
			Distance = distanceFromNode;
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", 
				OnNode ? "" : CurrentNode.Parent + " -> ", 
				CurrentNode,
				OnNode ? "" : string.Format(" (+{0})", Distance));
		}
	}
}
