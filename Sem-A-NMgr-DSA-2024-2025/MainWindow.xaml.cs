using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<RoutingMatrixEntry> _routingMatrixData = new ObservableCollection<RoutingMatrixEntry>();
        private ObservableCollection<SuccessorVectorEntry> _successorVectorData = new ObservableCollection<SuccessorVectorEntry>();

        public MainWindow()
        {
            InitializeComponent();
            GraphLayout.Graph = visualGraph;

            // Привязка данных
            ShortestPathsGrid.ItemsSource = _routingMatrixData;
            SuccessorVectorGrid.ItemsSource = _successorVectorData;

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

                UpdateRoutingMatrix(nodeName);
                UpdateSuccessorVector(nodeName);
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

                    UpdateRoutingMatrix(nodeName);
                    UpdateSuccessorVector(nodeName);
                }
            }
            else
            {
                ShowWarningMessage("Please select a node to remove.");
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

                try
                {
                    visualGraph.AddEdge(edge);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex);
                }

                UpdateNodesEdgesGrid();
                DrawGraph();

                EdgeFromInput.Clear();
                EdgeToInput.Clear();
                EdgeWeightInput.Clear();

                UpdateRoutingMatrix(from);
                UpdateSuccessorVector(to);
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

                    UpdateRoutingMatrix(from);
                    UpdateSuccessorVector(to);
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

            NodesEdgesGrid.ItemsSource = null;
            _routingMatrixData.Clear();
            _successorVectorData.Clear();

            // Обновляем интерфейс
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
            // Очищаем старые данные
            _routingMatrixData.Clear();

            // Добавляем новые данные
            foreach (var path in paths)
            {
                var parts = path.Split(new[] { " -> " }, StringSplitOptions.None);
                _routingMatrixData.Add(new RoutingMatrixEntry
                {
                    StartNode = parts[0],
                    EndNode = parts.Last(),
                    Path = path, // Путь в виде строки
                    Weight = CalculatePathWeight(path)
                });
            }
        }

        private int CalculatePathWeight(string path)
        {
            var parts = path.Split(new[] { " -> " }, StringSplitOptions.None);
            int weight = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var from = parts[i];
                var to = parts[i + 1];
                var node = graph.GetNode(from);
                if (node != null && node.GetNeighbors().ContainsKey(graph.GetNode(to)))
                {
                    weight += node.GetNeighbors()[graph.GetNode(to)];
                }
            }
            return weight;
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
                try
                {
                    var paths = graph.ExportPathsFromNode(nodeName);
                    UpdateShortestPathsGrid(paths);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex);
                }

            }
            else
            {
                ShowWarningMessage("Please enter a node name.");
            }
        }
        //private void ExportPathsFromNode_Click(object sender, RoutedEventArgs e)
        //{
        //    if (graph.isEmpty())
        //    {
        //        ShowWarningMessage("Graph is empty");
        //        return;
        //    }

        //    string nodeName = ExportNodeInput.Text;
        //    if (!string.IsNullOrEmpty(nodeName))
        //    {
        //        try
        //        {
        //            var paths = graph.ExportPathsFromNode(nodeName);


        //            UpdateShortestPathsGrid(paths);
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowErrorMessage(ex);
        //            return;
        //        }

        //        UpdateRoutingMatrix(nodeName);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a node name.");
        //    }
        //}
        private void ExportPathsToNode_Click(object sender, RoutedEventArgs e)
        {

            if (graph.isEmpty()) ShowWarningMessage("Graph is empty");

            string nodeName = ExportNodeInput.Text;
            if (string.IsNullOrEmpty(nodeName))
            {
                ShowWarningMessage("Please enter a node name.");
                return;
            }

            try
            {
                var paths = graph.ExportPathsToNode(nodeName);
                UpdateShortestPathsGrid(paths);

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
                return;
            }

            UpdateSuccessorVector(nodeName);
        }

        private void SaveGraphToFile_Click(object sender, RoutedEventArgs e)
        {
            if (graph.isEmpty())
            {
                ShowWarningMessage("Graph is empty");
                return;
            }


            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Graph files (*.graph)|*.graph|All files (*.*)|*.*",
                DefaultExt = ".graph"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (var writer = new System.IO.StreamWriter(filePath))
                    {
                        foreach (var node in graph.GetNodes())
                        {
                            writer.WriteLine($"NODE:{node.GetName()}");
                        }

                        foreach (var node in graph.GetNodes())
                        {
                            foreach (var neighbor in node.GetNeighbors())
                            {
                                writer.WriteLine($"EDGE:{node.GetName()},{neighbor.Key.GetName()},{neighbor.Value}");
                            }
                        }
                    }

                    MessageBox.Show("Graph saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save graph: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadGraphFromFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Graph files (*.graph)|*.graph|All files (*.*)|*.*",
                DefaultExt = ".graph"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    graph = new RoadNetworkGraph();
                    visualGraph = new BidirectionalGraph<object, IEdge<object>>();
                    GraphLayout.Graph = visualGraph;

                    using (var reader = new System.IO.StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.StartsWith("NODE:"))
                            {
                                string nodeName = line.Substring("NODE:".Length);
                                graph.AddNode(nodeName);
                                visualGraph.AddVertex(nodeName);
                            }
                            else if (line.StartsWith("EDGE:"))
                            {
                                string[] parts = line.Substring("EDGE:".Length).Split(',');
                                string from = parts[0];
                                string to = parts[1];
                                int weight = int.Parse(parts[2]);

                                graph.AddEdge(from, to, weight);
                                var edge = new WeightedEdge<object>(from, to, weight);
                                visualGraph.AddEdge(edge);
                            }
                        }
                    }
                    UpdateNodesEdgesGrid();
                    DrawGraph();

                    if (graph.GetNodes().Any())
                    {
                        string firstNode = graph.GetNodes().First().GetName();
                        UpdateRoutingMatrix(firstNode);
                        UpdateSuccessorVector(firstNode);
                        UpdateShortestPathsGrid(graph.ExportPathsFromNode(firstNode));

                    }

                    MessageBox.Show("Graph loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load graph: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateRoutingMatrix(string startNode)
        {
            _routingMatrixData.Clear();
            var routingMatrix = graph.GetRoutingMatrix(startNode);
            foreach (var entry in routingMatrix)
            {
                _routingMatrixData.Add(entry);
            }
        }

        private void UpdateSuccessorVector(string endNode)
        {
            _successorVectorData.Clear();
            var successorVector = graph.GetSuccessorVector(endNode);

            foreach (var entry in successorVector)
            {
                _successorVectorData.Add(new SuccessorVectorEntry
                {
                    Node = entry.Node,
                    NextNode = entry.NextNode,
                    Weight = entry.Weight
                });
            }
        }

        //private void UpdateDirectedGraph(string startNode)
        //{
        //    var directedGraph = graph.GenerateDirectedGraph(startNode);
        //    DirectedGraphLayout.Graph = directedGraph;
        //}

        private void ShowErrorMessage(Exception e)
        {
            ShowCustomMessage(
                "An error occurred:\n\n" + e.Message,
                e.GetType().ToString(),
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowWarningMessage(string v)
        {

            ShowCustomMessage("An error occurred:\n\n" + v,
            "Warning",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
        }

        private void ShowCustomMessage(string message, string header, MessageBoxButton button, MessageBoxImage image)
        {
            MessageBox.Show(message, header, button, image);
        }
    }
}