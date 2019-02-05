/*Новиков В.А. aka fallingsappy
Реализовать алгоритм Дейкстры*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace DijkstraAlgorithm
{
    class Dijkstra
    {

        private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private static void Print(int[] distance, int verticesCount)
        {
            Console.WriteLine("Вершина    Расстояние от заданной вершины");

            for (int i = 0; i < verticesCount; ++i)
                Console.WriteLine("  {0}\t              {1}", i, distance[i]);
        }

        public static void DijkstraAlgo(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                distance[i] = int.MaxValue;
                shortestPathTreeSet[i] = false;
            }

            distance[source] = 0;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                        distance[v] = distance[u] + graph[u, v];
            }

            Print(distance, verticesCount);
        }

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

        static void Main(string[] args)
        {
            Console.WriteLine("Эта программа реализует алгоритм Дейкстры:\n");

            int[,] graph = null;
            string filename = "AdjacencyMatrix.txt";
            int i = 0, j = 0;

            graph = MatrixRead(filename, out i, out j);

            DijkstraAlgo(graph, 0, 8);
            Console.ReadLine();
        }
    }
}
