using Landis.Utilities;
using Flel = Edu.Wisc.Forest.Flel;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Landis
{
    /// <summary>
    /// Methods for working with Landis data.
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// The name of the InputVariable that appears first in Landis text
        /// input.
        /// </summary>
        public const string InputVarName = "LandisData";

        //---------------------------------------------------------------------

        /// <summary>
        /// The filename extension for files with serialized Landis data
        /// objects.
        /// </summary>
        public const string FileExtension = "landis";

        //---------------------------------------------------------------------

        /// <summary>
        /// The markers for comments in Landis text data.
        /// </summary>
        public static class CommentMarkers
        {
            /// <summary>
            /// The marker for comment lines.
            /// </summary>
            public const string Line = ">>";

            /// <summary>
            /// The marker for end-of-line comments.
            /// </summary>
            public const string EndOfLine = "<<";
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Opens a text file for reading.
        /// </summary>
        /// <remarks>
        /// The file is configured so that blank lines and comment lines are
        /// skipped, and end-of-line comments are trimmed.  The marker for
        /// comment lines is ">" while the marker for end-of-line comments is
        /// "&lt;&lt;".
        /// </remarks>
        public static FileLineReader OpenTextFile(string path)
        {
            Require.ArgumentNotNull(path);
            try {
                FileLineReader reader = new FileLineReader(path);
                reader.SkipBlankLines = true;
       
                reader.SkipCommentLines = true;
                reader.CommentLineMarker = CommentMarkers.Line;
       
                reader.TrimEndComments = true;
                reader.EndCommentMarker = CommentMarkers.EndOfLine;
       
                return reader;
            }
            catch (FileNotFoundException) {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads an instance of T from a file.  The file may contain a
        /// serialized form of an editable instance or it may be a text file
        /// that needs parsing.
        /// </summary>
        public static T Load<T>(string         path,
                                ITextParser<T> parser)
        {
            if (Path.GetExtension(path) == FileExtension) {
                //  Deserialize an editable instance from the file
                //  Binary serialization:
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open,
                                               FileAccess.Read, FileShare.Read);
                using (stream) {
                    IEditable<T> editableObject = (IEditable<T>) formatter.Deserialize(stream);
                    if (! editableObject.IsComplete)
                        throw new System.ApplicationException("Not complete T");
                    return editableObject.GetComplete();
                }
            }
            else {
                LineReader reader = OpenTextFile(path);
                try {
                    return parser.Parse(reader);
                }
                finally {
                    reader.Close();
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new text file for writing.
        /// </summary>
        /// <param name="path">
        /// Path of the text file.
        /// </param>
        /// <remarks>
        /// If the path contains any directories that does not exists, they
        /// are created.
        ///
        /// If the file already exists, its current contents are overwritten.
        /// </remarks>
        public static StreamWriter CreateTextFile(string path)
        {
            Require.ArgumentNotNull(path);
            path = path.Trim(null);
            if (path.Length == 0)
                throw new ArgumentException("path is empty or just whitespace");

            string dir = Path.GetDirectoryName(path);
            if (! string.IsNullOrEmpty(dir)) {
                Flel.Util.Directory.EnsureExists(dir);
            }

            return new StreamWriter(path);
        }
    }
}
