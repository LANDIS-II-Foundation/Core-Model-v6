using Landis.PlugIns.Admin;
using NUnit.Framework;

namespace Landis.Test.PlugIns.Admin
{
    [TestFixture]
    public class ExtensionInfo_Test
    {
        [Test]
        public void PersistentInfo()
        {
        	ExtensionInfo extensionInfo = new ExtensionInfo(
				"Foo Bar",				    // name
        	    "3.8",                      // version
        	    "disturbance:bar",          // type
        	    "Org.Foo.Bar",              // assembly name
        	    "Org.Foo.Bar.Extension",    // class name
        	    "Computes foobar stats",    // description
        	    "Foo Bar User Guide.pdf",   // user guide path
        	    new System.Version("5.1")   // core version
        	);

        	PersistentDataset.PlugInInfo persistentInfo = extensionInfo.PersistentInfo;
        	Assert.IsNotNull(persistentInfo);
        	Assert.AreEqual(persistentInfo.Name,
        	                 extensionInfo.Name);
        	Assert.AreEqual(persistentInfo.TypeName,
        	                 extensionInfo.Type);
        	Assert.AreEqual(persistentInfo.Version,
        	                 extensionInfo.Version);
        	Assert.AreEqual(persistentInfo.AssemblyName,
        	                 extensionInfo.AssemblyName);
        	Assert.AreEqual(persistentInfo.ClassName,
        	                 extensionInfo.ClassName);
        	Assert.AreEqual(persistentInfo.UserGuidePath,
        	                 extensionInfo.UserGuidePath);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CoreInfo_NameAndType()
        {
        	ExtensionInfo extensionInfo = new ExtensionInfo(
				"Foo Bar",				    // name
        	    null,                       // version
        	    "output",                   // type
        	    null,                       // assembly name
        	    null,                       // class name
        	    null,                       // description
        	    null,                       // user guide path
        	    null                        // core version
        	);

        	Landis.PlugIns.PlugInInfo coreInfo = extensionInfo.CoreInfo;
        	Assert.IsNotNull(coreInfo);
        	Assert.AreEqual(extensionInfo.Name, coreInfo.Name);
        	Assert.IsNotNull(coreInfo.PlugInType);
        	Assert.AreEqual(extensionInfo.Type, coreInfo.PlugInType.Name);
        	Assert.IsNull(coreInfo.ImplementationName);
        }
    }
}
