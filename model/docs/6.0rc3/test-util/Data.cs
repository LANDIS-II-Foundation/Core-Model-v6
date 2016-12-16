using System.IO;
using System.Reflection;

namespace Landis.Test
{
    public static class Data
    {
        public const string DirPlaceholder = "{data folder}";

        private static string directory;

        /// <summary>
        /// The directory with the test data.
        /// </summary>
        public static string Directory
        {
            get {
                return directory;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize the path to the directory with the test data.
        /// </summary>
        /// <remarks>
        /// The test data reside in a subdirectory relative to the solution's
        /// root.  This method determines the absolute path to the directory
        /// with the test data, stores it, and prints it to Output showing
        /// what the DataDirPlaceholder represents.
        /// </remarks>
        /// <param name='dataRelPath'>
        /// The relative path to the data directory from the solution's root
        /// directory.
        /// </param>
        public static void InitializeDirectory(string dataRelPath)
        {
            Assembly testAssembly = Assembly.GetExecutingAssembly();
            System.Uri testAssemblyUri = new System.Uri(testAssembly.CodeBase);
            string testAssemblyPath = testAssemblyUri.LocalPath;
 
            // Test assembly is located in SOLUTION_ROOT/build/CONFIG where
            // CONFIG is "Debug" or "Release".
            string configDir = Path.GetDirectoryName(testAssemblyPath);
            string buildDir = Path.GetDirectoryName(configDir);
            string solutionRoot = Path.GetDirectoryName(buildDir);
 
            directory = Path.Combine(solutionRoot, dataRelPath);

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
