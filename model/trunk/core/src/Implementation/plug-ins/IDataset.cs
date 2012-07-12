namespace Landis.PlugIns
{
    /// <summary>
    /// A collection of information about installed plug-ins.
    /// </summary>
    public interface IDataset
    {
        /// <summary>
        /// The number of plug-ins in the dataset.
        /// </summary>
        int Count
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the information for a plug-in.
        /// </summary>
        /// <returns>
        /// null if there is no plug-in installed with the specified name.
        /// </returns>
        PlugInInfo this[string name]
        {
            get;
        }
    }
}
