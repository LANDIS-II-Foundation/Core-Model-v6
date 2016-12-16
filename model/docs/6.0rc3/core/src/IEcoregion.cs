namespace Landis.Core
{
    /// <summary>
    /// The information for an ecoregion (its index and parameters).
    /// </summary>
    public interface IEcoregion
        : IEcoregionParameters
    {
        /// <summary>
        /// Index of the ecoregion in the dataset of ecoregion parameters.
        /// </summary>
        int Index
        {
            get;
        }
    }
}
