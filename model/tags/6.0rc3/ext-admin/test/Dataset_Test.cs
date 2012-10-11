using Landis.PlugIns.Admin;
using NUnit.Framework;
using System.IO;

namespace Landis.Test.PlugIns.Admin
{
    [TestFixture]
    public class Dataset_Test
    {
    	private ExtensionInfo fooExtension;
    	private ExtensionInfo barExtension;
    	private const string twoPlugInDatasetFilename = "TwoPlugIns.xml";
    	private string twoPlugInDatasetPath;
    	private Dataset eventDataset;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
        	fooExtension = new ExtensionInfo(
				"Foo Plug-in",	            // name
        	    null,                       // version
        	    "succession",               // type
        	    "Com.Acme.Foo",             // assembly name
        	    "Com.Acme.Foo.PlugIn",      // class name
        	    "",                         // description
        	    null,                       // user guide path
        	    new System.Version("5.1")   // core version
            );

        	barExtension = new ExtensionInfo(
				"Bar Extension",	    // name
        	    null,                   // version
        	    "disturbance:bar",      // type
        	    "Com.Acme.Bar",         // assembly name
        	    "Com.Acme.Bar.PlugIn",  // class name
        	    "",                     // description
        	    null,                   // user guide path
        	    null                    // core version
            );

        	twoPlugInDatasetPath = Data.MakeInputPath(twoPlugInDatasetFilename);
        	
        	Dataset.SavedEvent += SavedEventHandler;
        }

        //---------------------------------------------------------------------

        private void SavedEventHandler(Dataset dataset)
        {
            eventDataset = dataset;
        }

        //---------------------------------------------------------------------

