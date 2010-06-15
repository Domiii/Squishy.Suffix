using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domi.Suffix.Tests
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
		public void TestSimple()
		{
			var tree = SuffixTreeBuilder.Create("aa");
			Assert.IsTrue(tree.Contains(""));
			Assert.IsTrue(tree.Contains("a"));
			Assert.IsTrue(tree.Contains("aa"));
			//Assert.IsTrue(tree.Contains("aaa"));
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
	}
}
