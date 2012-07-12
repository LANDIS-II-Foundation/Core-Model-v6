//  Authors: Jimm Domingo, Srinivas S., Robert Scheller; UW-Madison, Portland State University

using Landis.SpatialModeling;
using System.IO;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Troschuetz.Random;

namespace Landis.Core
{
    /// <summary>
    /// Interface to the core framework for plug-ins.
    /// </summary>
    public interface ICore
        : IRasterFactory
    {

        IUserInterface Log
        {
            get;
        }

        //---------------------------------------------------------------------

        Generator Generator
        {
            get;
        }

        //---------------------------------------------------------------------


        /// <summary>
        /// The dataset of species parameters for the scenario.
        /// </summary>
        ISpeciesDataset Species
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dataset of ecoregion parameters for the scenario.
        /// </summary>
        IEcoregionDataset Ecoregions
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The ecoregion for each site on the landscape.
        /// </summary>
        ISiteVar<IEcoregion> Ecoregion
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The landscape for the scenario.
        /// </summary>
        ILandscape Landscape
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The length of a side of a cell (a site on landscape).
        /// </summary>
        /// <remarks>
        /// Units: meters
        /// </remarks>
        float CellLength
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area of a cell (a site on landscape).
        /// </summary>
        /// <remarks>
        /// Units: hectares
        /// </remarks>
        float CellArea
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario starts from.
        /// </summary>
        /// <remarks>
        /// This year represents time step 0, so the first year in the scenario
        /// is this year plus 1.
        /// </remarks>
        int StartTime
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The last calendar year in the scenario.
        /// </summary>
        int EndTime
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year in the current time step.
        /// </summary>
        int CurrentTime
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of years from StartTime to CurrentTime.
        /// </summary>
        int TimeSinceStart
        {
            get;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Registers a site variable under a specific name.
        /// </summary>
        /// <param name="siteVar">
        /// The site variable being registered.
        /// </param>
        /// <param name="name">
        /// The name under which the site variable is to be registered.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// At least one of the parameters is null.
        /// </exception>
        /// <exception cref="System.ApplicationException">
        /// Another site variable has been previously registered with the same
        /// name.
        /// </exception>
        void RegisterSiteVar(ISiteVariable siteVar,
                             string        name);

        //-----------------------------------------------------------------

        /// <summary>
        /// Gets a site variable with a specific name and data type.
        /// </summary>
        /// <param name="name">
        /// The name under which the site variable is registered.
        /// </param>
        /// <returns>
        /// The site variable that was registered under the given name, or null
        /// if no site variable has been registered under the name.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="System.ApplicationException">
        /// The site variable registered with the given name has a different
        /// data type.
        /// </exception>
        ISiteVar<T> GetSiteVar<T>(string name);

        //------------------------------------------------------------------

                /// <summary>
        /// Writes an informational message into the log.
        /// </summary>
        /// <param name="message">
        /// Message to write into the log.  It may contain placeholders for
        /// optional arguments using the "{n}" notation used by the
        /// System.String.Format method.
        /// </param>
        /// <param name="mesgArgs">
        /// Optional arguments for the message.
        /// </param>
         void Info(string message,
                   params object[] mesgArgs);

        //--------------------------------------------------------------

        /// <summary>
        /// Initializes the random-number generator with a specific seed.
        /// </summary>
        void Initialize(uint seed);

        //---------------------------------------------------------------

        /// <summary>
        /// Generates a non-zero random seed using the system clock.
        /// This was found to fail on Mac OSX, due to identical results returned from System.Random (due to an internal error in System.Environment.TickCount)
        /// System.Random was replaced with System.Security.Cryptography.RandomNumberGenerator by Brendan C. Ward on 6/4/2008
        /// </summary>
        uint GenerateSeed();

        //---------------------------------------------------------------------

        [System.Obsolete("Use Landis.Data.OpenTextFile(...) instead")]
        FileLineReader OpenTextFile(string path);

        //---------------------------------------------------------------------

        [System.Obsolete("Use Landis.Data.Load<T>(...) instead")]
        T Load<T>(string path, ITextParser<T> parser);

        //---------------------------------------------------------------------

        List<T> shuffle<T>(List<T> list);

        //---------------------------------------------------------------------

        [System.Obsolete("Use Landis.Data.CreateTextFile(...) instead")]
        StreamWriter CreateTextFile(string path);

        //---------------------------------------------------------------------

        double GenerateUniform();

        //---------------------------------------------------------------------

        double NextDouble();

        //---------------------------------------------------------------------

        BetaDistribution BetaDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        BetaPrimeDistribution BetaPrimeDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        CauchyDistribution CauchyDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ChiDistribution ChiDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ChiSquareDistribution ChiSquareDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ContinuousUniformDistribution ContinuousUniformDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ErlangDistribution ErlangDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ExponentialDistribution ExponentialDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        FisherSnedecorDistribution FisherSnedecorDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        FisherTippettDistribution FisherTippettDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        GammaDistribution GammaDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        LaplaceDistribution LaplaceDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        LognormalDistribution LognormalDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        NormalDistribution NormalDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        ParetoDistribution ParetoDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        PowerDistribution PowerDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        RayleighDistribution RayleighDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        StudentsTDistribution StudentsTDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        TriangularDistribution TriangularDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        WeibullDistribution WeibullDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

        PoissonDistribution PoissonDistribution
        {
            get;
        }

        //------------------------------------------------------------------------

    }
}
