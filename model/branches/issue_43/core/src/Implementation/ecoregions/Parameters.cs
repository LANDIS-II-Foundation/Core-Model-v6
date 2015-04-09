using Edu.Wisc.Forest.Flel.Util;

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// The parameters for a single ecoregion.
    /// </summary>
    public class Parameters
        : IEcoregionParameters
    {
        private string name;
        private string description;
        private uint largeMapCode;
        private bool active;

        //---------------------------------------------------------------------

        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        public string Description
        {
            get {
                return description;
            }
        }

        //---------------------------------------------------------------------

        public ushort MapCode
        {
            get {
                return (ushort) largeMapCode;
            }
        }

        //---------------------------------------------------------------------

        public uint LargeMapCode
        {
            get
            {
                return largeMapCode;
            }
        }


        //---------------------------------------------------------------------

        public bool Active
        {
            get {
                return active;
            }
        }

        //---------------------------------------------------------------------

        public Parameters(string name,
                          string description,
                          uint mapCode,
                          bool   active)
        {
            this.name        = name;
            this.description = description;
            this.largeMapCode   = mapCode;
            this.active      = active;
        }

        //---------------------------------------------------------------------

        public Parameters(IEcoregionParameters parameters)
        {
            name        = parameters.Name;
            description = parameters.Description;
            largeMapCode = parameters.LargeMapCode;
            active      = parameters.Active;
        }
    }
}
