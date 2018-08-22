using System;
using System.Collections.Generic;
using System.IO;
using Memoire_CLI;
using Newtonsoft.Json;
using RNESlib;
using RNESlib.Components.Interfaces;

namespace RNESCLI
{
    public class RNESCLI
    {
        private static void Main(string[] args)
        {
            // Process CLI arguments
            ProcessArguments(args, out var inputPath, out var outptuPath, out var drugsMapFilePath, out var targetPhenotypes);

            //Instanciate RNESCore and Register components
            var rnesCore = new RNESCore();
            var drugFinder = new DrugDatabase();
            drugFinder.InitializeFromFile(drugsMapFilePath);
            rnesCore.RegisterKnowledgeInstance<IDrugDatabase>(drugFinder);
            rnesCore.RegisterSkill<ISBMLManager, SBMLManager>();
            rnesCore.RegisterSkill<INetworkSimplifier, NetworkSimplifier>();
            rnesCore.RegisterSkill<INetworkExtender, NetworkExtender>();

            // Open input file in SBML
            Console.WriteLine("Reading SBML");

            // Get binary model from file
            var sbmlManager = rnesCore.ResolveKnowledge<ISBMLManager>();
            var binaryModel = sbmlManager.ReadFromSbmlFile(inputPath);

            // Call network extender
            Console.WriteLine("Improving network");
            var networkExtender = rnesCore.ResolveSkill<INetworkExtender>();
            networkExtender.SimplifyAndExtendModelWithDrugs(binaryModel, new List<string>(), targetPhenotypes.PhenotypesList);
            
            // Output file
            Console.WriteLine("Writing SBML");
            sbmlManager.WriteToSbmlFile(binaryModel, outptuPath);
        }

        private static void ProcessArguments(string[] args, out string inputPath, out string outptuPath, out string drugsMapFilePath, out TargetPhenotypes targetPhenotypes)
        {
            // Validate arguments
            if (args.Length != 4)
            {
                Console.WriteLine("wrong number of arguments");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Retrieve input file path
            inputPath = args[0];
            ValidatePath(inputPath);

            // Retrieve output file path
            outptuPath = args[1];
            var jsonSerializer = new JsonSerializer();

            // Retrieve drugs map
            drugsMapFilePath = args[2];
            ValidatePath(drugsMapFilePath);

            // Retrieve target phenotypes 
            var targetPhenotypesPath = args[3];
            ValidatePath(targetPhenotypesPath);
            using (var file = File.OpenText(targetPhenotypesPath))
            {
                targetPhenotypes = (TargetPhenotypes)jsonSerializer.Deserialize(file, typeof(TargetPhenotypes));
            }
        }

        private static void ValidatePath(string pathToCheck)
        {
            if (!File.Exists(pathToCheck))
            {
                Console.WriteLine("[Error] {0} : No such file.", pathToCheck);
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}