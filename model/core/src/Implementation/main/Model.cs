using Edu.Wisc.Forest.Flel.Util;
using Flel = Edu.Wisc.Forest.Flel;
using Loader = Edu.Wisc.Forest.Flel.Util.PlugIns.Loader;
using log4net;
using Landis.Core;

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
        private IExtensionDataset extensionDataset;
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
        private List<ExtensionMain> disturbAndOtherExtensions;
        private IUserInterface ui;

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

        //---------------------------------------------------------------------

        public double NextDouble()
        {
            return RandomNumberGenerator.NextDouble();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Model(IExtensionDataset extensionDataset,
                     IConfigurableRasterFactory rasterFactory,
                     ILandscapeFactory landscapeFactory)

        {
            this.extensionDataset = extensionDataset;
            siteVarRegistry = new SiteVarRegistry();

            this.rasterFactory = rasterFactory;
            this.landscapeFactory = landscapeFactory;

            BindExtensionToFormat(".bin", "ENVI");
            BindExtensionToFormat(".bmp", "BMP");
            BindExtensionToFormat(".gis", "LAN");
            BindExtensionToFormat(".img", "HFA");
            BindExtensionToFormat(".tif", "GTiff");
            BindExtensionToFormat(".ingr", "INGR");
            BindExtensionToFormat(".vrt",  "VRT" );
            
 
            ui = null;
        }

        //---------------------------------------------------------------------

        // Bind a file extension to a raster format if the format is supported
        // by the raster factory.
        private void BindExtensionToFormat(string fileExtension,
                                           string formatCode)
        {
            RasterFormat rasterFormat = rasterFactory.GetFormat(formatCode);
            if (rasterFormat != null)
                rasterFactory.BindExtensionToFormat(fileExtension, rasterFormat);
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

        IUserInterface ICore.UI
        {
            get
            {
                return ui;
            }
        }

        [System.Obsolete("Use the UI property instead.")]
        IUserInterface ICore.Log
        {
            get
            {
                return ui;
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
            ui.WriteLine("   Registering Data:  {0}.", name);
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
            this.ui = ui;

            siteVarRegistry.Clear();

            Scenario scenario = LoadScenario(scenarioPath);
            startTime = scenario.StartTime;
            endTime = scenario.EndTime;
            timeSinceStart = 0;
            currentTime = startTime;
           
            InitializeRandomNumGenerator(scenario.RandomNumberSeed);

            LoadSpecies(scenario.Species);
            LoadEcoregions(scenario.Ecoregions);
            
            ui.WriteLine("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
            Ecoregions.Map ecoregionsMap = new Ecoregions.Map(scenario.EcoregionsMap,
                                                              ecoregions,
                                                              rasterFactory);
            // -- ProcessMetadata(ecoregionsMap.Metadata, scenario);
            cellLength = scenario.CellLength.Value;
            cellArea = (float)((cellLength * cellLength) / 10000);
            ui.WriteLine("Cell length = {0} m, cell area = {1} ha", cellLength, cellArea);

            using (IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid()) {
                ui.WriteLine("Map dimensions: {0} = {1:#,##0} cell{2}", grid.Dimensions,
                             grid.Count, (grid.Count == 1 ? "" : "s"));
                // landscape = new Landscape(grid);
                landscape = landscapeFactory.CreateLandscape(grid);
            }
            ui.WriteLine("Sites: {0:#,##0} active ({1:p1}), {2:#,##0} inactive ({3:p1})",
                         landscape.ActiveSiteCount, (landscape.Count > 0 ? ((double)landscape.ActiveSiteCount)/landscape.Count : 0),
                         landscape.InactiveSiteCount, (landscape.Count > 0 ? ((double)landscape.InactiveSiteCount)/landscape.Count : 0));

            ecoregionSiteVar = ecoregionsMap.CreateSiteVar(landscape);

            disturbAndOtherExtensions = new List<ExtensionMain>();

     

            try {
                ui.WriteLine("Loading {0} extension ...", scenario.Succession.Info.Name);
                succession = Loader.Load<SuccessionMain>(scenario.Succession.Info);
                succession.LoadParameters(scenario.Succession.InitFile, this);
                
                succession.Initialize(); 
                
                ExtensionMain[] disturbanceExtensions = LoadExtensions(scenario.Disturbances);
                InitExtensions(disturbanceExtensions);
                
                ExtensionMain[] otherExtensions = LoadExtensions(scenario.OtherExtensions);
                InitExtensions(otherExtensions);

                OutputExtensionInfo(scenario.Succession, scenario.Disturbances, scenario.OtherExtensions);

                //  Perform 2nd phase of initialization for non-succession extensions.
                foreach (ExtensionMain extension in disturbanceExtensions)
                    extension.InitializePhase2();
                foreach (ExtensionMain extension in otherExtensions)
                    extension.InitializePhase2();

                //  Run output extensions for TimeSinceStart = 0 (time step 0)
                foreach (ExtensionMain extension in otherExtensions) {
                    if (extension.Type.IsMemberOf("output"))
                        Run(extension);
                }

                //******************// for Rob
                //  Main time loop  //
                //******************//

                for (currentTime++, timeSinceStart++;
                     currentTime <= endTime;
                     currentTime++, timeSinceStart++) {

                    bool isSuccTimestep = IsExtensionTimestep(succession);

                    List<ExtensionMain> distExtensionsToRun;
                    distExtensionsToRun = GetExtensionsToRun(disturbanceExtensions);
                    bool isDistTimestep = distExtensionsToRun.Count > 0;

                    List<ExtensionMain> otherExtensionsToRun;
                    otherExtensionsToRun = GetExtensionsToRun(otherExtensions);

                    if (!isSuccTimestep && !isDistTimestep
                                        && otherExtensionsToRun.Count == 0)
                        continue;

                    ui.WriteLine("Current time: {0}", currentTime);

                    if (isDistTimestep) {
                        if (scenario.DisturbancesRandomOrder)
                            distExtensionsToRun = shuffle(distExtensionsToRun);
                        foreach (ExtensionMain distExtension in distExtensionsToRun)
                            Run(distExtension);
                    }

                    if (isSuccTimestep || isDistTimestep)
                        Run(succession);

                    foreach (ExtensionMain otherExtension in otherExtensionsToRun)
                        Run(otherExtension);
                }  // main time loop
            }
            finally {
                foreach (ExtensionMain extension in disturbAndOtherExtensions)
                    extension.CleanUp();
            }
            ui.WriteLine("Model run is complete.");
        }

        //---------------------------------------------------------------------

        private void OutputExtensionInfo(ExtensionAndInitFile succession, ExtensionAndInitFile[] disturbances, ExtensionAndInitFile[] otherExtensions)
        {
            string toDisplay = "Using the following extensions ...\n";
            string format = "   {0,-25} {1,-25}\n";

            toDisplay += string.Format(format, "Extension Name", "Extension Filename");
            toDisplay += string.Format(format, "--------------", "------------------");

            toDisplay += string.Format(format, succession.Info.Name, succession.InitFile);
            
            foreach (ExtensionAndInitFile extension in disturbances)
                toDisplay += string.Format(format, extension.Info.Name, extension.InitFile); 

            foreach (ExtensionAndInitFile extension in otherExtensions)
                toDisplay += string.Format(format, extension.Info.Name, extension.InitFile);
            
            ui.WriteLine("{0}", toDisplay);
        }

        //---------------------------------------------------------------------

        private Scenario LoadScenario(string path)
        {
            ui.WriteLine("Loading scenario from file \"{0}\" ...", path);
            ScenarioParser parser = new ScenarioParser(extensionDataset);
            return Load<Scenario>(path, parser);
        }

        //---------------------------------------------------------------------

        private void InitializeRandomNumGenerator(uint? seed)
        {
            if (seed.HasValue)
            {
                Initialize(seed.Value);
                ui.WriteLine("Initialized random number generator with user-supplied seed = {0:#,##0}", seed.Value);
            }
            else
            {
                uint generatedSeed = GenerateSeed();
                Initialize(generatedSeed);
                ui.WriteLine("Initialized random number generator with seed = {0:#,##0}", generatedSeed);
            }

        }

        //---------------------------------------------------------------------

        private void LoadSpecies(string path)
        {
            ui.WriteLine("Loading species data from file \"{0}\" ...", path);
            Species.DatasetParser parser = new Species.DatasetParser();
            species = Load<ISpeciesDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private void LoadEcoregions(string path)
        {
            ui.WriteLine("Loading ecoregions from file \"{0}\" ...", path);
            Ecoregions.DatasetParser parser = new Ecoregions.DatasetParser();
            ecoregions = Load<IEcoregionDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private const string cellLengthExceptionPrefix = "Cell Length Exception: ";

        //---------------------------------------------------------------------

        //Begin of ui.cs file contents

        /// <summary>
        /// Writes an informational message into the ui.
        /// </summary>
        /// <param name="message">
        /// Message to write into the ui.  It may contain placeholders for
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

        //End of ui.cs file Contents

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

        private List<ExtensionMain> GetExtensionsToRun(ExtensionMain[] extensions)
        {
            List<ExtensionMain> extensionsToRun = new List<ExtensionMain>();
            foreach (ExtensionMain extension in extensions) {
                if (IsExtensionTimestep(extension))
                    extensionsToRun.Add(extension);
            }
            return extensionsToRun;
        }

        //---------------------------------------------------------------------

        private bool IsExtensionTimestep(ExtensionMain extension)
        {
            return (extension.Timestep > 0) && (timeSinceStart % extension.Timestep == 0);
        }

        //---------------------------------------------------------------------

        private void Run(ExtensionMain extension)
        {
            ui.WriteLine("Running {0} ...", extension.Name);
            extension.Run();
        }

        //---------------------------------------------------------------------

        private ExtensionMain[] LoadExtensions(ExtensionAndInitFile[] extensions)
        {

            ExtensionMain[] loadedExtensions = new ExtensionMain[extensions.Length];
            foreach (int i in Indexes.Of(extensions))
            {
                ExtensionAndInitFile extensionAndInitFile = extensions[i];
                ui.WriteLine("Loading {0} extension ...", extensionAndInitFile.Info.Name);
                ExtensionMain loadedExtension = Loader.Load<ExtensionMain>(extensionAndInitFile.Info);
                loadedExtension.LoadParameters(extensionAndInitFile.InitFile, this);

                loadedExtensions[i] = loadedExtension;

                disturbAndOtherExtensions.Add(loadedExtension);
            }
            return loadedExtensions;
        }

        //-----------------------------------------------------------------------

        private void InitExtensions(ExtensionMain[] extensions)
        {
            foreach (ExtensionMain extension in extensions)
            {
                extension.Initialize();
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
