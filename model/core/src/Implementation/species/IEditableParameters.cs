using Landis.Utilities;

using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// Editable set of parameters for a tree species.
    /// </summary>
    public interface IEditableParameters
        : IEditable<ISpeciesParameters>
    {
        /// <summary>
        /// Name
        /// </summary>
        InputValue<string> Name
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Longevity (years)
        /// </summary>
        InputValue<int> Longevity
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Age of sexual maturity (years)
        /// </summary>
        InputValue<int> Maturity
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Shade tolerance class (1-5)
        /// </summary>
        InputValue<byte> ShadeTolerance
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Fire tolerance class (1-5)
        /// </summary>
        InputValue<byte> FireTolerance
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Effective seed dispersal distance (m?)
        /// </summary>
        InputValue<int> EffectiveSeedDist
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum seed dispersal distance (m?)
        /// </summary>
        InputValue<int> MaxSeedDist
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Vegetative reproduction probability
        /// </summary>
        InputValue<float> VegReprodProb
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum age for sprouting (years)
        /// </summary>
        InputValue<int> MinSproutAge
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum age for sprouting (years)
        /// </summary>
        InputValue<int> MaxSproutAge
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The form of reproduction that occurs after fires.
        /// </summary>
        InputValue<PostFireRegeneration> PostFireRegeneration
        {
            get;
            set;
        }
    }
}
