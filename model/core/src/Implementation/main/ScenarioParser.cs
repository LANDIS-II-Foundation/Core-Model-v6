using Landis.Utilities;
using Landis.Core;
using System;
using System.Collections.Generic;

namespace Landis
{
    /// <summary>
    /// A parser that reads a model scenario from text input.
    /// </summary>
    public class ScenarioParser
        : Landis.TextParser<Scenario>
    {
        private enum ValidExtensionTypes {
            SuccessionOnly,
            AnyButSuccession,
            AnyButSuccessionOrDisturbance
        }

        //---------------------------------------------------------------------

        private Dictionary<string, int> nameLineNumbers;

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return "Scenario";
            }
        }

        //---------------------------------------------------------------------

        public ScenarioParser(IExtensionDataset installedExtensions)
        {
            ExtensionInfoIO.RegisterReadMethod(installedExtensions);
            nameLineNumbers = new Dictionary<string, int>();
        }

        //---------------------------------------------------------------------

        protected override Scenario Parse()
        {
            ReadLandisDataVar();

            EditableScenario scenario = new EditableScenario();

            scenario.StartTime = new InputValue<int>(0, "0");

            InputVar<int> duration = new InputVar<int>("Duration");
            ReadVar(duration);
            scenario.EndTime = duration.Value;

            InputVar<string> species = new InputVar<string>("Species");
            ReadVar(species);
            scenario.Species = species.Value;

            InputVar<string> ecoregions = new InputVar<string>("Ecoregions");
            ReadVar(ecoregions);
            scenario.Ecoregions = ecoregions.Value;

            InputVar<string> ecoregionsMap = new InputVar<string>("EcoregionsMap");
            ReadVar(ecoregionsMap);
            scenario.EcoregionsMap = ecoregionsMap.Value;

            InputVar<float> cellLength = new InputVar<float>("CellLength");
            if (ReadOptionalVar(cellLength)) {
                scenario.CellLength = cellLength.Value;
            }

            //  Table of extensions
            nameLineNumbers.Clear();  // if Parse called more than once

            //  Succession extension must be first entry in table
            if (AtEndOfInput)
                throw NewParseException("Expected a succession extension");
            ReadExtension(scenario.Succession, ValidExtensionTypes.SuccessionOnly);

            //  0 or more disturbance extensions

            const string DisturbancesRandomOrder = "DisturbancesRandomOrder";
            const string RandomNumberSeed = "RandomNumberSeed";

            while (! AtEndOfInput && CurrentName != DisturbancesRandomOrder
                                  && CurrentName != RandomNumberSeed
                                  && scenario.OtherExtensions.Count == 0) {
                EditableExtension Extension = new EditableExtension();
                ReadExtension(Extension, ValidExtensionTypes.AnyButSuccession);
                if (Extension.Info.Actual.Type.IsMemberOf("disturbance"))
                    scenario.Disturbances.Add(Extension);
                else
                    scenario.OtherExtensions.Add(Extension);
            }

            //  Check for optional DisturbancesRandomOrder parameter
            InputVar<bool> randomOrder = new InputVar<bool>(DisturbancesRandomOrder);
            if (ReadOptionalVar(randomOrder))
                scenario.DisturbancesRandomOrder = randomOrder.Value;

            //  All other extensions besides succession and disturbances (e.g.,
            //  output, metapopulation)

            if (scenario.OtherExtensions.Count == 0)
                nameLineNumbers.Clear();
            while (! AtEndOfInput && CurrentName != RandomNumberSeed) {
                EditableExtension Extension = new EditableExtension();
                ReadExtension(Extension, ValidExtensionTypes.AnyButSuccessionOrDisturbance);
                scenario.OtherExtensions.Add(Extension);
            }

            //    Either at end of file or we've encountered the optional
            //    RandomNumberSeed parameter.
            if (! AtEndOfInput) {
                InputVar<uint> seed = new InputVar<uint>(RandomNumberSeed);
                ReadVar(seed);
                scenario.RandomNumberSeed = seed.Value;
                CheckNoDataAfter(string.Format("the {0} parameter", RandomNumberSeed));
            }

            return scenario.GetComplete();
        }

        //---------------------------------------------------------------------

        private void ReadExtension(EditableExtension   Extension,
                                ValidExtensionTypes validTypes)
        {
            StringReader currentLine = new StringReader(CurrentLine);
            Extension.Info = ReadExtensionName(currentLine);

            string error = null;
            switch (validTypes) {
                case ValidExtensionTypes.SuccessionOnly:
                    if (! Extension.Info.Actual.Type.IsMemberOf("succession"))
                        error = "is not a succession extension";
                    break;

                case ValidExtensionTypes.AnyButSuccession:
                    if (Extension.Info.Actual.Type.IsMemberOf("succession"))
                        error = "is a succession extension";
                    break;

                case ValidExtensionTypes.AnyButSuccessionOrDisturbance:
                    if (Extension.Info.Actual.Type.IsMemberOf("succession"))
                        error = "is a succession extension";
                    if (Extension.Info.Actual.Type.IsMemberOf("disturbance"))
                        error = "is a disturbance extension";
                    break;

                default:
                    throw new ArgumentException("Unknown value for parameter \"allowedTypes\"");
            }
            if (error != null)
                throw new InputValueException(Extension.Info.Actual.Name,
                                              "\"{0}\" {1}",
                                              Extension.Info.Actual.Name,
                                              error);
            CheckForRepeatedName(Extension.Info.Actual.Name);
            ReadInitFile(Extension, currentLine);
            GetNextLine();
        }

        //---------------------------------------------------------------------

        private InputValue<ExtensionInfo> ReadExtensionName(StringReader currentLine)
        {
            InputVar<ExtensionInfo> Extension = new InputVar<ExtensionInfo>("Extension");
            ReadValue(Extension, currentLine);
            return Extension.Value;
        }

        //---------------------------------------------------------------------

        private void ReadInitFile(EditableExtension Extension,
                                  StringReader   currentLine)
        {
            InputVar<string> initFile = new InputVar<string>("InitializationFile");
            ReadValue(initFile, currentLine);
            Extension.InitFile = initFile.Value;

            CheckNoDataAfter("the " + initFile.Name + " column",
                             currentLine);
        }

        //---------------------------------------------------------------------

        private void CheckForRepeatedName(string name)
        {
            int lineNumber;
            if (nameLineNumbers.TryGetValue(name, out lineNumber))
                throw new InputValueException(name,
                                              "The extension \"{0}\" was previously used on line {1}",
                                              name, lineNumber);
            else
                nameLineNumbers[name] = LineNumber;
        }
    }
}
