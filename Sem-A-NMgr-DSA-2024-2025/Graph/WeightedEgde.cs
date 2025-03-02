using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem_A_NMgr_DSA_2024_2025.Graph
{
    public class WeightedEdge<T> : Edge<T>
    {
        public int Weight { get; }

        public WeightedEdge(T source, T target, int weight) : base(source, target)
        {
            Weight = weight;
        }
    }
}
