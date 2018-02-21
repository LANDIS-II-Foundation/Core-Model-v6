using System;
using System.Collections.Generic;

namespace Landis.Extensions.Admin
{
    /// <summary>
    /// A command that lists the extensions in the extension database.
    /// </summary>
    public class ListCommand
        : ICommand
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ListCommand()
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
                for (int i = 0; i < extensions.Count; i++) {
                    if (i >= 1)
                        Console.WriteLine();
                    ExtensionInfo entry = extensions[i];
                    Console.WriteLine("Extension    {0}", Util.QuoteIfNeeded(entry.Name));
                    WriteOptionalInfo("Version      {0}", entry.Version);
                    Console.WriteLine("Type         {0}", Util.QuoteIfNeeded(entry.Type));
                    Console.WriteLine("Assembly     {0}", entry.AssemblyName);
                    Console.WriteLine("Class        {0}", entry.ClassName);
                    WriteOptionalInfo("Description  {0}", entry.Description);
                    WriteOptionalInfo("UserGuide    {0}", entry.UserGuidePath);
                    WriteOptionalInfo("CoreVersion  {0}", entry.CoreVersion);
                }
            }
        }

        //---------------------------------------------------------------------

        private void WriteOptionalInfo(string outputLine,
                                       object info)
        {
            if (info == null)
                return;
            string infoStr = info.ToString();
            if (infoStr == null)
                return;
            if (infoStr.Trim(null) == "")
                return;
            Console.WriteLine(outputLine, Util.QuoteIfNeeded(infoStr));
        }
    }
}
