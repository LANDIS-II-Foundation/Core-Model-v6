using Landis.Utilities;

namespace Landis.Species
{
    /// <summary>
    /// Utility class for the effective seed-dispersal distances of species.
    /// </summary>
    public static class EffectiveSeedDist
    {
        /// <summary>
        /// A species' effective seed-dispersal distance is set to this
        /// constant to signify that the species is universally present on
        /// the landscape.
        /// </summary>
        public const int Universal = -1;

        /// <summary>
        /// String value to use as input to specify universally present.
        /// </summary>
        public const string UniversalAsString = "uni";

        //---------------------------------------------------------------------

        ///    <summary>
        /// Reads an integer input value for an effective seed-dispersal
        /// distance.
        /// </summary>
        /// <param name="reader">
        /// The string reader from which the input value is read.
        /// </param>
        /// <param name="index">
        /// The starting index in the reader's input stream where the input
        /// value was located.
        /// </param>
        /// <remarks>
        /// This method uses the registered ReadMethod for integer values.
        /// The input value "uni" is treated specially, and yield an integer
        /// input value equal to the Universal constant.
        /// </remarks>
        public static InputValue<int> ReadMethod(StringReader reader,
                                                 out int      index)
        {
            ReadMethod<int> intRead = InputValues.GetReadMethod<int>();
            try {
                return intRead(reader, out index);
            }
            catch (InputValueException exc) {
                if (exc.Value == UniversalAsString) {
                    index = reader.Index - exc.Value.Length;
                    return new InputValue<int>(Universal, exc.Value);
                }
                else if (exc.Message.Contains("outside the range")) {
                    //    Overflow exception with integer value
                    throw;
                }
                else {
                    string message = string.Format("\"{0}\" is not a valid integer or \"{1}\"",
                                                   exc.Value, UniversalAsString);
                    throw new InputValueException(exc.Value, message);
                }
            }
        }
    }
}
