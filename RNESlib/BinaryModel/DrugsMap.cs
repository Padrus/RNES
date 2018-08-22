using System;
using System.Collections.Generic;

namespace Memoire_CLI
{
    /// <summary>
    /// This class represents a mapping between drugs and proteins including the kinf off effect they have on the protein (activation or inhibition)
    /// </summary>
    [Serializable]
    public class DrugsMap
    {
        public DrugsMap()
        {
            DrugsList = new List<DrugDescription>();
        }

        public List<DrugDescription> DrugsList { get; set; }
    }


    [Serializable]
    public class DrugDescription
    {
        public DrugDescription()
        {
            
        }

        public DrugDescription(string id, string targetId, DrugEffect effect)
        {
            Id = id;
            TargetId = targetId;
            Effect = effect;
        }

        public string Id { get; set; }
        public string TargetId { get; set; }
        public DrugEffect Effect { get; set; }
    }

    [Serializable]
    public enum DrugEffect
    {
        Activation,
        Inhibition
    }
}
