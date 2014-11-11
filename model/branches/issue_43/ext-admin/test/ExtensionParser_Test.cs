using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using Landis.PlugIns.Admin;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.PlugIns.Admin
{
    [TestFixture]
    public class ExtensionParser_Test
    {
    	private Dataset dataset;
        private ExtensionParser parser;
        private LineReader reader;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
        	dataset = new Dataset();
        	EditableExtensionInfo.Dataset = dataset;
            parser = new ExtensionParser();
            App.InstallingExtension = false;
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = System.IO.Path.Combine(Data.Directory, filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename)
        {
            int? errorLineNum = Testing.FindErrorMarker(Data.MakeInputPath(filename));
            try {
                reader = OpenFile(filename);
                ExtensionInfo extension = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null && errorLineNum.HasValue)
                    Assert.AreEqual(errorLineNum.Value, lrExc.LineNumber);
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
            TryParse("empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongValue()
        {
            TryParse("LandisData-WrongValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Name_Missing()
        {
            TryParse("Name-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Name_Empty()
        {
            TryParse("Name-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Name_Whitespace()
        {
            TryParse("Name-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Name_InUse()
        {
        	ExtensionInfo fooBarExtension = new ExtensionInfo(
				"Foo Bar",  // name
        	    null,       // version
        	    null,       // type
        	    null,       // assembly name
        	    null,       // class name
        	    null,       // description
        	    null,       // user guide path
        	    null        // core version
            );
        	dataset.Add(fooBarExtension);
        	try {
	            TryParse("Foo-Bar.txt");
        	}
        	finally {
        		dataset.Remove(fooBarExtension.Name);
        	}
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Version_Empty()
        {
            TryParse("Version-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Version_Whitespace()
        {
            TryParse("Version-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Type_Missing()
        {
            TryParse("Type-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Type_Empty()
        {
            TryParse("Type-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Type_Whitespace()
        {
            TryParse("Type-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        public void Description_Missing()
        {
            ExtensionInfo extension = ParseFile("Description-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.IsNull(extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Description_Empty()
        {
            TryParse("Description-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Description_Whitespace()
        {
            TryParse("Description-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        public void UserGuide_Missing()
        {
            ExtensionInfo extension = ParseFile("UserGuide-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.IsNull(extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void UserGuide_Empty()
        {
            TryParse("UserGuide-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void UserGuide_Whitespace()
        {
            TryParse("UserGuide-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Assembly_Missing()
        {
            TryParse("Assembly-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Assembly_Empty()
        {
            TryParse("Assembly-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Assembly_Whitespace()
        {
            TryParse("Assembly-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Class_Missing()
        {
            TryParse("Class-Missing.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Class_Empty()
        {
            TryParse("Class-Empty.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Class_Whitespace()
        {
            TryParse("Class-Whitespace.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreVersion_Missing()
        {
            ExtensionInfo extension = ParseFile("CoreVersion-Missing.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.IsNull(extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CoreVersion_NoMinor()
        {
            TryParse("CoreVersion-NoMinor.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CoreVersion_BadValue()
        {
            TryParse("CoreVersion-BadValue.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CoreVersion_TooBig()
        {
            TryParse("CoreVersion-TooBig.txt");
        }

        //---------------------------------------------------------------------

        private ExtensionInfo ParseFile(string filename)
        {
        	try {
	            reader = OpenFile(filename);
	            return parser.Parse(reader);
        	}
        	finally {
	            reader.Close();
        	}
        }

        //---------------------------------------------------------------------

        [Test]
        public void FooBar()
        {
            ExtensionInfo extension = ParseFile("Foo-Bar.txt");
            Assert.IsNotNull(extension);
            Assert.AreEqual("Foo Bar", extension.Name);
            Assert.AreEqual("v3.8 (beta 4)", extension.Version);
            Assert.AreEqual("disturbance:foo-bar", extension.Type);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
                            extension.AssemblyName);
            Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
                            extension.ClassName);
            Assert.AreEqual("Compute foo-bar metrics across landscape",
                            extension.Description);
            Assert.AreEqual("Foo-Bar_User-Guide.txt",
                            extension.UserGuidePath);
            Assert.AreEqual(new System.Version("5.0"),
                            extension.CoreVersion);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Extra()
        {
            TryParse("Extra.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WrongName_CoreVersion()
        {
            TryParse("WrongName-CoreVersion.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WrongName_UserGuide_CoreVer()
        {
            TryParse("WrongName-UserGuide-CoreVer.txt");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void WrongName_Desc_UGuide_CoreVer()
        {
            TryParse("WrongName-Desc-UGuide-CoreVer.txt");
        }
    }
}
