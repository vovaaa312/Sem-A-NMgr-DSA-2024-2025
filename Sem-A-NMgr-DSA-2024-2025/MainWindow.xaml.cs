using Sem_A_NMgr_DSA_2024_2025.Graph;
using GraphSharp.Controls;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Sem_A_NMgr_DSA_2024_2025
{
    public partial class MainWindow : Window
    {
        private RoadNetworkGraph graph = new RoadNetworkGraph();
        private BidirectionalGraph<object, IEdge<object>> visualGraph = new BidirectionalGraph<object, IEdge<object>>();

        public MainWindow()
        {
            InitializeComponent();
            GraphLayout.Graph = visualGraph;
            UpdateNodesEdgesGrid();
            DrawGraph();
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            string nodeName = NodeNameInput.Text;
            if (!string.IsNullOrEmpty(nodeName))
            {
                graph.AddNode(nodeName);
                visualGraph.AddVertex(nodeName);
                UpdateNodesEdgesGrid();
                DrawGraph();
                NodeNameInput.Clear();
            }
        }

        private void RemoveNode_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = NodesEdgesGrid.SelectedItem as dynamic;
            if (selectedItem != null)
            {
                string nodeName = selectedItem.NodeName;
                if (!string.IsNullOrEmpty(nodeName))
                {
                    graph.RemoveNode(nodeName);
                    visualGraph.RemoveVertex(nodeName);
                    UpdateNodesEdgesGrid();
                    DrawGraph();
                }
            }
            else
            {
                MessageBox.Show("Please select a node to remove.");
            }
        }

        private void EditNode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editing a node is not implemented yet.");
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            string from = EdgeFromInput.Text;
            string to = EdgeToInput.Text;
            if (int.TryParse(EdgeWeightInput.Text, out int weight))
            {
                graph.AddEdge(from, to, weight);
                visualGraph.AddEdge(new WeightedEdge<object>(from, to, weight));
                UpdateNodesEdgesGrid();
                DrawGraph();
                EdgeFromInput.Clear();
                EdgeToInput.Clear();
                EdgeWeightInput.Clear();
            }
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            string from = EdgeFromInput.Text;
            string to = EdgeToInput.Text;
            graph.RemoveEdge(from, to);
            var edge = visualGraph.Edges.FirstOrDefault(e => e.Source.ToString() == from && e.Target.ToString() == to);
            if (edge != null)
            {
                visualGraph.RemoveEdge(edge);
            }
            UpdateNodesEdgesGrid();
            DrawGraph();
            EdgeFromInput.Clear();
            EdgeToInput.Clear();
        }

        private void EditEdge_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editing an edge is not implemented yet.");
        }

        private void FindShortestPath_Click(object sender, RoutedEventArgs e)
        {
            string startNode = StartNodeInput.Text;
            string endNode = EndNodeInput.Text;

            if (!string.IsNullOrEmpty(startNode) && !string.IsNullOrEmpty(endNode))
            {
                var path = graph.ShortestPath(startNode, endNode);
                if (path.Count > 0)
                {
                    UpdateShortestPathsGrid(new List<string> { string.Join(" -> ", path) });
                }
                else
                {
                    MessageBox.Show("No path found.");
                }
            }
            else
            {
                MessageBox.Show("Please enter both start and end nodes.");
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            graph = new RoadNetworkGraph();
            visualGraph = new BidirectionalGraph<object, IEdge<object>>();
            GraphLayout.Graph = visualGraph;
            UpdateNodesEdgesGrid();
            DrawGraph();
        }

        private void UpdateNodesEdgesGrid()
        {
            NodesEdgesGrid.Items.Clear();
            var nodes = graph.GetNodes().ToList();

            // Добавляем узлы
            foreach (var node in nodes)
            {
                NodesEdgesGrid.Items.Add(new { NodeName = node.GetName(), NeighborNode = "", Weight = "" });
            }

            // Добавляем грани (без дублирования)
            var addedEdges = new HashSet<string>();
            foreach (var node in nodes)
            {
                foreach (var neighbor in node.GetNeighbors())
                {
                    string edgeKey = $"{node.GetName()}-{neighbor.Key.GetName()}";
                    string reverseEdgeKey = $"{neighbor.Key.GetName()}-{node.GetName()}";

                    if (!addedEdges.Contains(edgeKey) && !addedEdges.Contains(reverseEdgeKey))
                    {
                        NodesEdgesGrid.Items.Add(new { NodeName = node.GetName(), NeighborNode = neighbor.Key.GetName(), Weight = neighbor.Value });
                        addedEdges.Add(edgeKey);
                    }
                }
            }
        }

        private void UpdateShortestPathsGrid(List<string> paths)
        {
            ShortestPathsGrid.Items.Clear();
            foreach (var path in paths)
            {
                var parts = path.Split(new[] { " -> " }, StringSplitOptions.None);
                ShortestPathsGrid.Items.Add(new { StartNode = parts[0], EndNode = parts[parts.Length - 1], Path = path, Length = parts.Length - 1 });
            }
        }

        private void DrawGraph()
        {
            GraphLayout.Relayout();
        }
    }

    // Класс для хранения грани с весом
    public class WeightedEdge<T> : Edge<T>
    {
        public int Weight { get; }

        public WeightedEdge(T source, T target, int weight) : base(source, target)
        {
            Weight = weight;
        }
    }
}