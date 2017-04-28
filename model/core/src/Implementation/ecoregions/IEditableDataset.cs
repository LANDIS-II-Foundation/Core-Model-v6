using Landis.Utilities;
using System.Collections.Generic;

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// Editable dataset of ecoregion parameters.
    /// </summary>
    public interface IEditableDataset
        : IEditable<IEcoregionDataset>, IList<IEditableParameters>
    {
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the parameters for a ecoregion.
        /// </summary>
        /// <param name="name">
        /// The ecoregion's name.
        /// </param>
        /// <remarks>
        /// If there are no parameters for the specified name, then the get
        /// accessor returns null.  If the value for the set accessor is null,
        /// then the parameters for the specified name are removed from the
        /// dataset (it is not an error if the name is not in the dataset).
        /// </remarks>
        IEditableParameters this[string name]
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds an ecoregion's parameters by its map code.
        /// </summary>
        /// <param name="mapCode">
        /// The ecoregion's map code.
        /// </param>
        IEditableParameters Find(ushort mapCode);
    }
}
