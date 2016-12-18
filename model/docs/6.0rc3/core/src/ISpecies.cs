namespace Landis.Core
{
    /// <summary>
    /// The information for a tree species (its index and parameters).
    /// </summary>
    public interface ISpecies
        : ISpeciesParameters
    {
        /// <summary>
        /// Index of the species in the dataset of species parameters.
        /// </summary>
        int Index
        {
            get;
        }
    }
}
