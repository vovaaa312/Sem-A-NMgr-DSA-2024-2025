using System;
using System.Collections.Generic;
using System.Linq;

namespace Sem_A_NMgr_DSA_2024_2025.Graph
{
    public class RoadNetworkGraph : AbstractGraph
    {
        public override void AddEdge(string from, string to, int weight)
        {
            AddNode(from);
            AddNode(to);

            if (!nodes[from].GetNeighbors().ContainsKey(nodes[to]))
            {
                nodes[from].AddNeighbor(nodes[to], weight);
                nodes[to].AddNeighbor(nodes[from], weight);
            }
        }

        public override void RemoveEdge(string from, string to)
        {
            if (nodes.ContainsKey(from) && nodes.ContainsKey(to))
            {
                nodes[from].RemoveNeighbor(nodes[to]);
                nodes[to].RemoveNeighbor(nodes[from]);
            }
        }

        public override List<string> ShortestPath(string start, string end)
        {
            var distances = new Dictionary<Node, int>();
            var previous = new Dictionary<Node, Node>();
            //var queue = new PriorityQueue<Node>(Comparer<Node>.Create((a, b) => distances[a] - distances[b]));
            var queue = new PriorityQueue<Node>(Comparer<Node>.Create((a, b) => distances[a] - distances[b]));

            Node source = GetNode(start);
            Node target = GetNode(end);

            foreach (var node in nodes.Values)
            {
                distances[node] = int.MaxValue;
            }
            distances[source] = 0;

            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current == target) break;

                foreach (var entry in current.GetNeighbors())
                {
                    Node neighbor = entry.Key;
                    int newDist = distances[current] + entry.Value;

                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previous[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            List<string> path = new List<string>();
            for (Node at = target; at != null; at = previous.ContainsKey(at) ? previous[at] : null)
            {
                path.Add(at.GetName());
            }

            path.Reverse();
            return path;
        }

        public List<string> ExportPathsFromNode(string start)
        {
            List<string> paths = new List<string>();
            Node source = GetNode(start);

            foreach (var target in nodes.Values)
            {
                if (!source.Equals(target))
                {
                    List<string> path = ShortestPath(source.GetName(), target.GetName());
                    if (path.Count > 0)
                    {
                        paths.Add(string.Join(" -> ", path));
                    }
                }
            }

            return paths;
        }

        public List<string> ExportPathsToNode(string end)
        {
            List<string> paths = new List<string>();
            Node target = GetNode(end);

            foreach (var source in nodes.Values)
            {
                if (!source.Equals(target))
                {
                    List<string> path = ShortestPath(source.GetName(), target.GetName());
                    if (path.Count > 0)
                    {
                        paths.Add(string.Join(" -> ", path));
                    }
                }
            }

            return paths;
        }
    }
}