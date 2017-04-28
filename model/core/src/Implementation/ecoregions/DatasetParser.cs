using Landis.Utilities;
using System.Collections.Generic;

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// A parser that reads a dataset of ecoregion parameter from text input.
    /// </summary>
    public class DatasetParser
        : Landis.TextParser<IEcoregionDataset>
    {
        public override string LandisDataValue
        {
            get {
                return "Ecoregions";
            }
        }

        //---------------------------------------------------------------------

        public DatasetParser()
        {
        }

        //---------------------------------------------------------------------

        protected override IEcoregionDataset Parse()
        {
            ReadLandisDataVar();

            IEditableDataset dataset = new EditableDataset();

            Dictionary <string, int> nameLineNumbers = new Dictionary<string, int>();
            Dictionary <ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();

            InputVar<string> name = new InputVar<string>("Name");
            InputVar<string> description = new InputVar<string>("Description");
            InputVar<ushort> mapCode = new InputVar<ushort>("Map Code");
            InputVar<bool> active = new InputVar<bool>("Active");

            while (! AtEndOfInput) {
                IEditableParameters parameters = new EditableParameters();
                dataset.Add(parameters);

                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(active, currentLine);
                parameters.Active = active.Value;

                int lineNumber;

                ReadValue(mapCode, currentLine);
                if (mapCodeLineNumbers.TryGetValue(mapCode.Value.Actual, out lineNumber))
                    throw new InputValueException(mapCode.Value.String,
                                                  "The map code {0} was previously used on line {1}",
                                                  mapCode.Value.Actual, lineNumber);
                else
                    mapCodeLineNumbers[mapCode.Value.Actual] = LineNumber;
                parameters.MapCode = mapCode.Value;

                ReadValue(name, currentLine);
                if (nameLineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
                    throw new InputValueException(name.Value.String,
                                                  "The name \"{0}\" was previously used on line {1}",
                                                  name.Value.Actual, lineNumber);
                else
                    nameLineNumbers[name.Value.Actual] = LineNumber;
                parameters.Name = name.Value;

                ReadValue(description, currentLine);
                parameters.Description = description.Value;

                CheckNoDataAfter("the " + description.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            return dataset.GetComplete();
        }
    }
}
