using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domi.Suffix
{
	public class CharacterLocation
	{
		public SuffixNode Parent
		{
			get;
			internal set;
		}

		public SuffixNode Next
		{
			get;
			internal set;
		}

		private int m_Distance;

		public int Distance
		{
			get { return m_Distance; }
			internal set { m_Distance = value; }
		}

		public void MoveTo(SuffixNode node)
		{
			Parent = Next = node;
			Distance = 0;
		}

		public void MoveTo(CharacterLocation location)
		{
			Parent = location.Parent;
			Next = location.Next;
			Distance = location.Distance;
		}

		public void MoveTo(SuffixNode node, SuffixNode nextNode, int distanceFromNode)
		{
			Parent = node;
			Next = nextNode;
			Distance = distanceFromNode;
		}
	}
}
