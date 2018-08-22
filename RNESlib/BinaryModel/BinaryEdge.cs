using System.Linq;

namespace Memoire_CLI
{
    public class BinaryEdge : BinaryEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">The parent model</param>
        /// <param name="id">The edge Id</param>
        /// <param name="type">The edge type</param>
        public BinaryEdge(BinaryModel model, string id, EdgeType type) : base(model, id)
        {
            Type = type;
        }

        /// <summary>
        /// Edge output
        /// </summary>
        public BinaryNode Output { get; set; }

        /// <summary>
        /// Edge input
        /// </summary>
        public BinaryNode Input { get; set; }

        /// <summary>
        /// Edge type
        /// </summary>
        public EdgeType Type { get; }

        /// <summary>
        /// Checks if the current edge type is positive
        /// </summary>
        /// <returns><c>true</c> if the edge type is positive, <c>flase</c> otherwise</returns>
        public bool IsPositive()
        {
            return Type == EdgeType.Positive;
        }

        /// <summary>
        /// Checks is the current edge type is negative
        /// </summary>
        /// <returns><c>true</c> is the edge type is negative, <c>false</c> otherwise</returns>
        public bool IsNegative()
        {
            return Type == EdgeType.Negative;
        }

        /// <summary>
        /// Checks if all the conditons are met for the current edge to activate.
        /// An edge can activate if its input node is active.
        /// </summary>
        /// <returns></returns>
        public override bool CanActivate()
        {
            return Input.IsActive;
        }

        /// <summary>
        /// Clones the curent edge.
        /// </summary>
        /// <param name="model">the model in which the clone should be put</param>
        /// <returns>A new <see cref="BinaryEdge"/> instance containing the clone</returns>
        public BinaryEdge Clone(BinaryModel model)
        {
            var clone = new BinaryEdge(model, Id, Type)
            {
                Input = model.Nodes.First(n => n.Id.Equals(Input.Id)),
                Output = model.Nodes.First(n => n.Id.Equals(Output.Id)),
                State = State
            };
            return clone;
        }
    }

    /// <summary>
    /// Enumeration of all the possible binary edge types
    /// </summary>
    public enum EdgeType {
        Positive,
        Negative
    }
}
