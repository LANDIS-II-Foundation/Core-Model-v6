using System.Collections.Generic;

namespace Landis.Core
{
    /// <summary>
    /// A read-only collection of species parameters.
    /// </summary>
    public interface ISpeciesDataset
        : IEnumerable<ISpecies>
    {
        /// <summary>
        /// The number of species in the dataset.
        /// </summary>
        int Count
        {
            get;
        }
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the parameters for a species.
        /// </summary>
        /// <param name="index">
        /// The species's index in the dataset.
        /// </param>
        ISpecies this[int index]
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the parameters for a species.
        /// </summary>
        /// <param name="name">
        /// The species's name.
        /// </param>
        ISpecies this[string name]
        {
            get;
        }
    }
}
