using System.Collections.Generic;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public interface INetworkExtender
    {
        /// <summary>
        /// Simplifies the input model and extends it with drugs susceptible of having a posiive efect on the target phenotypes.
        /// </summary>
        /// <param name="model">The binary model to extend</param>
        /// <param name="protectedNodes">The nodes that should be protected</param>
        /// <param name="targetPhenotypes">The target phenotypes</param>
        void SimplifyAndExtendModelWithDrugs(BinaryModel model, List<string> protectedNodes,
            List<TargetPhenotype> targetPhenotypes);

        /// <summary>
        /// Enriches the target model with the appropriate drugs having a positive effect on the target phenotype
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="targetPhenotypes">The target phenotypes</param>
        /// <param name="drugsList">The available drugs list</param>
        /// <param name="drugNodes">The drug nodes that have been added to the model</param>
        /// <param name="drugEdges">The drug edges that have been added to the model</param>
        void AddDrugsToModel(BinaryModel model, List<TargetPhenotype> targetPhenotypes, List<DrugDescription> drugsList, out List<BinaryNode> drugNodes, out List<BinaryEdge> drugEdges);

        /// <summary>
        /// Adds new entities to the target model
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="newNodes">The nodes to add</param>
        /// <param name="newEdges">The edges to add</param>
        /// <param name="protectNodes">If set to<c>true</c>, new nodes and nodes related to the added edges will be protected</param>
        void AddEntitiesToModel(BinaryModel model, List<BinaryNode> newNodes, List<BinaryEdge> newEdges, bool protectNodes);
    }
}
