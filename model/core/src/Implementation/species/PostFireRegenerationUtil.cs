using Landis.Utilities;

using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// Utility methods for the enumerated type PostFireRegeneration.
    /// </summary>
    public static class PostFireRegenerationUtil
    {
        /// <summary>
        /// Parses a word into a PostFireRegeneration value.
        /// </summary>
        /// <exception cref="System.FormatException">
        /// The word doesn't match any of these: "none", "resprout",
        /// "serotiny".
        /// </exception>
        public static PostFireRegeneration Parse(string word)
        {
            if (word == "none")
                return PostFireRegeneration.None;
            else if (word == "resprout")
                return PostFireRegeneration.Resprout;
            else if (word == "serotiny")
                return PostFireRegeneration.Serotiny;
            throw new System.FormatException("Valid values: none, resprout, serotiny");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values of the
        /// type PostFireRegeneration.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<PostFireRegeneration>("form of post-fire regeneration");
            InputValues.Register<PostFireRegeneration>(Parse);
        }
    }
}
