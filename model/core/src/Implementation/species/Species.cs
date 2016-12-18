
using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// The information for a tree species (its index and parameters).
    /// </summary>
    public class Species
        : Parameters, ISpecies
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

        public Species(int                index,
                       ISpeciesParameters parameters)
            : base(parameters)
        {
            this.index = index;
        }
    }
}
