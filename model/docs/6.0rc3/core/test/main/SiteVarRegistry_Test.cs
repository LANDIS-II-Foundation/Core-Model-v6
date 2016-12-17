using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Test.Main
{
    [TestFixture]
    public class SiteVarRegistry_Test
    {
        private ILandscape landscape;
        private SiteVarRegistry siteVarRegistry;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            bool[,] array = new bool[0,0];
            DataGrid<bool> grid = new DataGrid<bool>(array);
            landscape = new Landscape(grid);

            siteVarRegistry = new SiteVarRegistry();
        }

        //---------------------------------------------------------------------

        private void TryRegister(ISiteVariable siteVar,
                                 string        name)
        {
            try {
                siteVarRegistry.RegisterVar(siteVar, name);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void Register_NullVar()
        {
            TryRegister(null, "");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void Register_NullName()
        {
            ISiteVar<bool> var = landscape.NewSiteVar<bool>();
            TryRegister(var, null);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ApplicationException))]
        public void Register_SameName()
        {
            ISiteVar<bool> var = landscape.NewSiteVar<bool>();
            string name = "foo";
            siteVarRegistry.RegisterVar(var, name);
            TryRegister(var, name);
        }

        //---------------------------------------------------------------------

        [Test]
        public void GetVar()
        {
            ISiteVar<bool> var = landscape.NewSiteVar<bool>();
            string name = "My Site Variable";
            SiteVarRegistry registry = new SiteVarRegistry();
            registry.RegisterVar(var, name);

            ISiteVar<bool> fetchedVar = registry.GetVar<bool>(name);
            Assert.IsNotNull(fetchedVar);
            Assert.AreEqual(var, fetchedVar);
        }

        //---------------------------------------------------------------------

        [Test]
        public void GetVar_NameNotRegistered()
        {
            SiteVarRegistry registry = new SiteVarRegistry();

            ISiteVar<bool> fetchedVar = registry.GetVar<bool>("Should not exist");
            Assert.IsNull(fetchedVar);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ApplicationException))]
        public void GetVar_TypeMismatch()
        {
            ISiteVar<bool> var = landscape.NewSiteVar<bool>();
            string name = "My Site Variable";
            SiteVarRegistry registry = new SiteVarRegistry();
            registry.RegisterVar(var, name);

            try {
                ISiteVar<int> fetchedVar = registry.GetVar<int>(name);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
        }
    }
}
