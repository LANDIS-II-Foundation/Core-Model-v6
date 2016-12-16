//  Author: Jimm Domingo, UW-Madison, FLEL

namespace Landis.Core
{
    /// <summary>
    /// Base class for succession plug-ins.
    /// </summary>
    public abstract class SuccessionMain
        : ExtensionMain
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected SuccessionMain(string name)
            : base(name, new ExtensionType("succession"))
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the site cohorts for the active sites.
        /// </summary>
        /// <param name="initialCommunities">
        /// Path to the file with initial communities' definitions.
        /// </param>
        /// <param name="initialCommunitiesMap">
        /// Path to the raster file showing where the initial communities are.
        /// </param>
        public abstract void InitializeSites(string initialCommunities,
                                             string initialCommunitiesMap,
                                             ICore modelCore);
    }
}
