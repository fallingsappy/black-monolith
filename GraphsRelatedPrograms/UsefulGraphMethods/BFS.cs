using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulGraphMethods
{
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

        public void BFSWalkWithStartNode(int vertex)
        {
            var visited = new HashSet<int>();
            visited.Add(vertex);
            var q = new Queue<int>();
            q.Enqueue(vertex);

            while (q.Count > 0)
            {
                var current = q.Dequeue();
                Console.Write(" -> {0}", current);
                if (Adj.ContainsKey(current))
                {
                    foreach (int neighbour in Adj[current].Where(a => !visited.Contains(a)))
                    {
                        visited.Add(neighbour);
                        q.Enqueue(neighbour);
                    }
                }
            }
        }
    }
}
