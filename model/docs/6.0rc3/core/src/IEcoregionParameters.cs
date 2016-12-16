namespace Landis.Core
{
    /// <summary>
    /// The parameters for an ecoregion that can be edited.
    /// </summary>
    public interface IEcoregionParameters
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Description, e.g., "wetlands", "water", or "pine barrens".
        /// </summary>
        string Description
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Code that identifies the ecoregion on a map.
        /// </summary>
        ushort MapCode
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Indicates whether the sites in the ecoregion are active in a
        /// scenario.
        /// </summary>
        bool Active
        {
            get;
        }
    }
}
