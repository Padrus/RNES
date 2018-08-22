using System;
using System.Resources;
using System.Text;
using libsbmlcs;

namespace Memoire_CLI
{
    /// <summary>
    /// Base calss of any binary entity
    /// </summary>
    public abstract class BinaryEntity
    {
        public BinaryModel Model { get; set; }

        /// <summary>
        /// protected constructor
        /// </summary>
        /// <param name="model">The parent model</param>
        /// <param name="id">The entity Id</param>
        protected BinaryEntity(BinaryModel model, string id)
        {
            Model = model;
            Id = id;
            State = State.Unset;
        }

        /// <summary>
        /// Checks if the current entity is active
        /// </summary>
        public bool IsActive => State == State.Active;

        /// <summary>
        /// Checks if the current edge is inactive
        /// </summary>
        /// <returns></returns>
        public bool IsInactive => State == State.Inactive;

        /// <summary>
        /// Current entity state
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Activates current entity
        /// </summary>
        public void Activate()
        {
            State = State.Active;
        }

        /// <summary>
        /// Deactivate the current enity
        /// </summary>
        public void Deactivate()
        {
            State = State.Inactive;
        }

        public abstract bool CanActivate();

        /// <summary>
        /// The entity identifier
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Enity possble states
    /// </summary>
    public enum State
    {
        Active,
        Inactive,
        Unset
    }
}
