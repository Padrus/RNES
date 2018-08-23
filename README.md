# Regulatory Networks Expert System - RNES
Welcome to the RNES project.

## RNESlib
RNESlib is a .Net Core library that provides the ability to deploy an expert system to work with gene regulatory networks.

### Functionalities
Current version provides support for the following functionalities:
- Reading and writing of SBML Qual models.
- Simplifications of SBML Qual regulatory networks.
- Extention of SBML Qual regulatory networks with relevant drugs based on target nodes.

### Dependencies
RNESlib is build using .Net Core 2.1. You can download the latest version of .Net Core [here](https://www.microsoft.com/net/download)
RNESlib relies libSBML 5.17 for the support of SBML models. libSBML 5.17 installer can be downloaded [here](https://sourceforge.net/projects/sbml/files/libsbml/5.17.0/stable/Windows/64-bit/)

### Platforms support
Due to its dependency to libSBML, current version of RNESlib only works on x64 Windows operating systems.
To run it on Linux or macOS, you need to update the RNESlib project dependencies to the right version of libsbmlcs.dll and libsbmlcsP.dll. See https://sourceforge.net/projects/sbml/files/libsbml/5.17.0/ to find the right version of libSBML to suit your needs.

### Usage
Sample code using RNESlib can be found [here](https://github.com/TonusV/RNES/blob/master/RNESCLI/RNESCLI.cs)

## RNESCLI
RNESCLI is a command line applications that can be used to run RNESlib currently supported functionalities.

## Usage
Based on an input gene regulatory network, a list of available durgs and a list of target nodes, RNESCLI produces a simplified an extended SBML Qual model with the following characteristics:
- The output model is enriched with drug nodes that can induce the desired effect on at least one of the target nodes.
- The output model is simplified as much as possible without introducing contradictory edges.

## Syntax
RNESCLI must be called with four arguments:
- The input file path: a relative or absolute path to a valid SML Qual file.
- The output file path: a relative or absolute path to the output file that will be created.
- The available drugs map file path: a relative or absolute path to a json file containing a list of available drugs and the efect they have on a target node Id.
- The target nodes file path: a relative or absolute path to a json file containing a list of nodes that must be targetted and the desired effect (acivation or inhibition).

RNESCLI can be called from any Windows Command window, PowerShell window and Linux or macOS terminal using the following command line:

`dotnet RNESCLI.dll <input_file> <output_file> <drugs_map_file> <target_nodes_file>`

## Samples
For easier undertsanding, RNESCLI is provided with sample files located in the [TestResources subfolder](https://github.com/TonusV/RNES/tree/master/RNESCLI/TestResources).
To run RNESCLI on these files, run the following commands:

`dotnet RNESCLI.dll TestResources/VEGF_signaling_Clean.sbml VEGF_signaling_Clean.Simplified.sbml TestResources/VEGFDrugsMap.json TestResources/VEGFTargetPhenotypes.json`

`dotnet RNESCLI.dll Resources/MAPK_Cancer_Fate_Network.sbml_Clean.sbml MAPK_Cancer_Fate_Network.sbml_Clean_Simplified.sbml TestResources/MAPKDrugsMap.json TestResources/MAPKTargetPhenotypes.json`
