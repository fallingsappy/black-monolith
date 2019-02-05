using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    public class Stack<T>
    {
        private T[] s;
        private int currentStackIndex;

        public Stack(int N)
        {
            if (N < 0)
                throw new ArgumentOutOfRangeException("N");

            s = new T[N];
            currentStackIndex = 0;
        }

        public void push(T x)
        {
            if (currentStackIndex + 1 >= s.Length)
            {
                Array.Resize(ref s, (s.Length + 1) * 2);
            }

            s[currentStackIndex++] = x;
        }

        public T pop()
        {
            if (currentStackIndex == 0)
                throw new InvalidOperationException("Стэк пуст");

            T value = s[--currentStackIndex];
            s[currentStackIndex] = default(T);
            return value;
        }
    }
}
