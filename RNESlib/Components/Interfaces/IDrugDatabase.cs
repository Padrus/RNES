using System.Collections.Generic;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public interface IDrugDatabase
    {
        /// <summary>
        /// Gets the list of available drugs in the system.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> containing the available drugs in the system</returns>
        List<DrugDescription> GetAvailableDrugs();

        /// <summary>
        /// initializes the drug database from a json file.
        /// </summary>
        /// <param name="filePath">The target fil path</param>
        void InitializeFromFile(string filePath);
    }
}
