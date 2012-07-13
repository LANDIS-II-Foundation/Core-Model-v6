using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;
using System.IO;
using Landis.Core;


namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// A command that adds information about an extension to the plug-in
    /// database.
    /// </summary>
    public class AddCommand
        : ICommand
    {
        private string extensionInfoPath;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AddCommand(string extensionInfoPath)
        {
            this.extensionInfoPath = extensionInfoPath;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            Dataset dataset = Util.OpenDatasetForChange(Dataset.DefaultPath);
            EditableExtensionInfo.Dataset = dataset;
            //Model model = new Model(null, null);

            ExtensionParser parser = new ExtensionParser();
            ExtensionInfo extension = Load<ExtensionInfo>(extensionInfoPath, parser);

            dataset.Add(extension);
            dataset.Save();
            Console.WriteLine("Added the extension \"{0}\"", extension.Name);
        }
        /// <summary>
        /// Loads an instance of T from a file.  The file may contain a
        /// serialized form of an editable instance or it may be a text file
        /// that needs parsing.
        /// </summary>
        private T Load<T>(string path,
                                ITextParser<T> parser)
        {
            LineReader reader = this.OpenTextFile(path);
            try
            {
                return parser.Parse(reader);
            }
            finally
            {
                reader.Close();
            }
        }
        //-----------------------------------------------------------------------

        public FileLineReader OpenTextFile(string path)
        {
            Require.ArgumentNotNull(path);
            try
            {
                FileLineReader reader = new FileLineReader(path);
                reader.SkipBlankLines = true;

                reader.SkipCommentLines = true;
                reader.CommentLineMarker = ">>";

                reader.TrimEndComments = true;
                reader.EndCommentMarker = "<<";

                return reader;
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }
            catch (DirectoryNotFoundException)
            {
                string mesg = string.Format("Error: The directory does not exist");
                throw new System.ApplicationException(mesg);
            }
        }
    }
}
