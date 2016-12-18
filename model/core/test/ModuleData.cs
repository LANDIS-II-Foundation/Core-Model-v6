using System.IO;

namespace Landis.Test
{
    public static class ModuleData
    {
        /// <summary>
        /// Get the relative path to the test data for a core's module.
        /// </summary>
        /// <remarks>
        /// The module's test data reside in a subdirectory called "data" in
        /// the module's subdirectory.  The path to this subdirectory is
        /// "core/test/MODULE/data" where MODULE is the module's name.
        /// <param name='moduleName'>
        /// The name of the module's subdirectory in the "test/" folder.
        /// </param>
        public static string GetRelativePath(string moduleName)
        {
            string coreTestDir = Path.Combine("core", "test");
            string moduleDir = Path.Combine(coreTestDir, moduleName);
            string moduleDataDir = Path.Combine(moduleDir, "data");

            return moduleDataDir;
        }
    }
}
