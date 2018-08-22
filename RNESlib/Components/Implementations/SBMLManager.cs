using System;
using System.Linq;
using libsbmlcs;
using Memoire_CLI;

namespace RNESlib.Components.Interfaces
{
    public class SBMLManager : ISBMLManager
    {
        /// <summary>
        ///     Initializes and returns a new binary model based on an SBML file.
        /// </summary>
        /// <param name="filePath">The SBML file path</param>
        /// <returns>A new <see cref="BinaryModel" /> instance initialized from <paramref name="filePath" /></returns>
        public BinaryModel ReadFromSbmlFile(string filePath)
        {
            var reader = new SBMLReader();
            var sbmlDoc = reader.readSBML(filePath);
            var sbmlModel = sbmlDoc.getModel();
            Console.WriteLine("Initializing boolean model");
            var binaryModel = new BinaryModel();
            InitFromSbmlQualModel(binaryModel, sbmlModel);
            return binaryModel;
        }

        /// <summary>
        ///     Creates a new binary model based onan SBML Qual model.
        /// </summary>
        public void InitFromSbmlQualModel(BinaryModel model, Model sbmlModel)
        {
            PopulateNodes(model, sbmlModel);
            PopulateEdges(model, sbmlModel);
        }

        /// <summary>
        ///     Converts the current binary model to an SBML 3.1 Qual document and writes it to a file.
        /// </summary>
        public void WriteToSbmlFile(BinaryModel model, string outputPath)
        {
            var outputSbmlDocument = ToSbmlQualDocument(model);
            var writer = new SBMLWriter();
            writer.writeSBML(outputSbmlDocument, outputPath);
        }

        /// <summary>
        ///     Populates the edges list based on the SBML Qual model content.
        /// </summary>
        /// <param name="model">The target binary model to populate</param>
        /// <param name="sbmlModel">The input SBML Qual model</param>
        private void PopulateEdges(BinaryModel model, Model sbmlModel)
        {
            var modelQualPlugin = (QualModelPlugin) sbmlModel.getPlugin("qual");
            var numTransitions = modelQualPlugin.getNumTransitions();

            for (var i = 0; i < numTransitions; i++)
            {
                var transition = modelQualPlugin.getTransition(i);
                var numInputs = transition.getNumInputs();
                var numOutputs = transition.getNumOutputs();

                for (var j = 0; j < numInputs; j++)
                {
                    var input = transition.getInput(j);
                    for (var k = 0; k < numOutputs; k++)
                    {
                        var output = transition.getOutput(k);
                        var edge = new BinaryEdge(model, transition.getId(),
                            input.getSign() == libsbml.INPUT_SIGN_NEGATIVE ? EdgeType.Negative : EdgeType.Positive)
                        {
                            Input = model.Nodes.FirstOrDefault(n => n.Id.Equals(input.getQualitativeSpecies())),
                            Output = model.Nodes.FirstOrDefault(n => n.Id.Equals(output.getQualitativeSpecies()))
                        };
                        model.Edges.Add(edge);
                    }
                }
            }
        }

        /// <summary>
        ///     Populates the model nodes list based on the SBML Qual model content.
        /// </summary>
        /// <param name="model">The target binary model to populate</param>
        /// <param name="sbmlModel">The input SBML Qual model</param>
        private void PopulateNodes(BinaryModel model, Model sbmlModel)
        {
            var modelQualPlugin = (QualModelPlugin) sbmlModel.getPlugin("qual");
            var numQualitativeSpecies = modelQualPlugin.getNumQualitativeSpecies();
            for (var i = 0; i < numQualitativeSpecies; i++)
            {
                var qualitativeSpecies = modelQualPlugin.getQualitativeSpecies(i);
                var node = new BinaryNode(model, qualitativeSpecies.getId())
                {
                    Name = qualitativeSpecies.isSetName() ? qualitativeSpecies.getName() : string.Empty
                };
                model.Nodes.Add(node);
            }
        }

        /// <summary>
        ///     Converts the current binary model to an SBML 3.1 Qual document.
        /// </summary>
        /// <returns>A new SBML Qual document based on curent binary model</returns>
        public SBMLDocument ToSbmlQualDocument(BinaryModel model)
        {
            var sbmlns = new SBMLNamespaces(3, 1, "qual", 1);
            var document = new SBMLDocument(sbmlns);
            document.setPackageRequired("qual", true);
            var sbmlModel = document.createModel();
            var qualModel = (QualModelPlugin) sbmlModel.getPlugin("qual");

            var compartment = sbmlModel.createCompartment();
            compartment.setConstant(false);
            compartment.setId("Default");
            compartment.setName("Default");

            foreach (var binaryNode in model.Nodes)
            {
                var qualitativeSpecies = qualModel.createQualitativeSpecies();
                qualitativeSpecies.setConstant(false);
                qualitativeSpecies.setId(binaryNode.Id);
                qualitativeSpecies.setName(binaryNode.Name);
                qualitativeSpecies.setCompartment("Default");
            }

            foreach (var binaryEdge in model.Edges)
            {
                var transition = qualModel.createTransition();

                var input = transition.createInput();
                input.setQualitativeSpecies(binaryEdge.Input.Id);
                input.setTransitionEffect(libsbml.INPUT_TRANSITION_EFFECT_NONE);
                input.setSign(binaryEdge.IsPositive() ? libsbml.INPUT_SIGN_POSITIVE : libsbml.INPUT_SIGN_NEGATIVE);

                var output = transition.createOutput();
                output.setQualitativeSpecies(binaryEdge.Output.Id);
                output.setTransitionEffect(libsbml.OUTPUT_TRANSITION_EFFECT_PRODUCTION);
                output.setOutputLevel(1);

                var defaultTrem = transition.createDefaultTerm();
                defaultTrem.setResultLevel(1);
            }

            return document;
        }
    }
}