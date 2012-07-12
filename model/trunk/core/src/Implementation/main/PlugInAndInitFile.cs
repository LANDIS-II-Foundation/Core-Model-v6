namespace Landis
{
    /// <summary>
    /// Information about a plug-in and its initialization file.
    /// </summary>
    public class PlugInAndInitFile
    {
        private PlugIns.PlugInInfo info;
        private string initFile;

        //---------------------------------------------------------------------

        /// <summary>
        /// Information about the plug-in.
        /// </summary>
        public PlugIns.PlugInInfo Info
        {
            get {
                return info;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The path to the plug-in's initialization file.
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
        /// <param name="plugInInfo">
        /// Information about the plug-in.
        /// </param>
        /// <param name="initFile">
        /// The path to the plug-in's initialization file.
        /// </param>
        public PlugInAndInitFile(PlugIns.PlugInInfo plugInInfo,
                                 string             initFile)
        {
            this.info = plugInInfo;
            this.initFile = initFile;
        }
    }
}
