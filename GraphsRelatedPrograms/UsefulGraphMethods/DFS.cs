using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
    public class Graph1
    {
        public Graph1()
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
            Console.Write(" -> {0} ", v);
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
