using Edu.Wisc.Forest.Flel.Util;
using Flel = Edu.Wisc.Forest.Flel;
using Loader = Edu.Wisc.Forest.Flel.Util.PlugIns.Loader;
using log4net;
using Landis.Core;

using Landis.PlugIns;


using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


using Landis.SpatialModeling;

using Troschuetz.Random;


namespace Landis
{
    public class Model
        : ICore
    {
        private PlugIns.IDataset plugInDataset;
        private SiteVarRegistry siteVarRegistry;
        private ISpeciesDataset species;
        private IEcoregionDataset ecoregions;
        private IConfigurableRasterFactory rasterFactory;
        private ILandscapeFactory landscapeFactory;
        private ILandscape landscape;
        private float cellLength;  // meters
        private float cellArea;    // hectares
        private ISiteVar<IEcoregion> ecoregionSiteVar;
        private int startTime;
        private int endTime;
        private int currentTime;
        private int timeSinceStart;
        private SuccessionMain succession;
        private List<ExtensionMain> disturbAndOtherPlugIns;

        private static Generator RandomNumberGenerator;
        private static BetaDistribution betaDist;
        private static BetaPrimeDistribution betaPrimeDist;
        private static CauchyDistribution cauchyDist;
        private static ChiDistribution chiDist;
        private static ChiSquareDistribution chiSquareDist;
        private static ContinuousUniformDistribution continuousUniformDist;
        private static ErlangDistribution erlangDist;
        private static ExponentialDistribution exponentialDist;
        private static FisherSnedecorDistribution fisherSnedecorDist;
        private static FisherTippettDistribution fisherTippettDist;
        private static GammaDistribution gammaDist;
        private static LaplaceDistribution laplaceDist;
        private static LognormalDistribution lognormalDist;
        private static NormalDistribution normalDist;
        private static ParetoDistribution paretoDist;
        private static PowerDistribution powerDist;
        private static RayleighDistribution rayleighDist;
        private static StudentsTDistribution studentsTDist;
        private static TriangularDistribution triangularDist;
        private static WeibullDistribution weibullDist;
        private static PoissonDistribution poissonDist;

        //---------------------------------------------------------------------

        private static ILog logger= LogManager.GetLogger("Landis");

        private static IUserInterface log;//move to ICore

        //---------------------------------------------------------------------

        public double NextDouble()
        {
            return RandomNumberGenerator.NextDouble();
        }

        //---------------------------------------------------------------------

        public static IUserInterface Log
        {
            get
            {
                return log;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Model(PlugIns.IDataset plugInDataset,
                     IConfigurableRasterFactory rasterFactory,
                     ILandscapeFactory landscapeFactory)

        {
            this.plugInDataset = plugInDataset;
            siteVarRegistry = new SiteVarRegistry();

            this.rasterFactory = rasterFactory;
            this.landscapeFactory = landscapeFactory;

            rasterFactory.BindExtensionToFormat(".bin", "ENVI");
            rasterFactory.BindExtensionToFormat(".bmp", "BMP");
            rasterFactory.BindExtensionToFormat(".gis", "LAN");
            rasterFactory.BindExtensionToFormat(".img", "HFA");
            rasterFactory.BindExtensionToFormat(".tif", "GTiff");
            rasterFactory.BindExtensionToFormat(".ingr", "INGR");
            rasterFactory.BindExtensionToFormat(".vrt",  "VRT" );
        }


         //---------------------------------------------------------------------

        IInputRaster<TPixel> IRasterFactory.OpenRaster<TPixel>(string path)
        {
            return rasterFactory.OpenRaster<TPixel>(path);
        }

        //---------------------------------------------------------------------


        IOutputRaster<TPixel> IRasterFactory.CreateRaster<TPixel>(string         path,
                                                                  Dimensions dimensions)
        {
            try {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (dir.Length > 0)
                    Edu.Wisc.Forest.Flel.Util.Directory.EnsureExists(dir);
                return rasterFactory.CreateRaster<TPixel>(path, dimensions);
            }
            catch (System.IO.IOException exc) {
                string mesg = string.Format("Error opening map \"{0}\"", path);
                throw new MultiLineException(mesg, exc);
            }
        }


        //---------------------------------------------------------------------

        IUserInterface ICore.Log
        {
            get
            {
                return log;
            }
        }


        //---------------------------------------------------------------------

        Generator ICore.Generator
        {
            get
            {
                return RandomNumberGenerator;
            }
        }

        //---------------------------------------------------------------------

        ISpeciesDataset ICore.Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        IEcoregionDataset ICore.Ecoregions
        {
            get {
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        ISiteVar<IEcoregion> ICore.Ecoregion
        {
            get {
                return ecoregionSiteVar;
            }
        }

        //---------------------------------------------------------------------

        ILandscape ICore.Landscape
        {
            get {
                return landscape;
            }
        }

        //---------------------------------------------------------------------

        float ICore.CellLength
        {
            get {
                return cellLength;
            }
        }

        //---------------------------------------------------------------------

        float ICore.CellArea
        {
            get {
                return cellArea;
            }
        }

        //---------------------------------------------------------------------

        int ICore.StartTime
        {
            get {
                return startTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.EndTime
        {
            get {
                return endTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.CurrentTime
        {
            get {
                return currentTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.TimeSinceStart
        {
            get {
                return timeSinceStart;
            }
        }

        //---------------------------------------------------------------------

        void ICore.RegisterSiteVar(ISiteVariable siteVar,
                                   string        name)
        {
            Log.WriteLine("   Registering Data:  {0}.", name);
            siteVarRegistry.RegisterVar(siteVar, name);
        }

        //---------------------------------------------------------------------

        ISiteVar<T> ICore.GetSiteVar<T>(string name)
        {
            return siteVarRegistry.GetVar<T>(name);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs a model scenario.
        /// </summary>
        public void Run(string scenarioPath, IUserInterface ui)
        {
            log = ui;

            siteVarRegistry.Clear();

            Scenario scenario = LoadScenario(scenarioPath);
            startTime = scenario.StartTime;
            endTime = scenario.EndTime;
            timeSinceStart = 0;
            currentTime = startTime;
            InitializeRandomNumGenerator(scenario.RandomNumberSeed);

            LoadSpecies(scenario.Species);
            LoadEcoregions(scenario.Ecoregions);

            log.WriteLine("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
            Ecoregions.Map ecoregionsMap = new Ecoregions.Map(scenario.EcoregionsMap,
                                                              ecoregions,
                                                              rasterFactory);
            // -- ProcessMetadata(ecoregionsMap.Metadata, scenario);
            cellLength = scenario.CellLength.Value;
            cellArea = (float)((cellLength * cellLength) / 10000);
            log.WriteLine("Cell length = {0} m, cell area = {1} ha", cellLength, cellArea);

            using (IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid()) {
                log.WriteLine("Map dimensions: {0} = {1:#,##0} cell{2}", grid.Dimensions,
                             grid.Count, (grid.Count == 1 ? "" : "s"));
                // landscape = new Landscape(grid);
                landscape = landscapeFactory.CreateLandscape(grid);
            }
            log.WriteLine("Sites: {0:#,##0} active ({1:p1}), {2:#,##0} inactive ({3:p1})",
                         landscape.ActiveSiteCount, (landscape.Count > 0 ? ((double)landscape.ActiveSiteCount)/landscape.Count : 0),
                         landscape.InactiveSiteCount, (landscape.Count > 0 ? ((double)landscape.InactiveSiteCount)/landscape.Count : 0));

            ecoregionSiteVar = ecoregionsMap.CreateSiteVar(landscape);

            disturbAndOtherPlugIns = new List<ExtensionMain>();

            try {
                log.WriteLine("Loading {0} plug-in ...", scenario.Succession.Info.Name);
                succession = Loader.Load<SuccessionMain>(scenario.Succession.Info);
                succession.LoadParameters(scenario.Succession.InitFile, this);
                succession.Initialize();

                ExtensionMain[] disturbancePlugIns = LoadPlugIns(scenario.Disturbances);
                InitPlugIns(disturbancePlugIns);

                ExtensionMain[] otherPlugIns = LoadPlugIns(scenario.OtherPlugIns);
                InitPlugIns(otherPlugIns);


                //  Perform 2nd phase of initialization for non-succession plug-ins.
                foreach (ExtensionMain plugIn in disturbancePlugIns)
                    plugIn.InitializePhase2();
                foreach (ExtensionMain plugIn in otherPlugIns)
                    plugIn.InitializePhase2();

                //  Run output plug-ins for TimeSinceStart = 0 (time step 0)
                foreach (ExtensionMain plugIn in otherPlugIns) {
                    if (plugIn.PlugInType.IsMemberOf("output"))
                        Run(plugIn);
                }

                //******************// for Rob
                //  Main time loop  //
                //******************//

                for (currentTime++, timeSinceStart++;
                     currentTime <= endTime;
                     currentTime++, timeSinceStart++) {

                    bool isSuccTimestep = IsPlugInTimestep(succession);

                    List<ExtensionMain> distPlugInsToRun;
                    distPlugInsToRun = GetPlugInsToRun(disturbancePlugIns);
                    bool isDistTimestep = distPlugInsToRun.Count > 0;

                    List<ExtensionMain> otherPlugInsToRun;
                    otherPlugInsToRun = GetPlugInsToRun(otherPlugIns);

                    if (!isSuccTimestep && !isDistTimestep
                                        && otherPlugInsToRun.Count == 0)
                        continue;

                    log.WriteLine("Current time: {0}", currentTime);

                    if (isDistTimestep) {
                        if (scenario.DisturbancesRandomOrder)
                            distPlugInsToRun = shuffle(distPlugInsToRun);
                        foreach (ExtensionMain distPlugIn in distPlugInsToRun)
                            Run(distPlugIn);
                    }

                    if (isSuccTimestep || isDistTimestep)
                        Run(succession);

                    foreach (ExtensionMain otherPlugIn in otherPlugInsToRun)
                        Run(otherPlugIn);
                }  // main time loop
            }
            finally {
                foreach (ExtensionMain plugIn in disturbAndOtherPlugIns)
                    plugIn.CleanUp();
            }
            log.WriteLine("Model run is complete.");
        }

        //---------------------------------------------------------------------

        private Scenario LoadScenario(string path)
        {
            log.WriteLine("Loading scenario from file \"{0}\" ...", path);
            ScenarioParser parser = new ScenarioParser(plugInDataset);
            return Load<Scenario>(path, parser);
        }

        //---------------------------------------------------------------------

        private void InitializeRandomNumGenerator(uint? seed)
        {
            if (seed.HasValue)
                Initialize(seed.Value);
            else {
                uint generatedSeed = GenerateSeed();
                Initialize(generatedSeed);
                log.WriteLine("Initialized random number generator with seed = {0:#,##0}", generatedSeed);
            }
        }

        //---------------------------------------------------------------------

        private void LoadSpecies(string path)
        {
            log.WriteLine("Loading species data from file \"{0}\" ...", path);
            Species.DatasetParser parser = new Species.DatasetParser();
            species = Load<ISpeciesDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private void LoadEcoregions(string path)
        {
            log.WriteLine("Loading ecoregions from file \"{0}\" ...", path);
            Ecoregions.DatasetParser parser = new Ecoregions.DatasetParser();
            ecoregions = Load<IEcoregionDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private const string cellLengthExceptionPrefix = "Cell Length Exception: ";

        //---------------------------------------------------------------------

        //Begin of Log.cs file contents

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
        public  void Info(string message,
                                params object[] mesgArgs)
        {
            logger.Info(string.Format(message, mesgArgs));
        }

        //End of Log.cs file Contents

        //-------------------------------------------------------------------------

        //Begin of Random.cs file contents
        /// <summary>
        /// Initializes the random-number generator with a specific seed.
        /// </summary>
        public void Initialize(uint seed)
        {
            RandomNumberGenerator = new MT19937Generator(seed);
            betaDist = new BetaDistribution(RandomNumberGenerator);
            betaPrimeDist = new BetaPrimeDistribution(RandomNumberGenerator);
            cauchyDist = new CauchyDistribution(RandomNumberGenerator);
            chiDist = new ChiDistribution(RandomNumberGenerator);
            chiSquareDist = new ChiSquareDistribution(RandomNumberGenerator);
            continuousUniformDist = new ContinuousUniformDistribution(RandomNumberGenerator);
            erlangDist = new ErlangDistribution(RandomNumberGenerator);
            exponentialDist = new ExponentialDistribution(RandomNumberGenerator);
            fisherSnedecorDist = new FisherSnedecorDistribution(RandomNumberGenerator);
            fisherTippettDist = new FisherTippettDistribution(RandomNumberGenerator);
            gammaDist = new GammaDistribution(RandomNumberGenerator);
            laplaceDist = new LaplaceDistribution(RandomNumberGenerator);
            lognormalDist = new LognormalDistribution(RandomNumberGenerator);
            normalDist = new NormalDistribution(RandomNumberGenerator);
            paretoDist = new ParetoDistribution(RandomNumberGenerator);
            powerDist = new PowerDistribution(RandomNumberGenerator);
            rayleighDist = new RayleighDistribution(RandomNumberGenerator);
            studentsTDist = new StudentsTDistribution(RandomNumberGenerator);
            triangularDist = new TriangularDistribution(RandomNumberGenerator);
            weibullDist = new WeibullDistribution(RandomNumberGenerator);
            poissonDist = new PoissonDistribution(RandomNumberGenerator);

            // generator.randomGenerator = new MT19937Generator(seed);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Generates a non-zero random seed using the system clock.
        /// This was found to fail on Mac OSX, due to identical results returned from System.Random (due to an internal error in System.Environment.TickCount)
        /// System.Random was replaced with System.Security.Cryptography.RandomNumberGenerator by Brendan C. Ward on 6/4/2008
        /// </summary>
        public uint GenerateSeed()
        {
            System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            uint seed;

            do
            {
                byte[] temp = new byte[4];
                rng.GetBytes(temp);
                seed = System.BitConverter.ToUInt32(temp, 0);
            } while (seed == 0);
            return seed;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Generates a random number with a uniform distribution between
        /// 0.0 and 1.0.
        /// </summary>
        double ICore.GenerateUniform()
        {
            return RandomNumberGenerator.NextDouble();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Shuffles the items in a list.
        /// </summary>
        /// <param name="list">
        /// The list to be shuffled.
        /// </param>

        //public static List<T> shuffle<T>(List<T> list)
        public List<T> shuffle<T>(List<T> list)
        {
            List<T> shuffledList = new List<T>();

            int randomIndex = 0;
            while (list.Count > 0)
            {
                randomIndex = RandomNumberGenerator.Next(list.Count); //Choose a random object in the list
                shuffledList.Add(list[randomIndex]); //add it to the new, random list
                list.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return shuffledList;
        }


        //---------------------------------------------------------------------

        private ApplicationException CellLengthException(string          message,
                                                         params object[] mesgArgs)
        {
            string excMessage;
            if (mesgArgs == null)
                excMessage = message;
            else
                excMessage = string.Format(message, mesgArgs);
            return new ApplicationException(cellLengthExceptionPrefix + excMessage);
        }

        //---------------------------------------------------------------------

        private List<ExtensionMain> GetPlugInsToRun(ExtensionMain[] plugIns)
        {
            List<ExtensionMain> plugInsToRun = new List<ExtensionMain>();
            foreach (ExtensionMain plugIn in plugIns) {
                if (IsPlugInTimestep(plugIn))
                    plugInsToRun.Add(plugIn);
            }
            return plugInsToRun;
        }

        //---------------------------------------------------------------------

        private bool IsPlugInTimestep(ExtensionMain plugIn)
        {
            return (plugIn.Timestep > 0) && (timeSinceStart % plugIn.Timestep == 0);
        }

        //---------------------------------------------------------------------

        private void Run(ExtensionMain plugIn)
        {
            log.WriteLine("Running {0} ...", plugIn.Name);
            plugIn.Run();
        }

        //---------------------------------------------------------------------

        private ExtensionMain[] LoadPlugIns(PlugInAndInitFile[] plugIns)
        {

            ExtensionMain[] loadedPlugIns = new ExtensionMain[plugIns.Length];
            foreach (int i in Indexes.Of(plugIns))
            {
                PlugInAndInitFile plugInAndInitFile = plugIns[i];
                log.WriteLine("Loading {0} plug-in ...", plugInAndInitFile.Info.Name);
                ExtensionMain loadedPlugIn = Loader.Load<ExtensionMain>(plugInAndInitFile.Info);
                loadedPlugIn.LoadParameters(plugInAndInitFile.InitFile, this);

                loadedPlugIns[i] = loadedPlugIn;

                disturbAndOtherPlugIns.Add(loadedPlugIn);
            }
            return loadedPlugIns;
        }

        //-----------------------------------------------------------------------

        private void InitPlugIns(ExtensionMain[] plugIns)
        {
            foreach (ExtensionMain plugIn in plugIns)
            {
                plugIn.Initialize();
            }
        }

        //-----------------------------------------------------------------------

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public FileLineReader OpenTextFile(string path)
        {
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public T Load<T>(string path,
                                ITextParser<T> parser)
        {
            return Landis.Data.Load<T>(path, parser);
        }

        //---------------------------------------------------------------------

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public StreamWriter CreateTextFile(string path)
        {
            return Landis.Data.CreateTextFile(path);
        }


        //------------------------------------------------------------------------

        public BetaDistribution BetaDistribution
        {
            get
            {
                return betaDist;
            }
        }

        //------------------------------------------------------------------------

        public BetaPrimeDistribution BetaPrimeDistribution
        {
            get
            {
                return betaPrimeDist;
            }
        }

        //------------------------------------------------------------------------

        public CauchyDistribution CauchyDistribution
        {
            get
            {
                return cauchyDist;
            }
        }

        //------------------------------------------------------------------------

        public ChiDistribution ChiDistribution
        {
            get
            {
                return chiDist;
            }
        }

        //------------------------------------------------------------------------

        public ChiSquareDistribution ChiSquareDistribution
        {
            get
            {
                return chiSquareDist;
            }
        }

        //------------------------------------------------------------------------

        public ContinuousUniformDistribution ContinuousUniformDistribution
        {
            get
            {
                return continuousUniformDist;
            }
        }

        //------------------------------------------------------------------------

        public ErlangDistribution ErlangDistribution
        {
            get
            {
                return erlangDist;
            }
        }

        //------------------------------------------------------------------------

        public ExponentialDistribution ExponentialDistribution
        {
            get
            {
                return exponentialDist;
            }
        }

        //------------------------------------------------------------------------

        public FisherSnedecorDistribution FisherSnedecorDistribution
        {
            get
            {
                return fisherSnedecorDist;
            }
        }

        //------------------------------------------------------------------------

        public FisherTippettDistribution FisherTippettDistribution
        {
            get
            {
                return fisherTippettDist;
            }
        }

        //------------------------------------------------------------------------

        public GammaDistribution GammaDistribution
        {
            get
            {
                return gammaDist;
            }
        }

        //------------------------------------------------------------------------

        public LaplaceDistribution LaplaceDistribution
        {
            get
            {
                return laplaceDist;
            }
        }

        //------------------------------------------------------------------------

        public LognormalDistribution LognormalDistribution
        {
            get
            {
                return lognormalDist;
            }
        }

        //------------------------------------------------------------------------

        public NormalDistribution NormalDistribution
        {
            get
            {
                return normalDist;
            }
        }

        //------------------------------------------------------------------------

        public ParetoDistribution ParetoDistribution
        {
            get
            {
                return paretoDist;
            }
        }

        //------------------------------------------------------------------------

        public PowerDistribution PowerDistribution
        {
            get
            {
                return powerDist;
            }
        }

        //------------------------------------------------------------------------

        public RayleighDistribution RayleighDistribution
        {
            get
            {
                return rayleighDist;
            }
        }

        //------------------------------------------------------------------------

        public StudentsTDistribution StudentsTDistribution
        {
            get
            {
                return studentsTDist;
            }
        }

        //------------------------------------------------------------------------

        public TriangularDistribution TriangularDistribution
        {
            get
            {
                return triangularDist;
            }
        }

        //------------------------------------------------------------------------

        public WeibullDistribution WeibullDistribution
        {
            get
            {
                return weibullDist;
            }
        }

        //------------------------------------------------------------------------

        public PoissonDistribution PoissonDistribution
        {
            get
            {
                return poissonDist;
            }
        }

        //------------------------------------------------------------------------
    }

}
