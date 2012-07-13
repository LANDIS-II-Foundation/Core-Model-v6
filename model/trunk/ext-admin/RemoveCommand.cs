using System;

namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// A command that remove an extension to the plug-in database.
    /// </summary>
    public class RemoveCommand
        : ICommand
    {
        private string extensionName;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RemoveCommand(string extensionName)
        {
            this.extensionName = extensionName;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            Dataset dataset = Util.OpenDatasetForChange(Dataset.DefaultPath);
            if (dataset.Count == 0)
                Console.WriteLine("No extensions are installed.");
            else {
                ExtensionInfo entry = dataset.Remove(extensionName);
                if (entry == null)
                    Console.WriteLine("There is no extension named \"{0}\".", extensionName);
                else {
                    Console.WriteLine("Removed the extension \"{0}\".", extensionName);
                    dataset.Save();
                }
            }
        }
    }
}
