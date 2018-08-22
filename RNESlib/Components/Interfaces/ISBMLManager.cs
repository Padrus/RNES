using libsbmlcs;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public interface ISBMLManager
    {
        /// <summary>
        /// Initializes and returns a new binary model based on an SBML file.
        /// </summary>
        /// <param name="filePath">The SBML file path</param>
        /// <returns>A new <see cref="BinaryModel"/> instance initialized from <paramref name="filePath"/></returns>
        BinaryModel ReadFromSbmlFile(string filePath);

        /// <summary>
        ///     Creates a new binary model based onan SBML Qual model.
        /// </summary>
        void InitFromSbmlQualModel(BinaryModel model, Model sbmlModel);
        
        /// <summary>
        ///     Converts the current binary model to an SBML 3.1 Qual document and writes it to a file.
        /// </summary>
        void WriteToSbmlFile(BinaryModel model, string outputPath);
    }
}
