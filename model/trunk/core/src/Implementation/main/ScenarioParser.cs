using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
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
        private enum ValidPlugInTypes {
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

        public ScenarioParser(PlugIns.IDataset installedPlugIns)
        {
            PlugInInfo.RegisterReadMethod(installedPlugIns);
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

            //  Table of plug-ins
            nameLineNumbers.Clear();  // if Parse called more than once

            //  Succession plug-in must be first entry in table
            if (AtEndOfInput)
                throw NewParseException("Expected a succession plug-in");
            ReadPlugIn(scenario.Succession, ValidPlugInTypes.SuccessionOnly);

            //  0 or more disturbance plug-ins

            const string DisturbancesRandomOrder = "DisturbancesRandomOrder";
            const string RandomNumberSeed = "RandomNumberSeed";

            while (! AtEndOfInput && CurrentName != DisturbancesRandomOrder
                                  && CurrentName != RandomNumberSeed
                                  && scenario.OtherPlugIns.Count == 0) {
                EditablePlugIn plugIn = new EditablePlugIn();
                ReadPlugIn(plugIn, ValidPlugInTypes.AnyButSuccession);
                if (plugIn.Info.Actual.PlugInType.IsMemberOf("disturbance"))
                    scenario.Disturbances.Add(plugIn);
                else
                    scenario.OtherPlugIns.Add(plugIn);
            }

            //  Check for optional DisturbancesRandomOrder parameter
            InputVar<bool> randomOrder = new InputVar<bool>(DisturbancesRandomOrder);
            if (ReadOptionalVar(randomOrder))
                scenario.DisturbancesRandomOrder = randomOrder.Value;

            //  All other plug-ins besides succession and disturbances (e.g.,
            //  output, metapopulation)

            if (scenario.OtherPlugIns.Count == 0)
                nameLineNumbers.Clear();
            while (! AtEndOfInput && CurrentName != RandomNumberSeed) {
                EditablePlugIn plugIn = new EditablePlugIn();
                ReadPlugIn(plugIn, ValidPlugInTypes.AnyButSuccessionOrDisturbance);
                scenario.OtherPlugIns.Add(plugIn);
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

        private void ReadPlugIn(EditablePlugIn   plugIn,
                                ValidPlugInTypes validTypes)
        {
            StringReader currentLine = new StringReader(CurrentLine);
            plugIn.Info = ReadPlugInName(currentLine);

            string error = null;
            switch (validTypes) {
                case ValidPlugInTypes.SuccessionOnly:
                    if (! plugIn.Info.Actual.PlugInType.IsMemberOf("succession"))
                        error = "is not a succession plug-in";
                    break;

                case ValidPlugInTypes.AnyButSuccession:
                    if (plugIn.Info.Actual.PlugInType.IsMemberOf("succession"))
                        error = "is a succession plug-in";
                    break;

                case ValidPlugInTypes.AnyButSuccessionOrDisturbance:
                    if (plugIn.Info.Actual.PlugInType.IsMemberOf("succession"))
                        error = "is a succession plug-in";
                    if (plugIn.Info.Actual.PlugInType.IsMemberOf("disturbance"))
                        error = "is a disturbance plug-in";
                    break;

                default:
                    throw new ArgumentException("Unknown value for parameter \"allowedTypes\"");
            }
            if (error != null)
                throw new InputValueException(plugIn.Info.Actual.Name,
                                              "\"{0}\" {1}",
                                              plugIn.Info.Actual.Name,
                                              error);
            CheckForRepeatedName(plugIn.Info.Actual.Name);
            ReadInitFile(plugIn, currentLine);
            GetNextLine();
        }

        //---------------------------------------------------------------------

        private InputValue<PlugIns.PlugInInfo> ReadPlugInName(StringReader currentLine)
        {
            InputVar<PlugIns.PlugInInfo> plugIn = new InputVar<PlugIns.PlugInInfo>("PlugIn");
            ReadValue(plugIn, currentLine);
            return plugIn.Value;
        }

        //---------------------------------------------------------------------

        private void ReadInitFile(EditablePlugIn plugIn,
                                  StringReader   currentLine)
        {
            InputVar<string> initFile = new InputVar<string>("InitializationFile");
            ReadValue(initFile, currentLine);
            plugIn.InitFile = initFile.Value;

            CheckNoDataAfter("the " + initFile.Name + " column",
                             currentLine);
        }

        //---------------------------------------------------------------------

        private void CheckForRepeatedName(string name)
        {
            int lineNumber;
            if (nameLineNumbers.TryGetValue(name, out lineNumber))
                throw new InputValueException(name,
                                              "The plug-in \"{0}\" was previously used on line {1}",
                                              name, lineNumber);
            else
                nameLineNumbers[name] = LineNumber;
        }
    }
}