        [Test]
        public void DefaultCtor()
        {
        	Dataset dataset = new Dataset();
        	Assert.AreEqual(0, dataset.Count);
        	Assert.AreEqual(Dataset.DefaultPath, dataset.Path);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void LoadOrCreate_Null()
        {
        	Dataset dataset = Dataset.LoadOrCreate(null);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void LoadOrCreate_Empty()
        {
        	Dataset dataset = Dataset.LoadOrCreate("");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void LoadOrCreate_Whitespace()
        {
        	Dataset dataset = Dataset.LoadOrCreate("  ");
        }

        //---------------------------------------------------------------------

        [Test]
        public void LoadOrCreate_Create()
        {
        	string path = Data.MakeOutputPath("LoadOrCreate_Create.xml");
        	if (File.Exists(path))
        		File.Delete(path);
        	Assert.IsFalse(File.Exists(path));

        	Dataset dataset = Dataset.LoadOrCreate(path);
        	Assert.IsNotNull(dataset);
        	Assert.AreEqual(0, dataset.Count);
        	Assert.AreEqual(path, dataset.Path);
        	Assert.IsTrue(File.Exists(path));

        	//  Confirm that the new file can be loaded separately
        	Dataset loadedDataset = new Dataset(path);
        	Assert.IsNotNull(loadedDataset);
        	Assert.AreEqual(0, loadedDataset.Count);
        	Assert.AreEqual(path, loadedDataset.Path);
        }

        //---------------------------------------------------------------------

        [Test]
        public void LoadOrCreate_Load()
        {
        	Assert.IsTrue(File.Exists(twoPlugInDatasetPath));
        	Dataset dataset = Dataset.LoadOrCreate(twoPlugInDatasetPath);

        	Assert.IsTrue(File.Exists(twoPlugInDatasetPath));
        	ValidateTwoPlugIns(dataset);
        }

        //---------------------------------------------------------------------

        private void TryOpenDataset(string filename)
        {
            string path;
            string pathDisplay;
            if (string.IsNullOrEmpty(filename)) {
                path = filename;
                if (filename == null)
                    pathDisplay = "(null)";
                else
                    pathDisplay = "";
            }
            else {
                path = Data.MakeInputPath(filename);
                pathDisplay = path.Replace(Data.Directory,
                                           Data.DirPlaceholder);
            }
            try {
                Data.Output.WriteLine();
                Data.Output.WriteLine("Reading the plug-in database \"{0}\" ...", pathDisplay);
                Dataset dataset = new Dataset(path);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine("Error: {0}",
                                      exc.Message.Replace(Data.Directory,
                                                          Data.DirPlaceholder));
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void NullPath()
        {
            TryOpenDataset(null);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void EmptyPath()
        {
            TryOpenDataset("");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void NonExistentPath()
        {
            TryOpenDataset("NonExistentFile");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void EmptyFile()
        {
            TryOpenDataset("EmptyFile.xml");
        }

        //---------------------------------------------------------------------

        [Test]
        public void Add()
        {
            Dataset dataset = new Dataset();
            Assert.AreEqual(0, dataset.Count);

            dataset.Add(fooExtension);
            Assert.AreEqual(1, dataset.Count);
            Assert.AreEqual(fooExtension, dataset[0]);
            Assert.AreEqual(fooExtension, dataset[fooExtension.Name]);

            dataset.Add(barExtension);
            Assert.AreEqual(2, dataset.Count);
            Assert.AreEqual(barExtension, dataset[1]);
            Assert.AreEqual(barExtension, dataset[barExtension.Name]);
        }

        //---------------------------------------------------------------------

        [Test]
        public void IDataset_Indexer()
        {
            Dataset dataset = new Dataset();
            dataset.Add(fooExtension);
            dataset.Add(barExtension);

            Landis.PlugIns.IDataset coreDataset = dataset;
            AssertAreEqual(fooExtension.CoreInfo, coreDataset[fooExtension.Name]);
            AssertAreEqual(barExtension.CoreInfo, coreDataset[barExtension.Name]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(Landis.PlugIns.PlugInInfo expected,
                                    Landis.PlugIns.PlugInInfo actual)
        {
        	Assert.IsNotNull(actual);
        	Assert.AreEqual(expected.Name,               actual.Name);
        	Assert.AreEqual(expected.PlugInType.Name,    actual.PlugInType.Name);
        	Assert.AreEqual(expected.ImplementationName, actual.ImplementationName);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void AddNameTwice()
        {
            Dataset dataset = new Dataset();
            Assert.AreEqual(0, dataset.Count);

            dataset.Add(fooExtension);
            Assert.AreEqual(1, dataset.Count);
            Assert.AreEqual(fooExtension, dataset[0]);
            Assert.AreEqual(fooExtension, dataset[fooExtension.Name]);

            dataset.Add(fooExtension);
        }

        //---------------------------------------------------------------------

        private Dataset LoadDataset(string filename)
        {
            string path = Data.MakeInputPath(filename);
            Data.Output.WriteLine();
            Data.Output.WriteLine("Opening file \"{0}\" ...",
                                  path.Replace(Data.Directory,
                                               Data.DirPlaceholder));
            return new Dataset(path);
        }

        //---------------------------------------------------------------------

        [Test]
        public void NoPlugIns()
        {
        	const string filename = "NoPlugIns.xml";
            Dataset dataset = LoadDataset(filename);
            Assert.AreEqual(0, dataset.Count);
            Assert.AreEqual(Data.MakeInputPath(filename), dataset.Path);
        }

        //---------------------------------------------------------------------

        [Test]
        public void TwoPlugIns()
        {
            Dataset dataset = LoadDataset(twoPlugInDatasetFilename);
            ValidateTwoPlugIns(dataset);
        }

        //---------------------------------------------------------------------

        private void ValidateTwoPlugIns(Dataset dataset)
        {
        	Assert.IsNotNull(dataset);
            Assert.AreEqual(2, dataset.Count);
            Assert.AreEqual(twoPlugInDatasetPath, dataset.Path);

            ExtensionInfo info = dataset["Foo"];
            Assert.IsNotNull(info);
            Assert.AreEqual("Foo", info.Name);
            Assert.AreEqual("disturbance:foo", info.Type);
            Assert.AreEqual("Com.Foo.PlugIn", info.ClassName);
            Assert.AreEqual("Com.Foo", info.AssemblyName);

            info = dataset["Bar Extension"];
            Assert.IsNotNull(info);
            Assert.AreEqual("Bar Extension", info.Name);
            Assert.AreEqual("output", info.Type);
            Assert.AreEqual("Org.Bar.PlugIn", info.ClassName);
            Assert.AreEqual("Org.Bar", info.AssemblyName);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void Remove_Null()
        {
        	Dataset dataset = new Dataset();
        	dataset.Remove(null);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Remove_Whitespace()
        {
        	Dataset dataset = new Dataset();
        	dataset.Remove(" ");
        }

        //---------------------------------------------------------------------

        [Test]
        public void Remove_UnknownName()
        {
        	Dataset dataset = new Dataset();
        	dataset.Add(fooExtension);
        	dataset.Add(barExtension);

        	Assert.IsNull(dataset.Remove("Unknown Plug-In"));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Remove()
        {
        	Dataset dataset = new Dataset();
        	dataset.Add(fooExtension);
        	dataset.Add(barExtension);

        	ExtensionInfo removedExtension = dataset.Remove(fooExtension.Name);
        	AssertAreEqual(fooExtension, removedExtension);
        	Assert.AreEqual(1, dataset.Count);
        	AssertAreEqual(barExtension, dataset[0]);

        	removedExtension = dataset.Remove(barExtension.Name);
        	AssertAreEqual(barExtension, removedExtension);
        	Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SaveAs()
        {
        	Dataset dataset = new Dataset();
        	dataset.Add(fooExtension);
        	dataset.Add(barExtension);

        	eventDataset = null;
        	string path = Data.MakeOutputPath("SaveAs_Test.xml");
        	dataset.SaveAs(path);
        	Assert.AreEqual(path, dataset.Path);
        	Assert.AreSame(dataset, eventDataset);

        	Dataset loadedDataset = new Dataset(path);
        	Assert.IsNotNull(loadedDataset);
        	AssertAreEqual(dataset, loadedDataset);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Save()
        {
        	Dataset dataset = new Dataset();
        	string path = Data.MakeOutputPath("Save_Test.xml");
        	dataset.SaveAs(path);
        	Assert.AreEqual(path, dataset.Path);

        	dataset.Add(barExtension);
        	dataset.Add(fooExtension);

        	eventDataset = null;
        	dataset.Save();
        	Assert.AreEqual(path, dataset.Path);
        	Assert.AreSame(dataset, eventDataset);

        	Dataset loadedDataset = new Dataset(path);
        	Assert.IsNotNull(loadedDataset);
        	AssertAreEqual(dataset, loadedDataset);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(Dataset expected,
                                    Dataset actual)
        {
        	Assert.AreEqual(expected.Count, actual.Count);
        	for (int i = 0; i < expected.Count; i++) {
        		ExtensionInfo expectedInfo = expected[i];
        		ExtensionInfo actualInfo = actual[expectedInfo.Name];
        		AssertAreEqual(expectedInfo, actualInfo);
        	}
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(ExtensionInfo expected,
                                    ExtensionInfo actual)
        {
        	Assert.IsNotNull(actual);

        	Assert.AreEqual(expected.Name,          actual.Name);
        	Assert.AreEqual(expected.Type,          actual.Type);
        	Assert.AreEqual(expected.Version,       actual.Version);
        	Assert.AreEqual(expected.AssemblyName,  actual.AssemblyName);
        	Assert.AreEqual(expected.ClassName,     actual.ClassName);
        	Assert.AreEqual(expected.Description,   actual.Description);
        	Assert.AreEqual(expected.UserGuidePath, actual.UserGuidePath);
        	
        	//  Core version is not checked because it is not stored in the
        	//  dataset.
        }
    }
}
