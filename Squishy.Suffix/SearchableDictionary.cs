using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squishy.Suffix
{
	/// <summary>
	/// Searchable dictionary based on a SuffixTree
	/// 
	/// </summary>
	public class SearchableDictionary<T> : SuffixTree
	{
		protected int firstUnusedIndex;

		public SearchableDictionary()
			: this(10)
		{
		}

		public SearchableDictionary(int initCapacity)
			: this(initCapacity, 75)
		{
		}

		public SearchableDictionary(int initCapacity, int pctInflationFactor)
		{
			Pairs = new KeyValuePair<string, T>[initCapacity];
			InflactionFactorPct = pctInflationFactor;
			firstUnusedIndex = 0;
			Count = 0;
		}

		public SearchableDictionary(KeyValuePair<string, T>[] pairs) : this(pairs, pairs.Length * 20)
		{
		}

		public SearchableDictionary(KeyValuePair<string, T>[] pairs, int estimatedStrLength)
		{
			SetPairs(pairs, estimatedStrLength);
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

		#region Creation
		private void SetPairs(KeyValuePair<string, T>[] pairs, int estimatedStrLength)
		{
			var sb = new StringBuilder(estimatedStrLength);
			foreach (var pair in pairs)
			{
				sb.Append(pair.Key);
				sb.Append(Separator);
			}

			String = sb.ToString();

			var builder = new SuffixTreeBuilder(this);
			builder.BuildTree();

			// TODO: Maintain lists of values that belong to each node
		}
		#endregion

		#region Queries
		public T[] this[params string[] inclusiveQueries]
		{
			get
			{
				var node = GetLowestCommonAncestor(inclusiveQueries);
				throw new NotImplementedException();
			}
		}

		public T[] this[string[] inclusiveQueries, string[] optionalQueries]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SuffixNode GetLowestCommonAncestor(params string[] queries)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Add (NIY)
		public void Add(IEnumerable<KeyValuePair<string, T>> pairs)
		{
			throw new NotImplementedException("Dictionary is currently immutable");

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
			throw new NotImplementedException("Dictionary is currently immutable");

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
		#endregion
	}

}
