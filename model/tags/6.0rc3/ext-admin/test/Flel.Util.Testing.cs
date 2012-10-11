namespace Edu.Wisc.Forest.Flel.Util
{
    /// <summary>
    /// Methods related to testing software.
    /// </summary>
    public static class Testing
    {
        /// <summary>
        /// The text that marks a line where an error is expected in an input
        /// input.
        /// </summary>
        public const string ErrorLineMarker = "<< ERROR HERE:";

        //---------------------------------------------------------------------

        /// <summary>
        /// The text that indicates an error is expected at the end of an
        /// input file.
        /// </summary>
        public const string ErrorEofMarker = "<< ERROR EOF:";

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds the first occurence of an error marker in a text file.
        /// </summary>
        /// <returns>
        /// The line number where the marker was found if the first marker is
        /// the ErrorLineMarker.  If the first marker is the ErrorEofMarker,
        /// then LineReader.EndOfInput is returned.  If no marker is found,
        /// then null is returned.
        /// </returns>
        public static int? FindErrorMarker(string path)
        {
            FileLineReader reader = new FileLineReader(path);
            try {
                string line = reader.ReadLine();
                while (line != null) {
                    if (line.Contains(ErrorLineMarker))
                        return reader.LineNumber;
                    if (line.Contains(ErrorEofMarker))
                        return LineReader.EndOfInput;
                    line = reader.ReadLine();
                }
                return null;
            }
            finally {
                reader.Close();
            }
        }
    }
}
