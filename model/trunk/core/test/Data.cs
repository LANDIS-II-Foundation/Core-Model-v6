using System.IO;
using System.Reflection;

namespace Landis.Test
{
    public static class Data
    {
        public const string DirPlaceholder = "{data folder}";

        private static string directory;

        /// <summary>
        /// The directory with the core module's test data.
        /// </summary>
        public static string Directory
        {
            get {
                return directory;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize the directory with the core module's test data.
        /// </summary>
        /// <remarks>
        /// The module's test data reside in a subdirectory called "data" in
        /// the module's subdirectory.  This method determines the path to the
        /// module's data directory, stores it, and prints it to Output showing
        /// what the DataDirPlaceholder represents.
        /// </remarks>
        /// <param name='ModuleDirName'>
        /// The name of the module's subdirectory in the "test/" folder.
        /// </param>
        public static void InitializeDirectory(string moduleDirName)
        {
            Assembly testAssembly = Assembly.GetExecutingAssembly();
            System.Uri testAssemblyUri = new System.Uri(testAssembly.CodeBase);
            string testAssemblyPath = testAssemblyUri.LocalPath;
 
            // Test assembly is located in SOLUTION_ROOT/build/CONFIG where
            // CONFIG is "Debug" or "Release".
            string configDir = Path.GetDirectoryName(testAssemblyPath);
            string buildDir = Path.GetDirectoryName(configDir);
            string solutionRoot = Path.GetDirectoryName(buildDir);
 
            // The module's data dir = SOLUTION_ROOT/core/test/MODULE/data
            string coreDir = Path.Combine(solutionRoot, "core");
            string testDir = Path.Combine(coreDir, "test");
            string moduleDir = Path.Combine(testDir, moduleDirName);
            directory = Path.Combine(moduleDir, "data");

            Output.WriteLine("{0} = \"{1}\"", DirPlaceholder, Directory);
            Output.WriteLine();
        }

        //---------------------------------------------------------------------

        public static string MakeInputPath(string filename)
        {
            return Path.Combine(Directory, filename);
        }

        //---------------------------------------------------------------------

        public static TextWriter Output
        {
            get {
                return System.Console.Out;
            }
        }
    }
}
