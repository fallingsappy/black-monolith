using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    class TNode<T>
    {
        public T Data { get; set; }
        public TNode<T> Next { get; set; }
        public TNode(T data)
        {
            Data = data;
        }
        internal class LQueue<T> : IEnumerable<T>
        {
            private TNode<T> _end;
            private TNode<T> _head;
            public int Size { get; private set; }
            public IEnumerator<T> GetEnumerator()
            {
                while (!Empty)
                {
                    var item = Front;
                    Pop();
                    yield return item;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public bool Empty
            {
                get { return Size == 0; }
            }
            public LQueue()
            {
                _end = new TNode<T>(default(T));
                _head = _end;
                Size = 0;
            }
            public T Front
            {
                get
                {
                    if (Empty)
                        throw new InvalidOperationException();
                    return _head.Data;
                }
            }
            public void Pop()
            {
                if (_head == null)
                    throw new InvalidOperationException();
                _head = _head.Next ?? _end;
                --Size;
            }
            public void Push(T item)
            {
                if (Empty)
                {
                    _end.Data = item;
                }
                else
                {
                    var node = new TNode<T>(item);
                    _end.Next = node;
                    _end = _end.Next;
                }
                ++Size;
            }
        }
    }
}
