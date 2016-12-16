namespace Landis
{
    /// <summary>
    /// A model scenario.
    /// </summary>
    public class Scenario
    {
        private int startTime;
        private int endTime;
        private string species;
        private string ecoregions;
        private string ecoregionsMap;
        private float? cellLength;
        private ExtensionAndInitFile succession;
        private ExtensionAndInitFile[] disturbances;
        private bool disturbRandom;
        private ExtensionAndInitFile[] otherExtensions;
        private uint? seed;

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario starts from (represents time
        /// step 0).
        /// </summary>
        public int StartTime
        {
            get {
                return startTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario ends at.
        /// </summary>
        public int EndTime
        {
            get {
                return endTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with species parameters.
        /// </summary>
        public string Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with ecoregion definitions.
        /// </summary>
        public string Ecoregions
        {
            get {
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the ecoregions are.
        /// </summary>
        public string EcoregionsMap
        {
            get {
                return ecoregionsMap;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The length of a cell's side (meters).  Optional; used only if
        /// ecoregion map does not specify cell length in its metadata.
        /// </summary>
        public float? CellLength
        {
            get {
                return cellLength;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The succession extension to use for the scenario.
        /// </summary>
        public ExtensionAndInitFile Succession
        {
            get {
                return succession;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The disturbance extensions to use for the scenario.
        /// </summary>
        public ExtensionAndInitFile[] Disturbances
        {
            get {
                return disturbances;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Are disturbance run is random order?
        /// </summary>
        public bool DisturbancesRandomOrder
        {
            get {
                return disturbRandom;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The other extensions to use for the scenario (for example: output,
        /// metapopulation).
        /// </summary>
        public ExtensionAndInitFile[] OtherExtensions
        {
            get {
                return otherExtensions;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The seed for the random number generator.
        /// </summary>
        public uint? RandomNumberSeed
        {
            get {
                return seed;
            }
        }

        //---------------------------------------------------------------------

        public Scenario(int                    startTime,
                        int                    endTime,
                        string                 species,
                        string                 ecoregions,
                        string                 ecoregionsMap,
                        float?                 cellLength,
                        ExtensionAndInitFile   succession,
                        ExtensionAndInitFile[] disturbances,
                        bool                   disturbRandom,
                        ExtensionAndInitFile[] otherExtensions,
                        uint?                  seed)
        {
            this.startTime       = startTime;
            this.endTime         = endTime;
            this.species         = species;
            this.ecoregions      = ecoregions;
            this.ecoregionsMap   = ecoregionsMap;
            this.cellLength      = cellLength;
            this.succession      = succession;
            this.disturbances    = disturbances;
            this.disturbRandom   = disturbRandom;
            this.otherExtensions = otherExtensions;
            this.seed            = seed;
        }
    }
}
