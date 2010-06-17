using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	/// <summary>
	/// Represents a link between one node and another node.
	/// The node that this link links to is the node of the suffix of the first node, without the first character.
	/// </summary>
	public class SuffixLink
	{
		/// <summary>
		/// The node that this link links to
		/// </summary>
		public SuffixNode Node;
	}
}
