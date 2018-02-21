using System;
using System.Collections.Generic;

namespace Landis.Extensions.Admin
{
    /// <summary>
    /// A command that displays a brief listing in tabular format of the
    /// extensions in the extension database.
    /// </summary>
    public class ListBriefCommand
        : ICommand
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ListBriefCommand()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            Dataset dataset = Dataset.LoadOrCreate(Dataset.DefaultPath);
            if (dataset.Count == 0)
                Console.WriteLine("No extensions are installed.");
            else {
                List<ExtensionInfo> extensions = Util.GetExtsInAlphaOrder(dataset);

                const string nameHeading = "Extension";
                int maxNameLen = nameHeading.Length;
                foreach (ExtensionInfo extension in extensions) {
                    if (extension.Name.Length > maxNameLen)
                        maxNameLen = extension.Name.Length;
                }
                string format = string.Format("{{0,{0}}}  {{1, -22}} {{2}}", -maxNameLen);
                
                Console.WriteLine(format, nameHeading, "Version", "Description");
                Console.WriteLine(format, new string('-', nameHeading.Length),"-------", "-----------");
                foreach (ExtensionInfo extension in extensions)
                    Console.WriteLine(format, extension.Name, extension.Version, extension.Description);
            }
        }
    }
}
