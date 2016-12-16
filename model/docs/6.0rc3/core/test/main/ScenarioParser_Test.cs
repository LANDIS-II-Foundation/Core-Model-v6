using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Main
{
    [TestFixture]
    public class ScenarioParser_Test
    {
        private ScenarioParser parser;
        private LineReader reader;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            PlugInDataset plugIns = new PlugInDataset();
            plugIns.AddPlugIn("Age-only succession", "succession", null);

            plugIns.AddPlugIn("Null.Disturbance", "disturbance", null);
            plugIns.AddPlugIn("Age-only.Wind",    "disturbance:wind", null);
            plugIns.AddPlugIn("Age-only.Fire",    "disturbance:fire", null);
            plugIns.AddPlugIn("Harvest",          "disturbance:harvest", null);

            plugIns.AddPlugIn("Test.DumpEcoregions", "output", null);
            plugIns.AddPlugIn("Test.DumpSpecies",    "output", null);

            parser = new ScenarioParser(plugIns);
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = System.IO.Path.Combine(Data.Directory, filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename,
                              int    errorLineNum)
        {
            try {
                reader = OpenFile(filename);
                Scenario scenario = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null)
                    Assert.AreEqual(errorLineNum, lrExc.LineNumber);
                throw;
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Empty()
        {
            TryParse("empty.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongName()
        {
            TryParse("LandisData-WrongName.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_NoValue()
        {
            TryParse("LandisData-NoValue.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_MissingQuote()
        {
            TryParse("LandisData-MissingQuote.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongValue()
        {
            TryParse("LandisData-WrongValue.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_Missing()
        {
            TryParse("Duration-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_WrongName()
        {
            TryParse("Duration-WrongName.txt", 6);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_MissingValue()
        {
            TryParse("Duration-MissingValue.txt", 6);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_BadValue()
        {
            TryParse("Duration-BadValue.txt", 6);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_Negative()
        {
            TryParse("Duration-Negative.txt", 6);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Duration_Extra()
        {
            TryParse("Duration-Extra.txt", 6);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_Missing()
        {
            TryParse("Species-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_WrongName()
        {
            TryParse("Species-WrongName.txt", 8);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_MissingValue()
        {
            TryParse("Species-MissingValue.txt", 8);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_Empty()
        {
            TryParse("Species-Empty.txt", 8);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_Whitespace()
        {
            TryParse("Species-Whitespace.txt", 8);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Species_Extra()
        {
            TryParse("Species-Extra.txt", 8);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_Missing()
        {
            TryParse("Ecoregions-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_WrongName()
        {
            TryParse("Ecoregions-WrongName.txt", 10);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_MissingValue()
        {
            TryParse("Ecoregions-MissingValue.txt", 10);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_Empty()
        {
            TryParse("Ecoregions-Empty.txt", 10);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_Whitespace()
        {
            TryParse("Ecoregions-Whitespace.txt", 10);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Ecoregions_Extra()
        {
            TryParse("Ecoregions-Extra.txt", 10);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_Missing()
        {
            TryParse("EcoregionsMap-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_WrongName()
        {
            TryParse("EcoregionsMap-WrongName.txt", 11);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_MissingValue()
        {
            TryParse("EcoregionsMap-MissingValue.txt", 11);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_Empty()
        {
            TryParse("EcoregionsMap-Empty.txt", 11);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_Whitespace()
        {
            TryParse("EcoregionsMap-Whitespace.txt", 11);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EcoregionsMap_Extra()
        {
            TryParse("EcoregionsMap-Extra.txt", 11);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_WrongName()
        {
            TryParse("CellLength-WrongName.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_MissingValue()
        {
            TryParse("CellLength-MissingValue.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_BadValue()
        {
            TryParse("CellLength-BadValue.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_Zero()
        {
            TryParse("CellLength-Zero.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_Negative()
        {
            TryParse("CellLength-Negative.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CellLength_Extra()
        {
            TryParse("CellLength-Extra.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_Missing()
        {
            TryParse("InitCommunities-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_WrongName()
        {
            TryParse("InitCommunities-WrongName.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_MissingValue()
        {
            TryParse("InitCommunities-MissingValue.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_Empty()
        {
            TryParse("InitCommunities-Empty.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_Whitespace()
        {
            TryParse("InitCommunities-Whitespace.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunities_Extra()
        {
            TryParse("InitCommunities-Extra.txt", 13);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_Missing()
        {
            TryParse("InitCommunitiesMap-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_WrongName()
        {
            TryParse("InitCommunitiesMap-WrongName.txt", 14);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_MissingValue()
        {
            TryParse("InitCommunitiesMap-MissingValue.txt", 14);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_Empty()
        {
            TryParse("InitCommunitiesMap-Empty.txt", 14);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_Whitespace()
        {
            TryParse("InitCommunitiesMap-Whitespace.txt", 14);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void InitCommunitiesMap_Extra()
        {
            TryParse("InitCommunitiesMap-Extra.txt", 14);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_Missing()
        {
            TryParse("Succession-Missing.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_BadString()
        {
            TryParse("Succession-BadString.txt", 20);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_InvalidName()
        {
            TryParse("Succession-InvalidName.txt", 20);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_Unknown()
        {
            TryParse("Succession-Unknown.txt", 20);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_MissingFile()
        {
            TryParse("Succession-MissingFile.txt", 20);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Succession_Extra()
        {
            TryParse("Succession-Extra.txt", 20);
        }

        //---------------------------------------------------------------------

        private Scenario ParseFile(string filename)
        {
            reader = OpenFile(filename);
            Scenario scenario = parser.Parse(reader);
            reader.Close();
            return scenario;
        }

        //---------------------------------------------------------------------

        [Test]
        public void Disturbances_Zero()
        {
            Scenario scenario = ParseFile("Disturbances-Zero.txt");
            Assert.IsFalse(scenario.CellLength.HasValue);
            Assert.AreEqual(0, scenario.Disturbances.Length);
            Assert.IsFalse(scenario.DisturbancesRandomOrder);
            Assert.AreEqual(2, scenario.OtherPlugIns.Length);
            Assert.IsFalse(scenario.RandomNumberSeed.HasValue);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Disturbances_ZeroRandom()
        {
            Scenario scenario = ParseFile("Disturbances-ZeroRandom.txt");
            Assert.IsFalse(scenario.CellLength.HasValue);
            Assert.AreEqual(0, scenario.Disturbances.Length);
            Assert.IsTrue(scenario.DisturbancesRandomOrder);
            Assert.AreEqual(2, scenario.OtherPlugIns.Length);
            Assert.IsFalse(scenario.RandomNumberSeed.HasValue);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Disturbances_SuccessionName()
        {
            TryParse("Disturbances-SuccessionName.txt", 24);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Disturbances_RepeatedName()
        {
            TryParse("Disturbances-RepeatedName.txt", 27);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void OtherPlugIns_DisturbanceName()
        {
            TryParse("OtherPlugIns-DisturbanceName.txt", 26);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void OtherPlugIns_RepeatedName()
        {
            TryParse("OtherPlugIns-RepeatedName.txt", 30);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyPlugIns()
        {
            Scenario scenario = ParseFile("ManyPlugIns.txt");
            CheckManyPlugIns(scenario, false, null, null);
        }

        //---------------------------------------------------------------------

        private void CheckManyPlugIns(Scenario scenario,
                                      bool     expectedDisturbRandomOrder,
                                      float?   expectedCellLength,
                                      uint?    expectedSeed)
        {
            Assert.AreEqual(expectedCellLength, scenario.CellLength);
            Assert.AreEqual(4, scenario.Disturbances.Length);
            Assert.AreEqual(expectedDisturbRandomOrder,
                            scenario.DisturbancesRandomOrder);
            Assert.AreEqual(2, scenario.OtherPlugIns.Length);
            Assert.AreEqual(expectedSeed, scenario.RandomNumberSeed);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyPlugIns_Random()
        {
            Scenario scenario = ParseFile("ManyPlugIns-Random.txt");
            CheckManyPlugIns(scenario, true, null, null);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyPlugIns_CellLength()
        {
            Scenario scenario = ParseFile("ManyPlugIns-CellLength.txt");
            CheckManyPlugIns(scenario, false, 25.0f, null);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyPlugIns_RandomNumberSeed()
        {
            Scenario scenario = ParseFile("ManyPlugIns-RandomNumberSeed.txt");
            CheckManyPlugIns(scenario, false, 25.0f, (uint) 1234567);
        }

        //---------------------------------------------------------------------

        [Test]
        public void RandomNumberSeed_Missing_NoOther()
        {
            Scenario scenario = ParseFile("RandomNumberSeed-Missing-NoOther.txt");
            CheckNoOtherPlugIns(scenario, false, null);
        }

        //---------------------------------------------------------------------

        private void CheckNoOtherPlugIns(Scenario scenario,
                                         bool     expectedDisturbRandomOrder,
                                         uint?    expectedSeed)
        {
            Assert.AreEqual((float?) 25.0f, scenario.CellLength);
            Assert.AreEqual(2, scenario.Disturbances.Length);
            Assert.AreEqual(expectedDisturbRandomOrder,
                            scenario.DisturbancesRandomOrder);
            Assert.AreEqual(0, scenario.OtherPlugIns.Length);
            Assert.AreEqual(expectedSeed, scenario.RandomNumberSeed);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void RandomNumberSeed_MissingValue()
        {
            TryParse("RandomNumberSeed-MissingValue.txt", 26);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void RandomNumberSeed_Zero()
        {
            TryParse("RandomNumberSeed-Zero.txt", 26);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void RandomNumberSeed_TooBig()
        {
            TryParse("RandomNumberSeed-TooBig.txt", 26);
        }

        //---------------------------------------------------------------------

        [Test]
        public void RandomNumberSeed_NoOther()
        {
            Scenario scenario = ParseFile("RandomNumberSeed-NoOther.txt");
            CheckNoOtherPlugIns(scenario, false, (uint) 4357);
        }

        //---------------------------------------------------------------------

        [Test]
        public void RandomNumberSeed_DisturbRandomOrder_NoOther()
        {
            Scenario scenario = ParseFile("RandomNumberSeed-DisturbRandomOrder-NoOther.txt");
            CheckNoOtherPlugIns(scenario, true, (uint) 4357);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ExtraAfterLastParm()
        {
            TryParse("ExtraAfterLastParm.txt", 28);
        }
    }
}
