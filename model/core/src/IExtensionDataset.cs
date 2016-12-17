namespace Landis.Core
{
    /// <summary>
    /// A collection of information about installed extensions.
    /// </summary>
    public interface IExtensionDataset
    {
        /// <summary>
        /// The number of extensions in the dataset.
        /// </summary>
        int Count
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the information for an extension.
        /// </summary>
        /// <returns>
        /// null if there is no extension installed with the specified name.
        /// </returns>
        ExtensionInfo this[string name]
        {
            get;
        }
    }
}
