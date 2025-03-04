using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sem_A_NMgr_DSA_2024_2025.Graph
{
    public abstract class AbstractGraph
    {
        protected Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        public abstract void AddEdge(string from, string to, int weight);

        public abstract void RemoveEdge(string from, string to);


        public void AddNode(string name)
        {
            if (!nodes.ContainsKey(name))
            {
                nodes[name] = new Node(name);
            }
        }

        public void RemoveNode(string name)
        {
            if (nodes.ContainsKey(name))
            {
                var nodeToRemove = nodes[name];
                foreach (var neighbor in nodeToRemove.GetNeighbors().Keys)
                {
                    neighbor.RemoveNeighbor(nodeToRemove);
                }
                nodes.Remove(name);
            }
        }
        public abstract List<string> ShortestPath(string start, string end);

        public Node GetNode(string name)
        {
            return nodes.ContainsKey(name) ? nodes[name] : null;
        }

        public ICollection<Node> GetNodes()
        {
            return nodes.Values;
        }

        public bool isEmpty() {
            return nodes.Count == 0;
        }
    }
}
