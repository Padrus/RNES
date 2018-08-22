using System.Linq;

namespace Memoire_CLI
{
    public class BinaryNode : BinaryEntity
    {
        /// <summary>
        /// Checks if the current node is an input in th model
        /// </summary>
        /// <returns><c>true</c> if the node is an input of the model, <c>false</c> otherwise</returns>
        public bool IsInput()
        {
            return Model.Edges.All(e => e.Output != this);
        }

        /// <summary>
        /// Checks if the current node is an output of the model
        /// </summary>
        /// <returns><c>true</c> if the node is an output of the model, <c>false</c> otherwise</returns>
        public bool IsOutput()
        {
            return Model.Edges.All(e => e.Input != this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">The parent model</param>
        /// <param name="id">The node id</param>
        public BinaryNode(BinaryModel model, string id) : base(model, id)
        {
        }

        /// <summary>
        /// Checks if all the conditions are met for the current node to activate.
        /// For a node to activate, all positive incoming edges shoud be active and all negative inccoming edges should be inactive.
        /// </summary>
        /// <returns><c>true</c> if the node can activate, <c>false</c> otherwise</returns>
        public override bool CanActivate()
        {
            return Model.Edges.Where(e => e.Output == this)
                .All(e => e.IsPositive() && e.IsActive || e.IsNegative() && e.IsInactive);
        }

        /// <summary>
        /// Node display name   
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines if the node can be removed when simplifying the model or not
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// Clones the curent node.
        /// </summary>
        /// <param name="model">the model in which the clone should be put</param>
        /// <returns>A new <see cref="BinaryNode"/> instance containing the clone</returns>
        public BinaryNode Clone(BinaryModel model)
        {
            var clone = new BinaryNode(model, Id)
            {
                Name = Name,
                Protected = Protected,
                State = State
            };

            return clone;
        }
    }
}
