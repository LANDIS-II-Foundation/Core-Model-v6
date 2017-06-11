using Landis.Utilities;
using Landis.Core;

namespace Landis
{
    /// <summary>
    /// Methods for reading extension information.
    /// </summary>
    public static class ExtensionInfoIO
    {
        private static IExtensionDataset installedExtensions;
        private static bool registered = false;

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads an extension name from a text reader and returns the
        /// information for the extension.
        /// </summary>
        public static InputValue<ExtensionInfo> Read(StringReader reader,
                                                     out int      index)
        {
            ReadMethod<string> strReadMethod = InputValues.GetReadMethod<string>();
            InputValue<string> name = strReadMethod(reader, out index);
            if (name.Actual.Trim(null) == "")
                throw new InputValueException(name.Actual,
                                              name.String + " is not a valid extension name.");
            ExtensionInfo info = installedExtensions[name.Actual];
            if (info == null)
                throw new InputValueException(name.Actual,
                                              "No extension with the name \"{0}\".",
                                              name.Actual);
            return new InputValue<ExtensionInfo>(info, name.Actual);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers a read method for extension names with the input values
        /// modules in the FLEL utility library.
        /// </summary>
        /// <param name="installedExtensions">
        /// The dataset of information about extensions installed on the local
        /// machine.  Used by the read method to validate extension names.
        /// </param>
        public static void RegisterReadMethod(IExtensionDataset installedExtensions)
        {
            if (installedExtensions == null)
                throw new System.ArgumentNullException();
            ExtensionInfoIO.installedExtensions = installedExtensions;

            if (! registered) {
                Type.SetDescription<ExtensionInfo>("extension name");
                InputValues.Register<ExtensionInfo>(Read);
                registered = true;
            }
        }
    }
}
