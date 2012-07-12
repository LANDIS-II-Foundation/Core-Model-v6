using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
    /// <summary>
    /// Methods for plug-in information.
    /// </summary>
    public static class PlugInInfo
    {
        private static PlugIns.IDataset installedPlugIns;
        private static bool registered = false;

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a plug-in name from a text reader and returns the
        /// information for the plug-in.
        /// </summary>
        public static InputValue<PlugIns.PlugInInfo> Read(StringReader reader,
                                                          out int      index)
        {
            ReadMethod<string> strReadMethod = InputValues.GetReadMethod<string>();
            InputValue<string> name = strReadMethod(reader, out index);
            if (name.Actual.Trim(null) == "")
                throw new InputValueException(name.Actual,
                                              name.String + " is not a valid plug-in name.");
            PlugIns.PlugInInfo info = installedPlugIns[name.Actual];
            if (info == null)
                throw new InputValueException(name.Actual,
                                              "No plug-in with the name \"{0}\".",
                                              name.Actual);
            return new InputValue<PlugIns.PlugInInfo>(info, name.Actual);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers a read method for plug-in names with the input values
        /// modules in the FLEL utility library.
        /// </summary>
        /// <param name="installedPlugIns">
        /// The dataset of information about plug-ins installed on the local
        /// machine.  Used by the read method to validate plug-in names.
        /// </param>
        public static void RegisterReadMethod(PlugIns.IDataset installedPlugIns)
        {
            if (installedPlugIns == null)
                throw new System.ArgumentNullException();
            PlugInInfo.installedPlugIns = installedPlugIns;

            if (! registered) {
                Type.SetDescription<PlugIns.PlugInInfo>("plug-in name");
                InputValues.Register<PlugIns.PlugInInfo>(Read);
                registered = true;
            }
        }
    }
}
