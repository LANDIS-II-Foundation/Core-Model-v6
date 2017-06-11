using Landis.Utilities;

using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// The parameters for a single tree species that can be edited.
    /// </summary>
    public class EditableParameters
        : IEditableParameters
    {
        private InputValue<string> name;
        private InputValue<int> longevity;
        private InputValue<int> maturity;
        private InputValue<byte> shadeTolerance;
        private InputValue<byte> fireTolerance;
        private InputValue<int> effectiveSeedDist;
        private InputValue<int> maxSeedDist;
        private InputValue<float> vegReprodProb;
        private InputValue<int> minSproutAge;
        private InputValue<int> maxSproutAge;
        private InputValue<PostFireRegeneration> postFireRegen;

        //---------------------------------------------------------------------

        /// <summary>
        /// Name
        /// </summary>
        public InputValue<string> Name
        {
            get {
                return name;
            }

            set {
                name = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Longevity (years)
        /// </summary>
        public InputValue<int> Longevity
        {
            get {
                return longevity;
            }

            set {
                if (value != null) {
                    if (maturity != null) {
                        if (value.Actual < maturity)
                            throw new InputValueException(value.String,
                                                          "Longevity must be = or > sexual maturity. Species Name: " + name.ToString());
                    }
                    if (maxSproutAge != null) {
                        if (value.Actual < maxSproutAge)
                            throw new InputValueException(value.String,
                                                          "Longevity must be = or > maximum sprouting age. Species Name: " + name.ToString());
                    }
                    if (maturity == null && maxSproutAge == null) {
                        if (value.Actual < 0)
                            throw new InputValueException(value.String,
                                                          "Longevity must be = or > 0. Species Name: " + name.ToString());
                    }
                }
                longevity = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Age of sexual maturity (years)
        /// </summary>
        public InputValue<int> Maturity
        {
            get {
                return maturity;
            }

            set {
                if (value != null) {
                    if (longevity != null) {
                        if (value.Actual > longevity)
                            throw new InputValueException(value.String,
                                                          "Sexual maturity must be < or = longevity. Species Name: " + name.ToString());
                    }
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Sexual maturity must be = or > 0. Species Name: " + name.ToString());
                }
                maturity = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Shade tolerance class (1-5)
        /// </summary>
        public InputValue<byte> ShadeTolerance
        {
            get {
                return shadeTolerance;
            }

            set {
                if (value != null)
                    ValidateTolerance(value);
                shadeTolerance = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Fire tolerance class (1-5)
        /// </summary>
        public InputValue<byte> FireTolerance
        {
            get {
                return fireTolerance;
            }

            set {
                if (value != null)
                    ValidateTolerance(value);
                fireTolerance = value;
            }
        }

        //---------------------------------------------------------------------

        private void ValidateTolerance(InputValue<byte> tolerance)
        {
            if (1 <= tolerance.Actual && tolerance.Actual <= 5)
                return;
            throw new InputValueException(tolerance.String,
                                          tolerance.String + " is not between 1 and 5. Species Name: " + name.ToString());
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Effective seed dispersal distance (m?)
        /// </summary>
        public InputValue<int> EffectiveSeedDist
        {
            get {
                return effectiveSeedDist;
            }

            set {
                if (value != null && value.Actual != Landis.Species.EffectiveSeedDist.Universal) {
                    if (maxSeedDist != null) {
                        if (value.Actual > maxSeedDist.Actual)
                            throw new InputValueException(value.String,
                                                          "Effective seed distance must be < or = maximum seed distance. Species Name: " + name.ToString());
                    }
                    else {
                        if (value.Actual <= 0)
                            throw new InputValueException(value.String,
                                                          "Effective seed distance must be > 0. Species Name: " + name.ToString());
                    }
                }
                effectiveSeedDist = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum seed dispersal distance (m?)
        /// </summary>
        public InputValue<int> MaxSeedDist
        {
            get {
                return maxSeedDist;
            }

            set {
                if (value != null) {
                    if (effectiveSeedDist != null && effectiveSeedDist.Actual != Landis.Species.EffectiveSeedDist.Universal) {
                        if (value.Actual < effectiveSeedDist.Actual)
                            throw new InputValueException(value.String,
                                                          "Maximum seed distance must be = or > effective seed distance. Species Name: " + name.ToString());
                    }
                    else {
                        if (value.Actual < 0)
                            throw new InputValueException(value.String,
                                                          "Maximum seed distance must be = or > 0. Species Name: " + name.ToString());
                    }
                }
                maxSeedDist = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Vegetative reproduction probability
        /// </summary>
        public InputValue<float> VegReprodProb
        {
            get {
                return vegReprodProb;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0.0 || value.Actual > 1.0)
                        throw new InputValueException(value.String,
                                                      value.String + " is not between 0 and 1.0. Species Name: " + name.ToString());
                }
                vegReprodProb = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum age for sprouting (years)
        /// </summary>
        public InputValue<int> MinSproutAge
        {
            get {
                return minSproutAge;
            }

            set {
                if (value != null) {
                    if (maxSproutAge != null) {
                        if (value.Actual > maxSproutAge)
                            throw new InputValueException(value.String,
                                                          "Minimum sprouting age must be < or = maximum sprouting age. Species Name: " + name.ToString());
                    }
                    else if (longevity != null) {
                        if (value.Actual > longevity)
                            throw new InputValueException(value.String,
                                                          "Minimum sprouting age must be < or = longevity. Species Name: " + name.ToString());
                    }
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Minimum sprouting age must be < or = 0. Species Name: " + name.ToString());
                }
                minSproutAge = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum age for sprouting (years)
        /// </summary>
        public InputValue<int> MaxSproutAge
        {
            get {
                return maxSproutAge;
            }

            set {
                if (value != null) {
                    if (longevity != null) {
                        if (value.Actual > longevity)
                            throw new InputValueException(value.String,
                                                          "Maximum sprouting age must be < or = longevity. Species Name: " + name.ToString());
                    }
                    if (minSproutAge != null) {
                        if (value.Actual < minSproutAge)
                            throw new InputValueException(value.String,
                                                          "Maximum sprouting age must be = or > minimum sprouting age. Species Name: " + name.ToString());
                    }
                    else if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Maximum sprouting age must be = or > 0. Species Name: " + name.ToString());
                }
                maxSproutAge = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The form of reproduction that occurs after fires.
        /// </summary>
        public InputValue<PostFireRegeneration> PostFireRegeneration
        {
            get {
                return postFireRegen;
            }

            set {
                postFireRegen = value;
            }
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                object[] parameters = new object[]{
                    name, longevity, maturity,
                    shadeTolerance, fireTolerance,
                    effectiveSeedDist, maxSeedDist,
                    vegReprodProb, minSproutAge, maxSproutAge, postFireRegen};
                foreach (object parameter in parameters)
                    if (parameter == null)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        public ISpeciesParameters GetComplete()
        {
            if (this.IsComplete)
                return new Parameters(name.Actual,
                                      longevity.Actual,
                                      maturity.Actual,
                                      shadeTolerance.Actual,
                                      fireTolerance.Actual,
                                      effectiveSeedDist.Actual,
                                      maxSeedDist.Actual,
                                      vegReprodProb.Actual,
                                      minSproutAge.Actual,
                                      maxSproutAge.Actual,
                                      postFireRegen.Actual);
            else
                return null;
        }
    }
}
