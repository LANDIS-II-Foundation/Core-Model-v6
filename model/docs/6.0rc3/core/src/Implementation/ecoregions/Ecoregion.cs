
using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// The information for an ecoregion (its index and parameters).
    /// </summary>
    public class Ecoregion
        : Parameters, IEcoregion
    {
        private int index;

        //---------------------------------------------------------------------

        public int Index
        {
            get {
                return index;
            }
        }

        //---------------------------------------------------------------------

        public Ecoregion(int                  index,
                         IEcoregionParameters parameters)
            : base(parameters)
        {
            this.index = index;
        }

        public override string ToString()
        {
            return this.Name;               
        }
    }
}
