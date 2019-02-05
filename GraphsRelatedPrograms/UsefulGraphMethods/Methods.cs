/*Новиков Владимир aka fallingsappy
*Создать библиотеку функций для работы с графами.*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    public class Methods
    {
        /// <summary>
        /// Считыванием матрицы из файла. Матрица записана в формате:
        /// 0 1 0 0 1 0
        /// 1 0 1 0 1 0
        /// 0 1 0 1 0 0
        /// 0 0 1 0 1 1
        /// 1 1 0 1 0 0
        /// 0 0 0 1 0 0
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="outi"></param>
        /// <param name="outj"></param>
        /// <returns></returns>
        static int[,] MatrixRead(string filename, out int outi, out int outj)
        {
            outi = 0;
            outj = 0;
            string[] lines = File.ReadAllLines(filename);
            int[,] a = new int[lines.Length, lines[0].Split(' ').Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(' ');
                outj = temp.Length;
                outi = lines.Length;
                for (int j = 0; j < temp.Length; j++)
                    a[i, j] = Convert.ToInt32(temp[j]);
            }
            return a;
        }

        /// <summary>
        /// Вывод матриц на консоль.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="a"></param>
        static void Print(int n, int m, int[,] a)
        {
            int digitsCount = 0;
            string space = " ";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(a[i, j]);
                    digitsCount = getCountsOfDigits(a[i, j]);
                    Console.Write(space.PadRight(5 - digitsCount));
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Вспомогательный метод для форматирования вывода матрицы на экран. Считает количество цифр в числе
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static int getCountsOfDigits(int N)
        {
            int count = (N == 0) ? 1 : 0;
            while (N != 0)
            {
                count++;
                N /= 10;
            }
            return count;
        }

        /// <summary>
        /// Чтение массива, записанного в одну строку, где его элементы разделены пробелом
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int[] ArrayRead(string filename)
        {
            int[] a;
            using (StreamReader sr = new StreamReader(filename))
            {
                int N = Int32.Parse(sr.ReadLine());
                string[] s = sr.ReadLine().Split(' ');
                a = new int[N];
                for (int i = 0; i < N; i++)
                {
                    a[i] = int.Parse(s[i]);
                }
            }
            return a;
        }
    }
}
