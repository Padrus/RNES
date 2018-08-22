using System.Collections.Generic;
using System.Linq;

namespace Memoire_CLI
{
    /// <summary>
    /// Binary model
    /// </summary>
    public class BinaryModel
    {
        public BinaryModel()
        {
            Nodes = new List<BinaryNode>();
            Edges = new List<BinaryEdge>();
        }

        /// <summary>
        /// List of all model entities.
        /// </summary>
        public IEnumerable<BinaryEntity> Entities => Edges.Concat((IEnumerable<BinaryEntity>) Nodes);

        /// <summary>
        /// List of all model edges.
        /// </summary>
        public List<BinaryEdge> Edges { get; }

        /// <summary>
        /// List of all model nodes.
        /// </summary>
        public List<BinaryNode> Nodes { get; }

        /// <summary>
        /// Clones the current model
        /// </summary>
        /// <returns>A new <see cref="BinaryModel"/> instance containing the clone</returns>
        public BinaryModel Clone()
        {
            var clonedModel = new BinaryModel();
            foreach (var binaryNode in Nodes)
            {
                clonedModel.Nodes.Add(binaryNode.Clone(clonedModel));
            }
            foreach (var binaryEdge in Edges)
            {
                clonedModel.Edges.Add(binaryEdge.Clone(clonedModel));
            }
            return clonedModel;
        }

        /// <summary>
        /// Gets a specific node based on its Id.
        /// </summary>
        /// <param name="nodeId">The target node Id</param>
        /// <returns>The node that matches <paramref name="nodeId"/> if it exists, <c>null</c> otherwise</returns>
        public BinaryNode GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id.Equals(nodeId));
        }
    }
}
