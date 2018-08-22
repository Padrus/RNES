using System.Collections.Generic;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public interface INetworkSimplifier
    {
        /// <summary>
        ///     Removes non protected nodes with only one input and one output and merges the appropriate edges.
        ///     Loop nodes and nodes with no input and/or output are also removed.
        /// </summary>
        /// <param name="model">The binary model</param>
        /// <returns>The number of nodes removed</returns>
        int CleanupNodes(BinaryModel model);

        /// <summary>
        ///     Protects nodes based on a list.
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="protectedNodes">The list of nodes to protect</param>
        /// <param name="drugsList">The list of drugs to protect</param>
        /// <param name="phenotypes">The list of phenotyeps to protect</param>
        void ProtectNodes(BinaryModel model, List<string> protectedNodes, List<DrugDescription> drugsList,
            List<BinaryNode> phenotypes);

        /// <summary>
        ///     Protects inputs and outputs
        /// </summary>
        /// <param name="model">The target model</param>
        void ProtectInputsAndOutputs(BinaryModel model);
    }
}