using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoire_CLI;
using Newtonsoft.Json;

namespace RNESlib.Components.Interfaces
{
    public class DrugDatabase : IDrugDatabase
    {
        private DrugsMap _drugsMap = new DrugsMap();

        public DrugDatabase()
        {
        }

        public void InitializeFromFile(string filePath)
        {
            var jsonSerializer = new JsonSerializer();
            using (var file = File.OpenText(filePath))
            {
                _drugsMap = (DrugsMap)jsonSerializer.Deserialize(file, typeof(DrugsMap));
            }
        }
        
        public List<DrugDescription> GetAvailableDrugs()
        {
            return _drugsMap.DrugsList;
        }
    }
}
