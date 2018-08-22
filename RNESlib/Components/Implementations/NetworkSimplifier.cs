using System.Collections.Generic;
using System.Linq;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public class NetworkSimplifier : INetworkSimplifier

    {
        /// <summary>
        /// Removes non protected nodes with only one input and one output and merges the appropriate edges.
        /// Loop nodes and nodes with no input and/or output are also removed.
        /// </summary>
        /// <param name="model">The binary model</param>
        /// <returns>The number of nodes removed</returns>
        public int CleanupNodes(BinaryModel model)
        {
            int entitiesRemoved = 0;
            var nodes = model.Nodes.ToList();
            foreach (var node in nodes)
            {
                if (node.Protected) continue;
                var canRemoveNode = true;

                var edges = model.Edges.ToList();
                var inputs = edges.Where(e => e.Output == node).ToList();
                var outputs = edges.Where(e => e.Input == node).ToList();

                // If a node has no imput nor output or is a loop, remove it
                if (inputs.Count == 0 || outputs.Count == 0)
                {
                    model.Nodes.Remove(node);
                    model.Edges.RemoveAll(e => inputs.Contains(e) || outputs.Contains(e));
                    entitiesRemoved++;
                }
                // Remove self regulations
                model.Edges.RemoveAll(e => e.Input == e.Output);

                var outputsToKeep = new List<BinaryEdge>();
                var inputsToKeep = new List<BinaryEdge>();
                var newEdgesToAdd = new List<BinaryEdge>();
                foreach (var inputEdge in inputs)
                {
                    foreach (var outputEdge in outputs)
                    {
                        var newEdge = new BinaryEdge(model, string.Concat(inputEdge.Id, outputEdge.Id), GetMergedEdgeType(inputEdge, outputEdge))
                        {
                            Input = inputEdge.Input,
                            Output = outputEdge.Output
                        };

                        if (HasOpositeEdge(model, newEdge.Input, newEdge.Output, newEdge.Type))
                        {
                            canRemoveNode = false;
                            inputsToKeep.Add(inputEdge);
                            outputsToKeep.Add(outputEdge);
                            continue;
                        }
                        if (newEdge.Input != newEdge.Output && !HasSameEdge(model, newEdge.Input, newEdge.Output, newEdge.Type))
                        {
                            newEdgesToAdd.Add(newEdge);
                        }
                    }
                }

                // Compute edges to remove
                var edgesToRemove = (model.Edges.Where(e => e.Input == node && !outputsToKeep.Contains(e) || e.Output == node && !inputsToKeep.Contains(e))).ToList();

                // Compare edges to add count with edges to remove
                var delta = newEdgesToAdd.Count - edgesToRemove.Count;
                if (delta <= 0)
                {
                    entitiesRemoved += model.Edges.RemoveAll(e => edgesToRemove.Contains(e));
                    model.Edges.AddRange(newEdgesToAdd);
                    if (!canRemoveNode) continue;
                    model.Nodes.Remove(node);
                    entitiesRemoved++;
                }
            }
            return entitiesRemoved;
        }

        /// <summary>
        /// Returns the edge type of a new edge merging two edges.
        /// </summary>
        /// <param name="inputEdge">The input edge</param>
        /// <param name="outputEdge">The output edge</param>
        /// <returns>The new edge type of the merged edge</returns>
        private EdgeType GetMergedEdgeType(BinaryEdge inputEdge, BinaryEdge outputEdge)
        {
            switch (inputEdge.Type)
            {
                case EdgeType.Positive:
                    return outputEdge.Type;
                case EdgeType.Negative:
                    return outputEdge.IsPositive() ? EdgeType.Negative : EdgeType.Positive;
                default:
                    return EdgeType.Positive;
            }
        }

        /// <summary>
        /// Protects nodes based on a list.
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="protectedNodes">The list of nodes to protect</param>
        /// <param name="drugsList">The list of drugs to protect</param>
        /// <param name="phenotypes">The list of phenotyeps to protect</param>
        public void ProtectNodes(BinaryModel model, List<string> protectedNodes, List<DrugDescription> drugsList, List<BinaryNode> phenotypes)
        {
            foreach (var binaryNode in model.Nodes)
            {
                if (protectedNodes.Contains(binaryNode.Id))
                {
                    binaryNode.Protected = true;
                }
                else if (drugsList.Any(d => d.TargetId.Equals(binaryNode.Id)))
                {
                    binaryNode.Protected = true;
                }
            }
            foreach (var binaryNode in phenotypes)
            {
                binaryNode.Protected = true;
            }
        }

        /// <summary>
        /// Protects inputs and outputs
        /// </summary>
        /// <param name="model">The target model</param>
        public void ProtectInputsAndOutputs(BinaryModel model)
        {
            foreach (var binaryNode in model.Nodes)
            {
                if (binaryNode.IsInput() || binaryNode.IsOutput())
                {
                    binaryNode.Protected = true;
                }
            }
        }

        /// <summary>
        /// Checks if a model already contains an edge with the same input, output and type
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="input">The input node</param>
        /// <param name="output">The output node</param>
        /// <param name="type">The edge type</param>
        /// <returns><c>true</c> if the model already contains the same edge, <c>false</c> otherwise</returns>
        private bool HasSameEdge(BinaryModel model, BinaryNode input, BinaryNode output, EdgeType type)
        {
            return model.Edges.Any(e => e.Input == input && e.Output == output && e.Type == type);
        }

        /// <summary>
        /// Checks if a model already contains an edge with the same input and output but a different type
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="input">The input node</param>
        /// <param name="output">The output node</param>
        /// <param name="type">The edge type</param>
        /// <returns><c>true</c> if the model already contains the oposite edge, <c>false</c> otherwise</returns>
        private bool HasOpositeEdge(BinaryModel model, BinaryNode input, BinaryNode output, EdgeType type)
        {
            return model.Edges.Any(e => e.Input == input && e.Output == output && e.Type != type);
        }
    }
}
