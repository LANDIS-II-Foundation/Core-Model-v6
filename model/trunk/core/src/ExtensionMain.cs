//  Author: Jimm Domingo, UW-Madison, FLEL

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Core
{
    /// <summary>
    /// Base class for plug-ins.
    /// </summary>
    public abstract class ExtensionMain
    {
        private string name;
        private ExtensionType type;
        private int timestep;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected ExtensionMain(string        name,
                                ExtensionType type)
        {
            this.name = name;
            this.type = type;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The plug-in's name.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The plug-in's type.
        /// </summary>
        public ExtensionType PlugInType
        {
            get {
                return type;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The plug-in's timestep (years).
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }

            protected set {
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the plug-in with a data file.
        /// </summary>
        /// <param name="dataFile">
        /// Path to the file with initialization data.
        /// </param>
        /// <param name="modelCore">
        /// The model's core framework.
        /// </param>
        public abstract void Initialize();

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads parameters for the plug-in with a data file.
        /// </summary>
        /// <param name="dataFile">
        /// Path to the file with initialization data.
        /// </param>
        /// <param name="modelCore">
        /// The model's core framework.
        /// </param>
        public abstract void LoadParameters(string dataFile, ICore modelCore);

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs the plug-in for the current timestep.
        /// </summary>
        public abstract void Run();

        //---------------------------------------------------------------------

        public void InitializePhase2()
        {
            // No-op !!!
        }

        //---------------------------------------------------------------------

        public void CleanUp()
        {
            // No-op !!!
        }

        //---------------------------------------------------------------------
    }
}
