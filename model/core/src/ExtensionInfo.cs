namespace Landis.Core
{
    /// <summary>
    /// Information about an extension.
    /// </summary>
    public class ExtensionInfo
        : Landis.Utilities.PlugIns.Info
    {
        ExtensionType type;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name">The extension's name</param>
        /// <param name="type">The extension's type</param>
        /// <param name="mainClass">The AssemblyQualifiedName of the
        /// extension's main class (dervied from ExtensionMain).</param>
        public ExtensionInfo(string        name,
                             ExtensionType type,
                             string        mainClass)
            : base(name, typeof(ExtensionMain), mainClass)
        {
            this.type = type;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The extension's type.
        /// </summary>
        public ExtensionType Type
        {
            get {
                return type;
            }
        }
    }
}
