using System.Collections.Generic;

namespace Landis.Core
{
    /// <summary>
    /// A read-only collection of ecoregion parameters.
    /// </summary>
    public interface IEcoregionDataset
        : IEnumerable<IEcoregion>
    {
        /// <summary>
        /// The number of ecoregions in the dataset.
        /// </summary>
        int Count
        {
            get;
        }
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the parameters for a ecoregion.
        /// </summary>
        /// <param name="index">
        /// The ecoregion's index in the dataset.
        /// </param>
        IEcoregion this[int index]
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the parameters for a ecoregion.
        /// </summary>
        /// <param name="name">
        /// The ecoregion's name.
        /// </param>
        IEcoregion this[string name]
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds an ecoregion's parameters by its map code.
        /// </summary>
        /// <param name="mapCode">
        /// The ecoregion's map code.
        /// </param>
        IEcoregion Find(ushort mapCode);
    }
}
