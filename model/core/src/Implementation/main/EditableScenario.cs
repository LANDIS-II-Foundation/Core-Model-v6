using Landis.Utilities;
using Landis.Core;

namespace Landis
{
    /// <summary>
    /// An editable model scenario.
    /// </summary>
    public class EditableScenario
        : IEditable<Scenario>
    {
        private InputValue<int> startTime;
        private InputValue<int> endTime;
        private InputValue<string> species;
        private InputValue<string> ecoregions;
        private InputValue<string> ecoregionsMap;
        private InputValue<float> cellLength;
        private EditableExtension succession;
        private EditableExtensionList disturbances;
        private EditableExtensionList otherExtensions;
        private InputValue<bool> disturbRandom;
        private InputValue<uint> seed;

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario starts from (represents time
        /// step 0).
        /// </summary>
        public InputValue<int> StartTime
        {
            get {
                return startTime;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0");
                    if (endTime != null && value.Actual > endTime.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be < or = EndTime ({0})",
                                                      endTime.Actual);
                }
                startTime = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario ends at.
        /// </summary>
        public InputValue<int> EndTime
        {
            get {
                return endTime;
            }

            set {
                if (value != null) {
                    if (startTime != null) {
                        if (value.Actual < startTime.Actual)
                            throw new InputValueException(value.String,
                                                          "Value must be = or > StartTime ({0})",
                                                          startTime.Actual);
                    }
                    else {
                        if (value.Actual < 0)
                            throw new InputValueException(value.String,
                                                          "Value must be = or > 0");
                    }
                }
                endTime = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with species parameters.
        /// </summary>
        public InputValue<string> Species
        {
            get {
                return species;
            }

            set {
                if (value != null) {
                    ValidatePath(value.Actual);
                }
                species = value;
            }
        }

        //---------------------------------------------------------------------

        private void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InputValueException();
            if (path.Trim(null).Length == 0)
                throw new InputValueException(path,
                                              "\"{0}\" is not a valid path.",
                                              path);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with ecoregion definitions.
        /// </summary>
        public InputValue<string> Ecoregions
        {
            get {
                return ecoregions;
            }

            set {
                if (value != null) {
                    ValidatePath(value.Actual);
                }
                ecoregions = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the ecoregions are.
        /// </summary>
        public InputValue<string> EcoregionsMap
        {
            get {
                return ecoregionsMap;
            }

            set {
                if (value != null) {
                    ValidatePath(value.Actual);
                }
                ecoregionsMap = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The length of a cell's side (meters).  Optional; used only if
        /// ecoregion map does not specify cell length in its metadata.
        /// </summary>
        public InputValue<float> CellLength
        {
            get {
                return cellLength;
            }
            set {
                if (value != null) {
                    if (value.Actual <= 0.0)
                        throw new InputValueException(value.String,
                                                      "Value must be > 0");
                }
                cellLength = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The succession extension to use for the scenario.
        /// </summary>
        public EditableExtension Succession
        {
            get {
                return succession;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The disturbance extensions to use for the scenario.
        /// </summary>
        public EditableExtensionList Disturbances
        {
            get {
                return disturbances;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Are disturbances run is random order?
        /// </summary>
        public InputValue<bool> DisturbancesRandomOrder
        {
            get {
                return disturbRandom;
            }

            set {
                disturbRandom = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The other extensions to use for the scenario (for example: output,
        /// metapopulation).
        /// </summary>
        public EditableExtensionList OtherExtensions
        {
            get {
                return otherExtensions;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The seed for the random number generator.
        /// </summary>
        public InputValue<uint> RandomNumberSeed
        {
            get {
                return seed;
            }

            set {
                if (value != null)
                    if (value.Actual == 0)
                        throw new InputValueException(value.String, "Value must be > 0.");
                seed = value;
            }
        }

        //---------------------------------------------------------------------

        public EditableScenario()
        {
            this.succession = new EditableExtension();
            this.disturbances = new EditableExtensionList();
            this.otherExtensions = new EditableExtensionList();
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                // DisturbanceRandomOrder isn't checked because it has a
                // default value; see GetComplete() method.  Also, CellLength
                // isn't checked because it's optional.
                foreach (object inputValue in new object[]{ startTime,
                                                            endTime,
                                                            species,
                                                            ecoregions,
                                                            ecoregionsMap
                                                          })
                    if (inputValue == null)
                        return false;
                if (succession.IsComplete && disturbances.IsComplete
                                          && otherExtensions.IsComplete)
                    return true;
                return false;
            }
        }

        //---------------------------------------------------------------------

        public Scenario GetComplete()
        {
            if (IsComplete) {
                return new Scenario(startTime.Actual,
                                    endTime.Actual,
                                    species.Actual,
                                    ecoregions.Actual,
                                    ecoregionsMap.Actual,
                                    cellLength == null ? (float?) null
                                                       : cellLength.Actual,
                                    succession.GetComplete(),
                                    disturbances.GetComplete(),
                                    disturbRandom == null ? false
                                                          : disturbRandom.Actual,
                                    otherExtensions.GetComplete(),
                                    seed == null ? (uint?) null
                                                 : seed.Actual);
            }
            else
                return null;
        }
    }
}
