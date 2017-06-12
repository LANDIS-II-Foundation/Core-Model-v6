using Landis.Utilities;

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
        private ushort mapCode;
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
                return mapCode;
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
                          ushort mapCode,
                          bool   active)
        {
            this.name        = name;
            this.description = description;
            this.mapCode     = mapCode;
            this.active      = active;
        }

        //---------------------------------------------------------------------

        public Parameters(IEcoregionParameters parameters)
        {
            name        = parameters.Name;
            description = parameters.Description;
            mapCode     = parameters.MapCode;
            active      = parameters.Active;
        }
    }
}
