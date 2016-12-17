using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Ecoregions;
using Landis.SpatialModeling;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Ecoregions
{
    [TestFixture]
    public class Map_Test
    {
        private Dataset dataset;
        private RasterFactory rasterFactory;
        private byte[,] ecoregions8Bit;
        private Dimensions dims8Bit;
        private const string path8Bit = "8-bit map";
        private ushort[,] ecoregions16Bit;
        private Dimensions dims16Bit;
        private string path16Bit = "16-bit map";

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            List<IEcoregionParameters> ecoregionParms = new List<IEcoregionParameters>();
            ecoregionParms.Add(new Parameters("eco0", "Ecoregion A", 0, true));
            ecoregionParms.Add(new Parameters("eco11", "Ecoregion B", 11, false));
            ecoregionParms.Add(new Parameters("eco222", "Ecoregion C", 222, true));
            ecoregionParms.Add(new Parameters("eco3333", "Ecoregion D", 3333, false));
            ecoregionParms.Add(new Parameters("eco-65535", "Ecoregion E", 65535, true));

            dataset = new Dataset(ecoregionParms);
            rasterFactory = new RasterFactory();

            //  Initialize 8-bit ecoregion data
            ecoregions8Bit = new byte[,] {
                {   0,   0,  11, 222,  11 },
                {   0,  11,  11, 222,  11 },
                {   0,  11,  11, 222, 222 },
                {  11,  11,  11, 222, 222 },
                {  11,  11, 222, 222, 222 },
                {  11,  11, 222, 222, 222 }
            };
            dims8Bit = new Dimensions(ecoregions8Bit.GetLength(0),
                                      ecoregions8Bit.GetLength(1));
            rasterFactory.SetData(path8Bit, ecoregions8Bit);

            //  Initialize 16-bit ecoregion data
            ecoregions16Bit = new ushort[,] {
                {   0,   0,  11, 222,  11,  3333,     0 },
                {   0,  11,  11, 222,  11,  3333, 65535 },
                {   0,  11,  11, 222, 222,  3333, 65535 },
                {  11,  11,  11, 222, 222,  3333, 65535 },
                {  11,  11, 222, 222, 222,  3333, 65535 },
                {  11,  11, 222, 222, 222, 65535, 65535 },
                {   0,   0, 222, 222, 222, 65535, 65535 }
            };
            dims16Bit = new Dimensions(ecoregions16Bit.GetLength(0),
                                       ecoregions16Bit.GetLength(1));
            rasterFactory.SetData(path16Bit, ecoregions16Bit);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Map8Bit()
        {
            Map map = new Map(path8Bit, dataset, rasterFactory);
            using (IInputGrid<bool> inputGrid = map.OpenAsInputGrid()) {
                Assert.AreEqual(dims8Bit.Rows, inputGrid.Dimensions.Rows);
                Assert.AreEqual(dims8Bit.Columns, inputGrid.Dimensions.Columns);

                for (int row = 0; row < dims8Bit.Rows; ++row) {
                    for (int column = 0; column < dims8Bit.Columns; ++column) {
                        IEcoregion ecoregion = dataset.Find(ecoregions8Bit[row,column]);
                        Assert.AreEqual(ecoregion.Active, inputGrid.ReadValue());
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Map16Bit()
        {
            Map map = new Map(path16Bit, dataset, rasterFactory);
            using (IInputGrid<bool> inputGrid = map.OpenAsInputGrid()) {
                Assert.AreEqual(dims16Bit.Rows, inputGrid.Dimensions.Rows);
                Assert.AreEqual(dims16Bit.Columns, inputGrid.Dimensions.Columns);

                for (int row = 0; row < dims16Bit.Rows; ++row) {
                    for (int column = 0; column < dims16Bit.Columns; ++column) {
                        IEcoregion ecoregion = dataset.Find(ecoregions16Bit[row,column]);
                        Assert.AreEqual(ecoregion.Active, inputGrid.ReadValue());
                    }
                }
            }
        }
    }
}
