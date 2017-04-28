using Landis.Utilities;
using Landis.Core;

namespace Landis
{
    /// <summary>
    /// A text parser for Landis data.
    /// </summary>
    public abstract class TextParser<T>
        : Landis.Utilities.TextParser<T>
    {
        /// <summary>
        /// The expected value for the LandisData InputVariable.
        /// </summary>
        public abstract string LandisDataValue
        {
            get;
        }

        //---------------------------------------------------------------------

        ///    <summary>
        /// Reads the InputVariable LandisData from the current line, and
        /// checks that its value matches the expected value.
        /// </summary>
        protected void ReadLandisDataVar()
        {
            InputVar<string> landisData = new InputVar<string>(Landis.Data.InputVarName);
            ReadVar(landisData);
            if (landisData.Value.Actual != LandisDataValue)
                throw new InputValueException(landisData.Value.String,
                                              "The value is not \"{0}\"",
                                              LandisDataValue);
        }
    }
}
