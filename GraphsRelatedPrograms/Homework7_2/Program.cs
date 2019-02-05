/*Новиков В.А. aka fallingsappy
Написать рекурсивную функцию обхода графа в глубину.*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework7_2
{
    class Program
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
        public static int[,] MatrixRead(string filename, out int outi, out int outj)
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

        static void Main(string[] args)
        {
            Console.WriteLine("Эта программа рекурсивно обходит граф в глубину.");
            Console.WriteLine("Исходный граф представлен в виде таблицы, где левый столбец исходная вершина,\nа правый целевая:");

            int[,] a = null;
            int idim, jdim; 
            string filename = "GraphList.txt";

            a = MatrixRead(filename, out idim, out jdim);
            Print(idim, jdim, a);


            var g = new Graph();

            for (int i = 0; i < idim; i++)
            {
                for (int j = 0; j < jdim; j++)
                    g.AddEdge(a[i,0], a[i,j]);
            }

            Console.WriteLine("Результат обхода: ");
            g.DFSWithRecursion(1);
            Console.ReadLine();
        }
    }

    
    public class Graph
    {
        public Graph()
        {
            Adj = new Dictionary<int, HashSet<int>>();
        }

        public Dictionary<int, HashSet<int>> Adj { get; private set; }

        public void AddEdge(int source, int target)
        {
            if (Adj.ContainsKey(source))
            {
                try
                {
                    Adj[source].Add(target);
                }
                catch
                {
                    Console.WriteLine("Это ребро уже существует: " + source + " -> " + target);
                }
            }
            else
            {
                var hs = new HashSet<int>();
                hs.Add(target);
                Adj.Add(source, hs);
            }
        }

        public void DFSWithRecursion(int vertex)
        {
            var visited = new HashSet<int>();
            Traverse(vertex, visited);
        }

        private void Traverse(int v, HashSet<int> visited)
        {
            visited.Add(v);
            Console.Write(" -> {0} ",v);
            if (Adj.ContainsKey(v))
            {
                foreach (int neighbour in Adj[v].Where(a => !visited.Contains(a)))
                {
                    Traverse(neighbour, visited);
                }
            }
        }
    }
}
