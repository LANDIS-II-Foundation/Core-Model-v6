using Landis.Core;
using Landis.Species;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Species
{
    [TestFixture]
    public class DatasetParser_Test
    {
        private DatasetParser parser;
        private LineReader reader;
        private StringReader currentLine;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            Data.InitializeDirectory(ModuleData.GetRelativePath("species"));
            parser = new DatasetParser();
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = Data.MakeInputPath(filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename,
                              int    errorLineNum)
        {
            try {
                reader = OpenFile(filename);
                // This method is only called on bad files, so we expect the
                // statement below to throw an exception.  Since we knowingly
                // ignore the variable "dataset", disable the CS0219 warning
                // "The variable '...' is assigned but its value is never used'.
#pragma warning disable 0219
                ISpeciesDataset dataset = parser.Parse(reader);
#pragma warning restore 0219
            }
            catch (System.Exception e) {
                Data.Output.WriteLine(e.Message.Replace(Data.Directory,
                                                        Data.DirPlaceholder));
                Data.Output.WriteLine();
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

        private ISpeciesDataset ParseFile(string filename)
        {
            reader = OpenFile(filename);
            ISpeciesDataset dataset = parser.Parse(reader);
            reader.Close();
            return dataset;
        }

        //---------------------------------------------------------------------

        [Test]
        public void EmptyTable()
        {
            ISpeciesDataset dataset = ParseFile("EmptyTable.txt");
            Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        private void CompareDatasetAndFile(ISpeciesDataset dataset,
                                           string filename)
        {
            FileLineReader file = OpenFile(filename);
            InputLine inputLine = new InputLine(file);

            InputVar<string> LandisData = new InputVar<string>(Landis.Data.InputVarName);
            inputLine.ReadVar(LandisData);

            int expectedIndex = 0;
            foreach (ISpecies species in dataset) {
                Assert.AreEqual(expectedIndex, species.Index);
                expectedIndex++;

                Assert.IsTrue(inputLine.GetNext());
                currentLine = new StringReader(inputLine.ToString());

                Assert.AreEqual(ReadValue<string>(),               species.Name);
                Assert.AreEqual(ReadValue<int>(),                  species.Longevity);
                Assert.AreEqual(ReadValue<int>(),                  species.Maturity);
                Assert.AreEqual(ReadValue<byte>(),                 species.ShadeTolerance);
                Assert.AreEqual(ReadValue<byte>(),                 species.FireTolerance);
                Assert.AreEqual(ReadEffectiveSeedDist(),           species.EffectiveSeedDist);
                Assert.AreEqual(ReadValue<int>(),                  species.MaxSeedDist);
                Assert.AreEqual(ReadValue<float>(),                species.VegReprodProb);
                Assert.AreEqual(ReadValue<int>(),                  species.MinSproutAge);
                Assert.AreEqual(ReadValue<int>(),                  species.MaxSproutAge);
                Assert.AreEqual(ReadValue<PostFireRegeneration>(), species.PostFireRegeneration);
            }
            Assert.IsFalse(inputLine.GetNext());
            file.Close();
        }

        //---------------------------------------------------------------------

        private T ReadValue<T>()
        {
            ReadMethod<T> method = InputValues.GetReadMethod<T>();
            int index;
            return method(currentLine, out index);
        }

        //---------------------------------------------------------------------

        private int ReadEffectiveSeedDist()
        {
            int index;
            return EffectiveSeedDist.ReadMethod(currentLine, out index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void FullTable()
        {
            string filename = "FullTable.txt";
            ISpeciesDataset dataset = ParseFile(filename);
            CompareDatasetAndFile(dataset, filename);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void NameRepeated()
        {
            TryParse("NameRepeated.txt", 21);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LongevityMissing()
        {
            TryParse("LongevityMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LongevityInvalid()
        {
            TryParse("LongevityInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LongevityNegative()
        {
            TryParse("LongevityNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaturityMissing()
        {
            TryParse("MaturityMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaturityInvalid()
        {
            TryParse("MaturityInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaturityNegative()
        {
            TryParse("MaturityNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaturityTooBig()
        {
            TryParse("MaturityTooBig.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ShadeMissing()
        {
            TryParse("ShadeMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ShadeInvalid()
        {
            TryParse("ShadeInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ShadeNegative()
        {
            TryParse("ShadeNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ShadeZero()
        {
            TryParse("ShadeZero.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ShadeTooBig()
        {
            TryParse("ShadeTooBig.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void FireMissing()
        {
            TryParse("FireMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void FireInvalid()
        {
            TryParse("FireInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void FireNegative()
        {
            TryParse("FireNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void FireZero()
        {
            TryParse("FireZero.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void FireTooBig()
        {
            TryParse("FireTooBig.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EffSeedMissing()
        {
            TryParse("EffSeedMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EffSeedInvalid()
        {
            TryParse("EffSeedInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EffSeedZero()
        {
            TryParse("EffSeedZero.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void EffSeedNegative()
        {
            TryParse("EffSeedNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSeedMissing()
        {
            TryParse("MaxSeedMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSeedNegative()
        {
            TryParse("MaxSeedNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSeedLessThanEff()
        {
            TryParse("MaxSeedLessThanEff.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ReprodProbMissing()
        {
            TryParse("ReprodProbMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ReprodProbNegative()
        {
            TryParse("ReprodProbNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ReprodProbTooBig()
        {
            TryParse("ReprodProbTooBig.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinSproutMissing()
        {
            TryParse("MinSproutMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinSproutNegative()
        {
            TryParse("MinSproutNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MinSproutMoreThanLongevity()
        {
            TryParse("MinSproutMoreThanLongevity.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSproutMissing()
        {
            TryParse("MaxSproutMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSproutNegative()
        {
            TryParse("MaxSproutNegative.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSproutMoreThanLongevity()
        {
            TryParse("MaxSproutMoreThanLongevity.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void MaxSproutLessThanMin()
        {
            TryParse("MaxSproutLessThanMin.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PostFireRegenMissing()
        {
            TryParse("PostFireRegenMissing.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void PostFireRegenInvalid()
        {
            TryParse("PostFireRegenInvalid.txt", 9);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ExtraText()
        {
            TryParse("ExtraText.txt", 9);
        }
    }
}
