using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    public class MQueue<T> : IEnumerable
    {
        private int Front = -1;
        private int Rear = -1;
        private int _Count = 0;
        private readonly int _Size;
        private readonly T[] Array;

        public MQueue(int Size)
        {
            this._Size = Size;
            this.Array = new T[Size];
        }

        public bool IsFull()
        {
            return Rear == Size - 1;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Enqueue(T item)
        {
            if (this.IsFull())
                throw new Exception("Очередь полностью заполнена.");
            Array[++Rear] = item;
            _Count++;
        }

        public T Dequeue()
        {
            if (this.IsEmpty())
                throw new Exception("Очередь пуста.");
            T value = Array[++Front];
            _Count--;
            if (Front == Rear)
            {
                Front = -1;
                Rear = -1;
            }
            return value;
        }

        public int Size
        {
            get { return _Size; }
        }

        public int Count
        {
            get { return _Count; }
        }

        public T Peek()
        {
            if (this.IsEmpty())
                throw new Exception("Очередь пуста.");

            T value = Array[Front + 1];
            return value;
        }

        public IEnumerator GetEnumerator()
        {
            if (this.IsEmpty())
                throw new Exception("Очередь пуста.");

            for (int i = Front + 1; i <= Rear; i++)
                yield return Array[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
