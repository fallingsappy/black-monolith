/*Новиков В.А. aka fallingsappy
Написать функции, которые считывают матрицу смежности из файла и выводят её на
экран.*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework7_1
{
    class Program
    {

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

        static void Main(string[] args)
        {
            string filename = null;
            int i = 0, j = 0;
            int[,] a = null;
            Console.WriteLine("Эта программа считывают матрицу смежности из файла и выводят её на экран.");
            Console.Write("\nУкажите путь до файла с исходными данными или нажмите Enter, чтобы считать данные из файла Array.txt (файл по умолчанию): ");
            filename = Console.ReadLine();
            if (filename == "")
            {
                filename = "AdjacencyMatrix.txt";
            }
            a = MatrixRead(filename, out i, out j);
            Console.WriteLine("Матрица смежности: ");
            Print(i, j, a);

            Console.ReadLine();
        }
    }
}
