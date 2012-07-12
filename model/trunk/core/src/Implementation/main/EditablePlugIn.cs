using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
    /// <summary>
    /// Editable pair of a plug-in and its initialization file.
    /// </summary>
    public class EditablePlugIn
        : IEditable<PlugInAndInitFile>
    {
        private InputValue<PlugIns.PlugInInfo> info;
        private InputValue<string> initFile;

        //---------------------------------------------------------------------

        /// <summary>
        /// Information about the plug-in.
        /// </summary>
        public InputValue<PlugIns.PlugInInfo> Info
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
        /// The path to the data file to initialize the plug-in with.
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

        public EditablePlugIn()
        {
            this.info = null;
            this.initFile = null;
        }

        //---------------------------------------------------------------------

        public PlugInAndInitFile GetComplete()
        {
            if (IsComplete)
                return new PlugInAndInitFile(info.Actual, initFile.Actual);
            else
                return null;
        }
    }
}
