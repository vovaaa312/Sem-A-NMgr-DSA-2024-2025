using QuickGraph;
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
            var queue = new PriorityQueue<Node>(Comparer<Node>.Create((a, b) => distances[a] - distances[b]));

            Node source = GetNode(start);
            Node target = GetNode(end);

            if (source == null || target == null)
            {
                return new List<string>(); 
            }

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

        public List<RoutingMatrixEntry> GetRoutingMatrix(string startNode)
        {
            var routingMatrix = new List<RoutingMatrixEntry>();
            var shortestPaths = new Dictionary<string, List<string>>();

            
            foreach (var node in nodes.Values)
            {
                if (node.GetName() != startNode)
                {
                    var path = ShortestPath(startNode, node.GetName());
                    shortestPaths[node.GetName()] = path;
                }
            }

            // Заполняем таблицу
            foreach (var targetNode in nodes.Values)
            {
                if (targetNode.GetName() != startNode)
                {
                    var path = shortestPaths[targetNode.GetName()];
                    if (path.Count > 1)
                    {
                        routingMatrix.Add(new RoutingMatrixEntry
                        {
                            StartNode = startNode,
                            EndNode = targetNode.GetName(),
                            NextNode = path[1], // Следующая вершина на пути
                            Weight = nodes[startNode].GetNeighbors()[nodes[path[1]]]
                        });
                    }
                }
            }

            return routingMatrix;
        }

        public List<SuccessorVectorEntry> GetSuccessorVector()
        {
            var successorVector = new List<SuccessorVectorEntry>();

            foreach (var node in nodes.Values)
            {
                successorVector.Add(new SuccessorVectorEntry
                {
                    Node = node.GetName(),
                    Neighbors = node.GetNeighbors().Keys.ToList() 
                });
            }

            return successorVector;
        }


    }

    public class RoutingMatrixEntry
    {
        public string StartNode { get; set; }
        public string EndNode { get; set; }
        public string NextNode { get; set; }

        public string Path { get; set; }
        public int Weight { get; set; }
    }


    public class SuccessorVectorEntry
    {
        public string Node { get; set; } 
        public List<Node> Neighbors { get; set; } 

        public string NeighborsString => string.Join(", ", Neighbors.Select(n => n.GetName()));
    }
}

