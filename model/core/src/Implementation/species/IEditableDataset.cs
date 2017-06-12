using Landis.Utilities;
using System.Collections.Generic;

using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// Editable dataset of species parameters.
    /// </summary>
    public interface IEditableDataset
        : IEditable<ISpeciesDataset>, IList<IEditableParameters>
    {
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the parameters for a species.
        /// </summary>
        /// <param name="name">
        /// The species's name.
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
    }
}
