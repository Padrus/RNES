using System.Collections.Generic;
using System.Linq;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public class NetworkExtender : INetworkExtender
    {
        private readonly IDrugDatabase _drugDatabase;
        private readonly INetworkSimplifier _networkSimplifier;

        public NetworkExtender(INetworkSimplifier networkSimplifier, IDrugDatabase drugDatabase)
        {
            _drugDatabase = drugDatabase;
            _networkSimplifier = networkSimplifier;
        }
        /// <summary>
        /// Simplifies the input model and extends it with drugs susceptible of having a posiive efect on the target phenotypes.
        /// </summary>
        /// <param name="model">The binary model to extend</param>
        /// <param name="protectedNodes">The nodes that should be protected</param>
        /// <param name="targetPhenotypes">The target phenotypes</param>
        public void SimplifyAndExtendModelWithDrugs(BinaryModel model, List<string> protectedNodes, List<TargetPhenotype> targetPhenotypes)
        {
            // Clone the model
            var modelClone = model.Clone();

            // Set inputs and outputs as protected
            _networkSimplifier.ProtectInputsAndOutputs(modelClone);

            // Get available drugs list
            var drugsList = _drugDatabase.GetAvailableDrugs();

            // Set protected nodes based on configuration
            _networkSimplifier.ProtectNodes(modelClone, protectedNodes, drugsList, new List<BinaryNode>());

            // Remove all intermediary nodes 
            int removedNodes;
            do
            {
                removedNodes = _networkSimplifier.CleanupNodes(modelClone);
            } while (removedNodes != 0);

            // Add drugs
            AddDrugsToModel(modelClone, targetPhenotypes, drugsList, out var drugNodes, out var drugEdges);

            // Add the nodes to the original model
            AddEntitiesToModel(model, drugNodes, drugEdges, true);

            // Protect other nodes if nescessary
            // More than nodes impacted by drugs and mandatory nodes, protect target phenotypes nodes 
            _networkSimplifier.ProtectNodes(model, protectedNodes, new List<DrugDescription>(), GetTargetPhenotypes(model, targetPhenotypes).Keys.ToList());

            // Remove all unnescesary nodes 
            do
            {
                removedNodes = _networkSimplifier.CleanupNodes(model);
            } while (removedNodes != 0);
        }
        public NetworkExtender(IDrugDatabase drugDatabase)
        {
            _drugDatabase = drugDatabase;
        }

        /// <summary>
        /// Enriches the target model with the appropriate drugs having a positive effect on the target phenotype
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="targetPhenotypes">The target phenotypes</param>
        /// <param name="drugsList">The available drugs list</param>
        /// <param name="drugNodes">The drug nodes that have been added to the model</param>
        /// <param name="drugEdges">The drug edges that have been added to the model</param>
        public void AddDrugsToModel(BinaryModel model, List<TargetPhenotype> targetPhenotypes, List<DrugDescription> drugsList, out List<BinaryNode> drugNodes, out List<BinaryEdge> drugEdges)
        {
            var phenotypes = GetTargetPhenotypes(model, targetPhenotypes);

            // Get drugs nodes and edges for each model
            drugNodes = new List<BinaryNode>();
            drugEdges = new List<BinaryEdge>();
            foreach (var phenotype in phenotypes)
            {
                var clonedModel = model.Clone();
                FindDrugs(clonedModel, phenotype.Key.Id, phenotype.Value, drugsList, out List<BinaryNode> intDrugNodes, out List<BinaryEdge> intDrugEdges);
                foreach (var drugNode in intDrugNodes)
                {
                    if (!drugNodes.Any(n => n.Id.Equals(drugNode.Id)))
                    {
                        drugNodes.Add(drugNode);
                    }
                }

                foreach (var drugEdge in intDrugEdges)
                {
                    if (!drugEdges.Any(e => e.Input.Id.Equals(drugEdge.Input.Id) && e.Output.Id.Equals(drugEdge.Output.Id) && e.Type == drugEdge.Type))
                    {
                        drugEdges.Add(drugEdge);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the desired phenotype nodes from a target model
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="targetPhenotypes">The desired phenotypes</param>
        /// <returns></returns>
        private Dictionary<BinaryNode, DrugEffect> GetTargetPhenotypes(BinaryModel model, List<TargetPhenotype> targetPhenotypes)
        {
            // Store phenotypes and target action
            return targetPhenotypes.ToDictionary(targetPhenotype => model.Nodes.First(n => n.Id.Equals(targetPhenotype.PhenotypeId)), targetPhenotype => targetPhenotype.Effect);
        }

        /// <summary>
        /// Adds new entities to the target model
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="newNodes">The nodes to add</param>
        /// <param name="newEdges">The edges to add</param>
        /// <param name="protectNodes">If set to<c>true</c>, new nodes and nodes related to the added edges will be protected</param>
        public void AddEntitiesToModel(BinaryModel model, List<BinaryNode> newNodes, List<BinaryEdge> newEdges, bool protectNodes)
        {
            // Add them to the model without duplicates
            foreach (var drugNode in newNodes)
            {
                drugNode.Model = model;
                drugNode.Protected = protectNodes;
            }
            model.Nodes.AddRange(newNodes);
            foreach (var fullDrugEdge in newEdges)
            {
                var newEdge = new BinaryEdge(model, fullDrugEdge.Id, fullDrugEdge.Type)
                {
                    Input = model.Nodes.First(n => n.Id.Equals(fullDrugEdge.Input.Id)),
                    Output = model.Nodes.First(n => n.Id.Equals(fullDrugEdge.Output.Id))
                };
                newEdge.Input.Protected = protectNodes;
                newEdge.Output.Protected = protectNodes;
                model.Edges.Add(newEdge);
            }
        }

        /// <summary>
        /// In a target model, finds the drugs that could have the desired effect on the target node.
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="nodeId">The Id of the target node</param>
        /// <param name="drugEffect">The desired effect</param>
        /// <param name="drugsList">The list of available drugs</param>
        /// <param name="drugNodes">The new nodes added for the drugs</param>
        /// <param name="drugsEdges">The new edges added for the drugs</param>
        public void FindDrugs(BinaryModel model, string nodeId, DrugEffect drugEffect, List<DrugDescription> drugsList, out List<BinaryNode> drugNodes, out List<BinaryEdge> drugsEdges)
        {
            drugNodes = new List<BinaryNode>();
            drugsEdges = new List<BinaryEdge>();
            var node = model.GetNode(nodeId);
            if (node.State != State.Unset)
            {
                return;
            }
            node.State = drugEffect == DrugEffect.Activation ? State.Active : State.Inactive;
            var targetDrug = drugsList.FirstOrDefault(d => d.TargetId.Equals(nodeId) && d.Effect == drugEffect);
            if (targetDrug != null)
            {
                var drugNode = new BinaryNode(model, targetDrug.Id);
                drugNode.State = State.Active;
                drugNodes.Add(drugNode);
                var drugEdge = new BinaryEdge(model, string.Concat(targetDrug.Id, node.Id), targetDrug.Effect == DrugEffect.Activation ? EdgeType.Positive : EdgeType.Negative);
                drugEdge.Input = drugNode;
                drugEdge.Output = node;
                drugsEdges.Add(drugEdge);
            }
            foreach (var edge in model.Edges.Where(e => e.Output == node))
            {
                var modelClone = model.Clone();
                var inputEffect = DrugEffect.Activation;
                switch (edge.Type)
                {
                    case EdgeType.Positive:
                        inputEffect = drugEffect;
                        break;
                    case EdgeType.Negative:
                        inputEffect = drugEffect == DrugEffect.Activation
                            ? DrugEffect.Inhibition
                            : DrugEffect.Activation;
                        break;
                }
                FindDrugs(modelClone, edge.Input.Id, inputEffect, drugsList, out var newDrugNodes, out var newDrugEdges);
                drugNodes.AddRange(newDrugNodes);
                drugsEdges.AddRange(newDrugEdges);
            }
        }
    }
}
