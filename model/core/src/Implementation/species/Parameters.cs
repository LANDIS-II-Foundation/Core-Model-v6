using Landis.Utilities;
using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// The parameters for a single tree species.
    /// </summary>
    public class Parameters
        : ISpeciesParameters
    {
        private string name;
        private int longevity;
        private int maturity;
        private byte shadeTolerance;
        private byte fireTolerance;
        private int effectiveSeedDist;
        private int maxSeedDist;
        private float vegReprodProb;
        private int minSproutAge;
        private int maxSproutAge;
        private PostFireRegeneration postFireRegen;

        //---------------------------------------------------------------------

        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        public int Longevity
        {
            get {
                return longevity;
            }
        }

        //---------------------------------------------------------------------

        public int Maturity
        {
            get {
                return maturity;
            }
        }

        //---------------------------------------------------------------------

        public byte ShadeTolerance
        {
            get {
                return shadeTolerance;
            }
        }

        //---------------------------------------------------------------------

        public byte FireTolerance
        {
            get {
                return fireTolerance;
            }
        }

        //---------------------------------------------------------------------

        public int EffectiveSeedDist
        {
            get {
                return effectiveSeedDist;
            }
        }

        //---------------------------------------------------------------------

        public int MaxSeedDist
        {
            get {
                return maxSeedDist;
            }
        }

        //---------------------------------------------------------------------

        public float VegReprodProb
        {
            get {
                return vegReprodProb;
            }
        }

        //---------------------------------------------------------------------

        public int MinSproutAge
        {
            get {
                return minSproutAge;
            }
        }

        //---------------------------------------------------------------------

        public int MaxSproutAge
        {
            get {
                return maxSproutAge;
            }
        }

        //---------------------------------------------------------------------

        public PostFireRegeneration PostFireRegeneration
        {
            get {
                return postFireRegen;
            }
        }

        //---------------------------------------------------------------------

        public Parameters(string name,
                          int longevity,
                          int maturity,
                          byte shadeTolerance,
                          byte fireTolerance,
                          int effectiveSeedDist,
                          int maxSeedDist,
                          float vegReprodProb,
                          int minSproutAge,
                          int maxSproutAge,
                          PostFireRegeneration postFireRegen)
        {
            this.name              = name;
            this.longevity         = longevity;
            this.maturity          = maturity;
            this.shadeTolerance    = shadeTolerance;
            this.fireTolerance     = fireTolerance;
            this.effectiveSeedDist = effectiveSeedDist;
            this.maxSeedDist       = maxSeedDist;
            this.vegReprodProb     = vegReprodProb;
            this.minSproutAge      = minSproutAge;
            this.maxSproutAge      = maxSproutAge;
            this.postFireRegen     = postFireRegen;
        }

        //---------------------------------------------------------------------

        public Parameters(ISpeciesParameters parameters)
        {
            name              = parameters.Name;
            longevity         = parameters.Longevity;
            maturity          = parameters.Maturity;
            shadeTolerance    = parameters.ShadeTolerance;
            fireTolerance     = parameters.FireTolerance;
            effectiveSeedDist = parameters.EffectiveSeedDist;
            maxSeedDist       = parameters.MaxSeedDist;
            vegReprodProb     = parameters.VegReprodProb;
            minSproutAge      = parameters.MinSproutAge;
            maxSproutAge      = parameters.MaxSproutAge;
            postFireRegen     = parameters.PostFireRegeneration;
        }
    }
}
