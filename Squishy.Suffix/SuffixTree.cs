using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	public class SuffixTree<T> : SuffixTree
	{
		protected int firstUnusedIndex;

		public SuffixTree()
			: this(10)
		{
		}

		public SuffixTree(int initCapacity)
			: this(initCapacity, 75)
		{
		}

		public SuffixTree(int initCapacity, int pctInflationFactor)
		{
			Pairs = new KeyValuePair<string, T>[initCapacity];
			InflactionFactorPct = pctInflationFactor;
			firstUnusedIndex = 0;
			Count = 0;
		}

		public int InflactionFactorPct
		{
			get;
			set;
		}

		public int Count
		{
			get;
			protected set;
		}

		public KeyValuePair<string, T>[] Pairs
		{
			get;
			private set;
		}

		public void Add(IEnumerable<KeyValuePair<string, T>> pairs)
		{
			var count = pairs.Count();
			var builder = new StringBuilder(count * 20);
			int i = 0;
			foreach (var pair in pairs)
			{
				builder.Append(pair.Key);
				if (i < count - 1)
				{
					builder.Append(Separator);
				}
			}

			foreach (var pair in pairs)
			{
				// add suffixes to tree
				Count++;
				// TODO: Fix me
				//Traverse(pair.Key);
			}
		}

		public void Add(KeyValuePair<string, T> pair)
		{
			Count++;
			if (firstUnusedIndex >= Pairs.Length)
			{
				// ensure Values array is big enough
				// Hint: Resizing is amortized linear for all values added in total, 
				//       as long as we use a factor, rather than a constant threshold for inflation
				var values = new KeyValuePair<string, T>[(Count * InflactionFactorPct + 50) / 100];	// rounding included
				Array.Copy(Pairs, values, Pairs.Length);
				Pairs = values;
			}

			// add pair
			Pairs[firstUnusedIndex] = pair;

			// find next unused index
			for (; Pairs.Length < firstUnusedIndex && !Equals(Pairs[firstUnusedIndex], default(T)); firstUnusedIndex++) ;

			// add to string
			var str = pair.Key;
			if (String.Length > 0)
			{
				// add separator
				str = Separator + str;
			}
			String += str;

			// add suffixes to tree
			// TODO: Fix me
			//Traverse(pair.Key);
		}
	}

	public class SuffixTree
	{
		internal int lastNodeId = -1;

		public SuffixTree()
		{
			Root = new SuffixNode(this, -1, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		public string String
		{
			get;
			internal set;
		}

		/// <summary>
		/// Necessary when building the tree.
		/// Is increased with every added character: 
		/// Nodes that go until "-1" go until this value.
		/// </summary>
		internal int StringLength
		{
			get;
			set;
		}

		public SuffixNode Root
		{
			get;
			protected set;
		}

		/// <summary>
		/// Need to make sure
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