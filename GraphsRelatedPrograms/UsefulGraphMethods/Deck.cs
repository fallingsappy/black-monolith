using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    class Deck<T>
    {
        T[] array;

        public Deck()
        {
            array = new T[0];
        }

        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        public bool Empty
        {
            get
            {
                return array.Length > 0;
            }
        }

        public void PushBack(T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
        }
        public void PushFront(T item)
        {
            Array.Resize(ref array, array.Length + 1);
            for (int i = array.Length - 1; i > 0; i--)
                array[i] = array[i - 1];
            array[0] = item;
        }

        public T PopBack()
        {
            T item = array[array.Length - 1];
            Array.Resize(ref array, array.Length - 1);
            return item;
        }
        public T PopFront()
        {
            T item = array[0];
            for (int i = 0; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            Array.Resize(ref array, array.Length - 1);
            return item;
        }
        public T Front
        {
            get
            {
                return array[0];
            }
        }
        public T Back
        {
            get
            {
                return array[array.Length - 1];
            }
        }
    }
}
