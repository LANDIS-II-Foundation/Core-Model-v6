using Edu.Wisc.Forest.Flel.Util;
using System.IO;

namespace Landis.Test.PlugIns.Admin
{
    public static class Data
    {
        private static NUnitInfo myNUnitInfo = new NUnitInfo();

        //---------------------------------------------------------------------

        public static readonly string Directory = myNUnitInfo.GetDataDir();
        public const string DirPlaceholder = "{data folder}";

        public static string MakeInputPath(string filename)
        {
            return Path.Combine(Directory, filename);
        }

        //---------------------------------------------------------------------

        static Data()
        {
            Output.WriteLine("{0} = \"{1}\"", DirPlaceholder, Directory);
        }

		//---------------------------------------------------------------------

		public static readonly string OutputDir = myNUnitInfo.GetOutDir(true);

		public static string MakeOutputPath(string filename)
		{
			return Path.Combine(OutputDir, filename);
		}

        //---------------------------------------------------------------------

        private static TextWriter writer = myNUnitInfo.GetTextWriter();

        public static TextWriter Output
        {
            get {
                return writer;
            }
        }
    }
}
