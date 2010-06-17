using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Squishy.Suffix.Tests
{
	[TestClass]
	public class SuffixTreeTest
	{
		public TestContext TestContext
		{
			get;
			set;
		}

		[TestMethod]
		public void TestSimpleTree()
		{
			var tree = SuffixTreeBuilder.Create("aa");
			Assert.IsTrue(tree.Contains(""));
			Assert.IsTrue(tree.Contains("a"));
			Assert.IsTrue(tree.Contains("aa"));
			
			Assert.IsFalse(tree.Contains("aaa"));
		}

		/// <summary>
		/// Test the tree that was presented in the slides
		/// </summary>
		[TestMethod]
		public void TestSlideTree()
		{
			var tree = SuffixTreeBuilder.Create("bbabbaab");

			Assert.IsTrue(tree.Contains(""));
			Assert.IsTrue(tree.Contains("a"));
			Assert.IsTrue(tree.Contains("b"));
			Assert.IsTrue(tree.Contains("aa"));
			Assert.IsTrue(tree.Contains("bb"));
			Assert.IsTrue(tree.Contains("aab"));
			Assert.IsTrue(tree.Contains("bab"));
			Assert.IsTrue(tree.Contains("abbaa"));
			Assert.IsTrue(tree.Contains("bbab"));
			Assert.IsTrue(tree.Contains("babbaa"));
			Assert.IsTrue(tree.Contains("abbaab"));

			Assert.IsFalse(tree.Contains("zzzzzzzzzzzzzzz"));
			Assert.IsFalse(tree.Contains("aba"));
			Assert.IsFalse(tree.Contains("abbb"));
			Assert.IsFalse(tree.Contains("babbaaa"));
			Assert.IsFalse(tree.Contains("babbab"));
			//Assert.IsTrue(tree.Contains("aaa"));
		}

		/// <summary>
		/// Test the tree that was presented in the slides for the 
		/// circular string linearization algorithm
		/// </summary>
		[TestMethod]
		public void TestDoubledSlideTree()
		{
			var tree = SuffixTreeBuilder.Create("bbabbaabbbabbaab");

			Assert.IsTrue(tree.Contains(""));
			Assert.IsTrue(tree.Contains("a"));
			Assert.IsTrue(tree.Contains("b"));
			Assert.IsTrue(tree.Contains("aa"));
			Assert.IsTrue(tree.Contains("bb"));
			Assert.IsTrue(tree.Contains("aab"));
			Assert.IsTrue(tree.Contains("bab"));
			Assert.IsTrue(tree.Contains("abbaa"));
			Assert.IsTrue(tree.Contains("bbab"));
			Assert.IsTrue(tree.Contains("babbaa"));
			Assert.IsTrue(tree.Contains("abbaab"));
			Assert.IsTrue(tree.Contains("bbabbaabbbabbaab"));

			Assert.IsFalse(tree.Contains("zzzzzzzzzzzzzzz"));
			Assert.IsFalse(tree.Contains("aba"));
			Assert.IsFalse(tree.Contains("babbaaa"));
			Assert.IsFalse(tree.Contains("babbab"));
			//Assert.IsTrue(tree.Contains("aaa"));
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestPerformances()
		{
#if DEBUG
			throw new InvalidOperationException("Don't test performance in Debug mode - switch to Release instead!");
#endif

			// IMPORTANT: As soon as we get to 32768 or more nodes, performance drops rapidly
			//	because we iterate over a lot more characters in each iteration (up to 26 different ones for each child!)
			// However the bottleneck situation seems to saturate for our small queries, once we get to above a million
			// (also need to look into LOH performance: http://msdn.microsoft.com/en-us/magazine/cc534993.aspx)


			Console.WriteLine("Starting performance tests - Keep in mind that the first one definitely performs worse than the following ones!");
			TestPerformance(100, 10);
			Console.WriteLine();

			TestPerformance(1024, 100);
			Console.WriteLine();

			TestPerformance(1024 * 4, 500);
			Console.WriteLine();

			TestPerformance(1024 * 16, 500);
			Console.WriteLine();

			TestPerformance(1024 * 32, 500);
			Console.WriteLine();

			TestPerformance(1024 * 64, 500);
			Console.WriteLine();

			TestPerformance(1 << 18, 1024);
			Console.WriteLine();

			TestPerformance(1 << 20, 1024);
			Console.WriteLine();

			TestPerformance(1 << 22, 1024);
			Console.WriteLine();
		}

		public string GetRandomString(int size)
		{
			var sb = new StringBuilder(size);
			var rand = new Random();
			for (var i = 0; i < size; i++)
			{
				var c = (char)rand.Next('a', 'z' + 1);
				sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Important: Don't run performance tests in Debug mode - always choose Release!
		/// </summary>
		/// <param name="treeSize"></param>
		/// <param name="querySize"></param>
		public void TestPerformance(int treeSize, int querySize)
		{
			Console.WriteLine("Building tree of size {0} and perform query of size {1}", treeSize, querySize);

			var timer = new Stopwatch();
			timer.Start();

			// create the strings
			var rand = new Random();
			var stringStartTime = timer.Elapsed;
			var treeStr = GetRandomString(treeSize);
			var queryOffset = rand.Next(0, treeSize-querySize);
			var query = treeStr.Substring(queryOffset, querySize);

			// create the tree
			var treeStartTime = timer.Elapsed;
			var tree = SuffixTreeBuilder.Create(treeStr);

			var queryStartTime = timer.Elapsed;
			var node = tree.GetNodeOrEdge(query);
			var endTime = timer.Elapsed;

			// (see http://www.csharp-examples.net/string-format-double/ for string formatting in C#)
			Console.WriteLine("String building took: {0:0.00} ms", (treeStartTime - stringStartTime).TotalMilliseconds);

			Console.WriteLine("Tree building took: {0:0.00} ms", (queryStartTime - treeStartTime).TotalMilliseconds);

			Console.WriteLine("Query took: {0:0.00} ms - Node found: {1}", (endTime - queryStartTime).TotalMilliseconds, node.NodeId);
		}
	}
}
