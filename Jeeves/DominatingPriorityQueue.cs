using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Jeeves
{
    public class DominatingPriorityQueue<T>
    {
        public DominatingPriorityQueue()
        {
            root = new Leaf<T>();
        }
        public DominatingPriorityQueue(Comparator<T> comparer) : this()
        {
            compare = comparer;
        }
        public DominatingPriorityQueue(Comparator<T> comparer, Dominator<T> dominator) : this(comparer)
        {
            dominates = dominator;
        }
        public DominatingPriorityQueue(T init) : this()
        {
            Insert(init);
        }
        public DominatingPriorityQueue(T init, Comparator<T> comparer) : this(init)
        {
            compare = comparer;
        }
        public DominatingPriorityQueue(T init, Comparator<T> comparer, Dominator<T> dominator) : this(init, comparer)
        {
            dominates = dominator;
        }
        public bool IsEmpty()
        {
            return IsEmpty(root);
        }
        public T PullHead()
        {
            T ret = root.Head;
            root = RemoveHead(root);
            return ret;
        }
        public void Insert(T state)
        {
            root = Insert(state, root);
        }
        public static DominatingPriorityQueue<T> operator +(DominatingPriorityQueue<T> PQ, IEnumerable<T> elems)
        {
            foreach (T elem in elems)
                PQ.Insert(elem);
            return PQ;
        }

        private bool IsEmpty(MinHeap<T> minHeap) =>
            minHeap is Leaf<T>;
        private MinHeap<T> Heap(T x, MinHeap<T> l, MinHeap<T> r) =>
            new Branch<T>(x, l, r, l.Size + r.Size + 1, Math.Max(l.Height, r.Height) + 1);
        private MinHeap<T> Heap(T x) =>
            Heap(x, new Leaf<T>(), new Leaf<T>());
        private MinHeap<T> BubbleUp(T x, MinHeap<T> l, MinHeap<T> r)
        {
            if ((l, r) is (Branch<T>(T y, MinHeap<T> yl, MinHeap<T> yr, _, _), _))
            {
                if (dominates(x, y)) return Heap(x, MergeChildren(yl, yr), r);
                if (dominates(y, x)) return Heap(y, MergeChildren(yl, yr), r);
                if (compare(x, y) > 0) return Heap(y, Heap(x, yl, yr), r);
            }
            if ((l, r) is (_, Branch<T>(T z, MinHeap<T> zl, MinHeap<T> zr, _, _)))
            {
                if (dominates(x, z)) return Heap(x, l, MergeChildren(zl, zr));
                if (dominates(z, x)) return Heap(z, l, MergeChildren(zl, zr));
                if (compare(x, z) > 0) return Heap(z, l, Heap(x, zl, zr));
            }
            return Heap(x, l, r);
        }
        private MinHeap<T> Insert(T x, MinHeap<T> heap)
        {
            if (IsEmpty(heap))
                return Heap(x);
            else if (heap.Left.Size < Math.Pow(2, heap.Left.Height) - 1)
                return BubbleUp(heap.Head, Insert(x, heap.Left), heap.Right);
            else if (heap.Right.Size < Math.Pow(2, heap.Right.Height) - 1)
                return BubbleUp(heap.Head, heap.Left, Insert(x, heap.Right));
            else if (heap.Right.Height < heap.Left.Height)
                return BubbleUp(heap.Head, heap.Left, Insert(x, heap.Right));
            else
                return BubbleUp(heap.Head, Insert(x, heap.Left), heap.Right);
        }
        private MinHeap<T> BubbleDown(T x, MinHeap<T> l, MinHeap<T> r)
        {
            if ((l, r) is ((Branch<T>(T y, _, _, _, _), Branch<T>(T z, MinHeap<T> rl, MinHeap<T> rr, _, _))))
            {
                if (dominates(x, z)) return Heap(x, l, MergeChildren(rl, rr));
                if (dominates(z, x)) return Heap(z, l, MergeChildren(rl, rr));
                if (compare(z, y) < 0 && compare(x, z) > 0) return Heap(z, l, BubbleDown(x, rl, rr));
            }
            if ((l, r) is (Branch<T>(T y1, MinHeap<T> ll, MinHeap<T> lr, _, _), _))
            {
                if (dominates(x, y1)) return Heap(x, MergeChildren(ll, lr), r);
                if (dominates(y1, x)) return Heap(y1, MergeChildren(ll, lr), r);
                if (compare(x, y1) > 0) return Heap(y1, BubbleDown(x, ll, lr), r);
            }
            return Heap(x, l, r);
        }
        private MinHeap<T> FloatLeft(T x, MinHeap<T> l, MinHeap<T> r) =>
            l switch
            {
                Branch<T>(T y, MinHeap<T> ll, MinHeap<T> lr, _, _) => Heap(y, Heap(x, ll, lr), r),
                _ => Heap(x, l, r)
            };
        private MinHeap<T> FloatRight(T x, MinHeap<T> l, MinHeap<T> r) =>
            r switch
            {
                Branch<T>(T y, MinHeap<T> rl, MinHeap<T> rr, _, _) => Heap(y, l, Heap(x, rl, rr)),
                _ => Heap(x, l, r)
            };
        private MinHeap<T> MergeChildren(MinHeap<T> l, MinHeap<T> r)
        {
            if (IsEmpty(l) && IsEmpty(r))
                return new Leaf<T>();
            else if (l.Size < Math.Pow(2, l.Height) - 1)
                return FloatLeft(l.Head, MergeChildren(l.Left, l.Right), r);
            else if (r.Size < Math.Pow(2, r.Height) - 1)
                return FloatRight(r.Head, l, MergeChildren(r.Left, r.Right));
            else if (r.Height < l.Height)
                return FloatLeft(l.Head, MergeChildren(l.Left, l.Right), r);
            else
                return FloatRight(r.Head, l, MergeChildren(r.Left, r.Right));
        }
        private MinHeap<T> BubbleRootDown(MinHeap<T> h) =>
            IsEmpty(h) ? new Leaf<T>() : BubbleDown(h.Head, h.Left, h.Right);
        private MinHeap<T> RemoveHead(MinHeap<T> h) =>
            IsEmpty(h) ? throw new Exception("Cannot remove element from empty heap") :
                BubbleRootDown(MergeChildren(h.Left, h.Right));
        private Comparator<T> compare = (T i, T j) => Comparer<T>.Default.Compare(i, j);
        private Dominator<T> dominates = (T i, T j) => false;
        private MinHeap<T> root;
    }
    public delegate int Comparator<T>(T i, T j);
    public delegate bool Dominator<T>(T i, T j);

    public class MinHeap<T>
    {
        public T Head { get; }
        public MinHeap<T> Left { get; }
        public MinHeap<T> Right { get; }
        public int Size { get; }
        public int Height { get; }
        public MinHeap(T head, MinHeap<T> left, MinHeap<T> right, int size, int height)
        {
            Head = head;
            Left = left;
            Right = right;
            Size = size;
            Height = height;
        }
    }
    public class Leaf<T> : MinHeap<T>
    {
        public Leaf() :
            base(default(T), null, null, 0, 0)
        { }
    }
    public class Branch<T> : MinHeap<T>
    {
        public Branch(T head, MinHeap<T> left, MinHeap<T> right, int size, int height) :
            base(head, left, right, size, height)
        { }
        public void Deconstruct(out T head, out MinHeap<T> left, out MinHeap<T> right, out int size, out int height)
        {
            head = Head;
            left = Left;
            right = Right;
            size = Size;
            height = Height;
        }
    }
}
