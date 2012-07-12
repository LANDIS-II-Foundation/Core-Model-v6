using Landis.PlugIns;
using Landis.PlugIns.Admin;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.PlugIns.Admin
{
    [TestFixture]
    public class PersistentDataset_Test
    {
    	private PersistentDataset.PlugInInfo fooPlugIn;
    	private PersistentDataset.PlugInInfo partialPlugIn;

        //---------------------------------------------------------------------

    	[TestFixtureSetUp]
    	public void Init()
    	{
            fooPlugIn = new PersistentDataset.PlugInInfo();
            fooPlugIn.Name = "Foo Plug-in";
            fooPlugIn.Version = "3.10";
            fooPlugIn.TypeName = "succession";
            fooPlugIn.AssemblyName = "Org.Bar.Foo";
            fooPlugIn.ClassName = "Org.Bar.Foo.PlugIn";
            fooPlugIn.UserGuidePath = "Foo PlugIn User Guide.pdf";

            partialPlugIn = new PersistentDataset.PlugInInfo();
            partialPlugIn.Name = "Partial Plug-in";
            // No version
            partialPlugIn.TypeName = "disturbance:wind";
            partialPlugIn.AssemblyName = "Com.Acme";
            partialPlugIn.ClassName = "Com.Acme.PlugIn";
            // No user guide
    	}

        //---------------------------------------------------------------------

        private void CreateSaveLoadAndCompareDataset(string                                filename,
                                                     params PersistentDataset.PlugInInfo[] plugIns)
        {
            PersistentDataset dataset = new PersistentDataset();
            foreach (PersistentDataset.PlugInInfo plugIn in plugIns)
            	dataset.PlugIns.Add(plugIn);

            string path = Data.MakeOutputPath(filename);
            dataset.Save(path);

            PersistentDataset dataset2;
            dataset2 = PersistentDataset.Load(path);

            AssertAreEqual(dataset, dataset2);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ZeroPlugIns()
        {
        	CreateSaveLoadAndCompareDataset("ZeroPlugIns.xml");
        }

        //---------------------------------------------------------------------

        [Test]
        public void OnePlugIn()
        {
        	CreateSaveLoadAndCompareDataset("OnePlugIn.xml", fooPlugIn);
        }

        //---------------------------------------------------------------------

        [Test]
        public void TwoPlugIns()
        {
        	CreateSaveLoadAndCompareDataset("TwoPlugIns.xml", fooPlugIn,
        	                                partialPlugIn);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDataset expected,
                                    PersistentDataset actual)
        {
            Assert.IsNotNull(actual);
            AssertAreEqual(expected.PlugIns, actual.PlugIns);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(List<PersistentDataset.PlugInInfo> expected,
                                    List<PersistentDataset.PlugInInfo> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                AssertAreEqual(expected[i], actual[i]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDataset.PlugInInfo expected,
                                    PersistentDataset.PlugInInfo actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
            Assert.AreEqual(expected.AssemblyName, actual.AssemblyName);
            Assert.AreEqual(expected.ClassName, actual.ClassName);
            Assert.AreEqual(expected.UserGuidePath, actual.UserGuidePath);
        }
    }
}
