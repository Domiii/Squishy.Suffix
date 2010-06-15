using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domi.Suffix
{
	/// <summary>
	/// Represents a link between one node and another node that represents the 
	/// suffix of the first node, minus the first character.
	/// </summary>
	public class SuffixLink
	{
		/// <summary>
		/// The node that this link links to
		/// </summary>
		public SuffixNode Node;

		/// <summary>
		/// The amount of characters that we travelled down before we created the link 
		/// (ie the length of the substring between the 2 linked nodes = k - (i+1))
		/// </summary>
		public int Length;
	}
}
