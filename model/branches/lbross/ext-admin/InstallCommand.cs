using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extensions.Admin
{
    /// <summary>
    /// A command that installs an extension's files and adds an entry for
    /// it in the extension dataset.
    /// </summary>
    public class InstallCommand
        : ICommand
    {
        private static string installDir = Application.Directory;

        private string extensionInfoPath;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public InstallCommand(string extensionInfoPath)
        {
            this.extensionInfoPath = extensionInfoPath;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
#if ENABLE_OLD_CODE
            //  TODO: This code is from the old AddCommand, so it needs to be
            //  updated eventually.

            Dataset dataset = Dataset.LoadOrCreate(Dataset.DefaultPath);
            EditableExtensionInfo.Dataset = dataset;
            ExtensionParser parser = new ExtensionParser();
            ExtensionInfo extension = Data.Load<ExtensionInfo>(extensionInfoPath, parser);

            List<string> missingLibs = new List<string>();
            foreach (string library in extension.ReferencedAssemblies) {
                if (! dataset.ReferencedByEntries(library))
                    missingLibs.Add(library);
            }

            List<string> libsToBeInstalled = new List<string>();
            foreach (string libPath in extension.LibraryPaths) {
                libsToBeInstalled.Add(Path.GetFileNameWithoutExtension(libPath));
            }

            foreach (string libToBeInstalled in libsToBeInstalled) {
                missingLibs.Remove(libToBeInstalled);
            }
            if (missingLibs.Count > 0) {
                MultiLineText message = new MultiLineText();
                message.Add("Error: The extension requires the following libraries which are not");
                message.Add("       currently installed and are not listed in the extension info file:");
                foreach (string lib in missingLibs)
                    message.Add("         " + lib);
                throw new MultiLineException(message);
            }

            Console.WriteLine("Installation directory: {0}", installDir);
            Console.WriteLine("Copying files to installation directory ...");
            CopyFileToInstallDir(extension.AssemblyPath);
            foreach (string libPath in extension.LibraryPaths) {
                CopyFileToInstallDir(libPath);
            }

            dataset.Add(extension);
            dataset.Save();
            Console.WriteLine("Extension {0} installed", extension.Name);
#endif
        }

        //---------------------------------------------------------------------

        private void CopyFileToInstallDir(string path)
        {
            string libFileName = Path.GetFileName(path);
            string targetFile = Path.Combine(installDir, libFileName);
            bool overwritten = File.Exists(targetFile);
            File.Copy(path, targetFile, true);
            if (overwritten)
                Console.WriteLine("  {0}  (replaced existing file)", libFileName);
            else
                Console.WriteLine("  {0}", libFileName);
        }
    }
}
