using Landis.Core;
using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Ecoregions
{
    [TestFixture]
    public class Dataset_Test
    {
        private List<IEcoregionParameters> ecoregionParms;
        private Dataset dataset;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            ecoregionParms = new List<IEcoregionParameters>();
            ecoregionParms.Add(new Parameters("eco1", "Ecoregion 1", 3,     true));
            ecoregionParms.Add(new Parameters("eco2", "Ecoregion 2", 22222, false));
            ecoregionParms.Add(new Parameters("eco9", "Ecoregion 9", 9,     true));

            dataset = new Dataset(ecoregionParms);
        }

        //---------------------------------------------------------------------

        [Test]
        public void EmptyDataset()
        {
            Dataset emptyDataset = new Dataset(new List<IEcoregionParameters>());
            Assert.AreEqual(0, emptyDataset.Count);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Count()
        {
            Assert.AreEqual(ecoregionParms.Count, dataset.Count);
        }

        //---------------------------------------------------------------------

        private void CheckEcoregion(IEcoregion ecoregion,
                                    int        expectedIndex)
        {
            Assert.IsNotNull(ecoregion);
            Assert.AreEqual(expectedIndex, ecoregion.Index);

            IEcoregionParameters expectedParms = ecoregionParms[expectedIndex];
            Assert.AreEqual(expectedParms.Name, ecoregion.Name);
            Assert.AreEqual(expectedParms.Description, ecoregion.Description);
            Assert.AreEqual(expectedParms.MapCode, ecoregion.MapCode);
            Assert.AreEqual(expectedParms.Active, ecoregion.Active);
        }

        //---------------------------------------------------------------------

        [Test]
        public void IndexerWithInt()
        {
            for (int index = 0; index < ecoregionParms.Count; ++index) {
                IEcoregion ecoregion = dataset[index];
                CheckEcoregion(ecoregion, index);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void IndexerWithNegativeInt()
        {
            // We expect the statement below to raise an exception, so disable
            // the CS0219 warning:
            // "The variable '...' is assigned but its value is never used'.
#pragma warning disable 0219
            IEcoregion ecoregion = dataset[-1];
#pragma warning restore 0219
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void IndexerWithTooBigInt()
        {
            // We expect the statement below to raise an exception, so disable
            // the CS0219 warning:
            // "The variable '...' is assigned but its value is never used'.
#pragma warning disable 0219
            IEcoregion ecoregion = dataset[dataset.Count];
#pragma warning restore 0219
        }

        //---------------------------------------------------------------------

        [Test]
        public void IndexerWithName()
        {
            for (int index = 0; index < ecoregionParms.Count; ++index) {
                IEcoregion ecoregion = dataset[ecoregionParms[index].Name];
                CheckEcoregion(ecoregion, index);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void IndexerWithUnknownName()
        {
            Assert.IsNull(dataset["unknown-ecoregion"]);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Find()
        {
            for (int index = 0; index < ecoregionParms.Count; ++index) {
                IEcoregion ecoregion = dataset.Find(ecoregionParms[index].MapCode);
                CheckEcoregion(ecoregion, index);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Find_BadMapCode()
        {
            Assert.IsNull(dataset.Find(ushort.MinValue));
            Assert.IsNull(dataset.Find(ushort.MaxValue));
        }
    }
}
