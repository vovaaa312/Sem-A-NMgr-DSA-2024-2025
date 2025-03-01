using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem_A_NMgr_DSA_2024_2025.Graph
{
    public class Node
    {
        private readonly string name;
        private readonly Dictionary<Node, int> neighbors = new Dictionary<Node, int>();

        public Node(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public Dictionary<Node, int> GetNeighbors()
        {
            return neighbors;
        }

        public void AddNeighbor(Node node, int weight)
        {
            neighbors[node] = weight;
        }

        public void RemoveNeighbor(Node node)
        {
            neighbors.Remove(node);
        }
    }
}
