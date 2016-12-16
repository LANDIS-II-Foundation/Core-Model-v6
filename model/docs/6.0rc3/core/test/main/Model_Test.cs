using Edu.Wisc.Forest.Flel.Util;
using Gov.Fgdc.Csdgm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Test.Main
{
    [TestFixture]
    public class Model_Test
    {
        private string originalWorkingDir;
        private TextWriter originalUIwriter;
        private RasterDriverManager rasterDriverMgr;
        private Model model;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            originalWorkingDir = Environment.CurrentDirectory;

            originalUIwriter = UI.TextWriter;
            UI.TextWriter = Data.Output;

            PlugInDataset plugIns = new PlugInDataset();
            plugIns.AddPlugIn("Null Succession", "succession",
                              typeof(Null.Succession).AssemblyQualifiedName);
            plugIns.AddPlugIn("Null Output", "output",
                              typeof(Null.Output).AssemblyQualifiedName);

            rasterDriverMgr = new RasterDriverManager();
            model = new Model(plugIns, rasterDriverMgr);
        }

        //---------------------------------------------------------------------

        private void TryRun(string runFolder)
        {
            try {
                string runFolderPath = Data.MakeInputPath(Path.Combine("model-run",
                                                                       runFolder));
                Environment.CurrentDirectory = runFolderPath;
                Data.Output.WriteLine();
                Data.Output.WriteLine("Current directory: {0}",
                                      runFolderPath.Replace(Data.Directory, Data.DirPlaceholder));
                model.Run("scenario.txt");
            }
            catch (System.Exception e) {
                Data.Output.WriteLine(e.Message);
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ScenarioFile_NotExist()
        {
            TryRun("ScenarioFile_NotExist");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void SpeciesFile_NotExist()
        {
            TryRun("SpeciesFile_NotExist");
        }

        //---------------------------------------------------------------------

        private float ComputeCellArea(float cellLength)
        {
            return (float) (cellLength * cellLength / 10000);
        }

        //---------------------------------------------------------------------

        private void AssertCellLength(float cellLength)
        {
            PlugIns.ICore core = model as PlugIns.ICore;
            Assert.AreEqual(cellLength, core.CellLength);
            Assert.AreEqual(ComputeCellArea(cellLength), core.CellArea);
        }

        //---------------------------------------------------------------------

        private Metadata NewRasterMetadataForDriverManager()
        {
            Metadata metadata = new Metadata();
            rasterDriverMgr.RasterMetadata = metadata;
            return metadata;
        }

        //---------------------------------------------------------------------

        private void SetAbscissaAndOrdinateRes(Metadata metadata,
                                               float    length)
        {
            metadata[AbscissaResolution.Name] = length;
            metadata[OrdinateResolution.Name] = length;
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void CellLength_WidthNotEqualHeight()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            metadata[AbscissaResolution.Name] = 25.0f;
            metadata[OrdinateResolution.Name] = 20.0f;

            TryRun("CellLength_WidthNot=Height");
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_WidthNotEqualHeight_Warn()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            metadata[AbscissaResolution.Name] = 25.0f;
            metadata[OrdinateResolution.Name] = 20.0f;

            TryRun("CellLength_WidthNot=Height_Warn");
            AssertCellLength(30);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void CellLength_Zero()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 0);

            TryRun("CellLength_Zero");
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_Zero_Warn()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 0);

            TryRun("CellLength_Zero_Warn");
            AssertCellLength(30);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void CellLength_Negative()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, -12345);

            TryRun("CellLength_Negative");
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_Negative_Warn()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, -12345);

            TryRun("CellLength_Negative_Warn");
            AssertCellLength(30);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_NoUnits()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 25);

            TryRun("CellLength_NoUnits");
            AssertCellLength(25);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_Map25m()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 25);
            metadata[PlanarDistanceUnits.Name] = PlanarDistanceUnits.Meters;

            TryRun("CellLength_Map25m");
            AssertCellLength(25);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_MapAndScenarioSame()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 25);
            metadata[PlanarDistanceUnits.Name] = PlanarDistanceUnits.Meters;

            TryRun("CellLength_MapAndScenarioSame");
            AssertCellLength(25);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_MapAndScenarioDiffer()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 25);
            metadata[PlanarDistanceUnits.Name] = PlanarDistanceUnits.Meters;

            TryRun("CellLength_MapAndScenarioDiffer");
            AssertCellLength(35);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CellLength_Map100ft()
        {
            Metadata metadata = NewRasterMetadataForDriverManager();
            SetAbscissaAndOrdinateRes(metadata, 100);
            metadata[PlanarDistanceUnits.Name] = PlanarDistanceUnits.SurveyFeet;

            TryRun("CellLength_Map100ft");
            float cellLength = (float) (100.0 * 1200.0 / 3937);
            AssertCellLength(cellLength);
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            UI.TextWriter = originalUIwriter;
            Environment.CurrentDirectory = originalWorkingDir;
        }
    }
}
