using System;
using System.Collections.Generic;

namespace Memoire_CLI
{
    [Serializable]
    public class ProtectedEntities
    {
        public ProtectedEntities()
        {
            EntitiestList = new List<string>();
        }

        public List<string> EntitiestList { get; set; }
    }
}
