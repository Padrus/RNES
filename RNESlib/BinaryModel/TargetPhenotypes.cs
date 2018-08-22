using System;
using System.Collections.Generic;

namespace Memoire_CLI
{
    [Serializable]
    public class TargetPhenotypes
    {
        public List<TargetPhenotype> PhenotypesList { get; set; }

        public TargetPhenotypes()
        {
            PhenotypesList = new List<TargetPhenotype>();
        }
    }

    [Serializable]
    public class TargetPhenotype
    {
        public TargetPhenotype(string phenotypeId, DrugEffect effect)
        {
            PhenotypeId = phenotypeId;
            Effect = effect;
        }

        public string PhenotypeId { get; set; }
        public DrugEffect Effect { get; set; }
    }
}
