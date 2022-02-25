using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jeeves;
using System.Collections.Generic;
using System.Linq;

namespace JeevesTest
{
	[TestClass]
	public class DominatingPriorityQueueTest
	{
		[TestMethod]
		public void DominatingPriorityQueueInitTest1()
		{
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>();
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueueInitTest2()
        {
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>(1);
			Assert.IsFalse(PQ.IsEmpty());
        }
		[TestMethod]
		public void DominatingPriorityQueuePullTest1()
		{
			int init = 1;
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>(init);
			int head = PQ.PullHead();
			Assert.AreEqual(init, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueuePullTest2()
        {
			int[] init = { 1, 2 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>();
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(1, head);
			head = PQ.PullHead();
			Assert.AreEqual(2, head);
			Assert.IsTrue(PQ.IsEmpty());
        }
		[TestMethod]
		public void DominatingPriorityQueuePullTest3()
		{
			int[] init = { 2, 1 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>();
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(1, head);
			head = PQ.PullHead();
			Assert.AreEqual(2, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueueCompareTest1()
        {
			int[] init = { 1, 2 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>((x, y) => y - x);
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(2, head);
			head = PQ.PullHead();
			Assert.AreEqual(1, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueueDominatorTest1()
        {
			int[] init = { 1, 2 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>(
				(x, y) => x - y,
				(x, y) => x == y);
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(1, head);
			head = PQ.PullHead();
			Assert.AreEqual(2, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueueDominatorTest2()
		{
			int[] init = { 1, 1 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>(
				(x, y) => x - y,
				(x, y) => x == y);
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(1, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
		[TestMethod]
		public void DominatingPriorityQueueDominatorTest3()
		{
			int[] init = { 1, 3, 3 };
			DominatingPriorityQueue<int> PQ = new DominatingPriorityQueue<int>(
				(x, y) => x - y,
				(x, y) => x == y);
			init.ToList().ForEach(x => PQ.Insert(x));
			int head = PQ.PullHead();
			Assert.AreEqual(1, head);
			head = PQ.PullHead();
			Assert.AreEqual(3, head);
			Assert.IsTrue(PQ.IsEmpty());
		}
	}
}

