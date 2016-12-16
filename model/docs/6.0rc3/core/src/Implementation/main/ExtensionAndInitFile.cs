using Landis.Core;

namespace Landis
{
    /// <summary>
    /// Information about an extension and its initialization file.
    /// </summary>
    public class ExtensionAndInitFile
    {
        private ExtensionInfo info;
        private string initFile;

        //---------------------------------------------------------------------

        /// <summary>
        /// Information about the extension.
        /// </summary>
        public ExtensionInfo Info
        {
            get {
                return info;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The path to the extension's initialization file.
        /// </summary>
        public string InitFile
        {
            get {
                return initFile;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="extensionInfo">
        /// Information about the extension.
        /// </param>
        /// <param name="initFile">
        /// The path to the extension's initialization file.
        /// </param>
        public ExtensionAndInitFile(ExtensionInfo extensionInfo,
                                    string        initFile)
        {
            this.info = extensionInfo;
            this.initFile = initFile;
        }
    }
}
