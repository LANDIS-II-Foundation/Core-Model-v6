using Landis.Utilities;
using Landis.Core;

namespace Landis
{
    /// <summary>
    /// Editable pair of an extension and its initialization file.
    /// </summary>
    public class EditableExtension
        : IEditable<ExtensionAndInitFile>
    {
        private InputValue<ExtensionInfo> info;
        private InputValue<string> initFile;

        //---------------------------------------------------------------------

        /// <summary>
        /// Information about the extension.
        /// </summary>
        public InputValue<ExtensionInfo> Info
        {
            get {
                return info;
            }

            set {
                info = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The path to the data file to initialize the extension with.
        /// </summary>
        public InputValue<string> InitFile
        {
            get {
                return initFile;
            }

            set {
                initFile = value;
            }
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                return (info != null) && (initFile != null);
            }
        }

        //---------------------------------------------------------------------

        public EditableExtension()
        {
            this.info = null;
            this.initFile = null;
        }

        //---------------------------------------------------------------------

        public ExtensionAndInitFile GetComplete()
        {
            if (IsComplete)
                return new ExtensionAndInitFile(info.Actual, initFile.Actual);
            else
                return null;
        }
    }
}
