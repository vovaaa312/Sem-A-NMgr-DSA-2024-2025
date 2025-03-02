using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphSharp.Controls;
using QuickGraph;
using Sem_A_NMgr_DSA_2024_2025.Graph;

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
                var edge = new WeightedEdge<object>(from, to, weight);
                visualGraph.AddEdge(edge);

                UpdateNodesEdgesGrid();
                DrawGraph();

                EdgeFromInput.Clear();
                EdgeToInput.Clear();
                EdgeWeightInput.Clear();
            }
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = NodesEdgesGrid.SelectedItem as dynamic;
            if (selectedItem != null)
            {
                string from = selectedItem.NodeName;
                string to = selectedItem.NeighborNode;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    graph.RemoveEdge(from, to);
                    var edge = visualGraph.Edges.FirstOrDefault(e => e.Source.ToString() == from && e.Target.ToString() == to);
                    if (edge != null)
                    {
                        visualGraph.RemoveEdge(edge);
                    }
                    UpdateNodesEdgesGrid();
                    DrawGraph();
                }
            }
            else
            {
                MessageBox.Show("Please select an edge to remove.");
            }
        }

        private void EditEdge_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editing an edge is not implemented yet.");
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

            foreach (var node in nodes)
            {
                NodesEdgesGrid.Items.Add(new { NodeName = node.GetName(), NeighborNode = "", Weight = "" });
            }

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

        private void ExportPathsFromNode_Click(object sender, RoutedEventArgs e)
        {
            string nodeName = ExportNodeInput.Text;
            if (!string.IsNullOrEmpty(nodeName))
            {
                var paths = graph.ExportPathsFromNode(nodeName);
                UpdateShortestPathsGrid(paths);
            }
            else
            {
                MessageBox.Show("Please enter a node name.");
            }
        }

        private void ExportPathsToNode_Click(object sender, RoutedEventArgs e)
        {
            string nodeName = ExportNodeInput.Text;
            if (!string.IsNullOrEmpty(nodeName))
            {
                var paths = graph.ExportPathsToNode(nodeName);
                UpdateShortestPathsGrid(paths);
            }
            else
            {
                MessageBox.Show("Please enter a node name.");
            }
        }
    }


}