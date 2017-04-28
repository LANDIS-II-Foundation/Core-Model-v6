using Landis.Utilities;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

using Landis.Core;
using Landis.Landscapes;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;

namespace Landis
{
    public static class App
    {
        public static int Main(string[] args)
        {
            try {
                // The log4net section in the application's configuration file
                // requires the environment variable WORKING_DIR be set to the
                // current working directory.
                Environment.SetEnvironmentVariable("WORKING_DIR", Environment.CurrentDirectory);
                log4net.Config.XmlConfigurator.Configure();

                ConsoleInterface ci = new ConsoleInterface();

                ci.TextWriter = Console.Out;

                string version = GetAppSetting("version");
                if (version == "")
                    throw new Exception("The application setting \"version\" is empty or blank");
                string release = GetAppSetting("release");
                if (release != "")
                    release = string.Format(" ({0})", release);
                ci.WriteLine("LANDIS-II {0}{1}", version, release);

                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (object attribute in assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false))
                    ci.WriteLine(((AssemblyCopyrightAttribute) attribute).Copyright);
                ci.WriteLine();

                if (args.Length == 0) {
                    ci.WriteLine("Error: No scenario file specified.");
                    return 1;
                }
                if (args.Length > 1) {
                    ci.WriteLine("Error: Extra argument(s) on command line:");
                    StringBuilder argsList = new StringBuilder();
                    argsList.Append(" ");
                    for (int i = 1; i < args.Length; ++i)
                        argsList.AppendFormat(" {0}", args[i]);
                    ci.WriteLine(argsList.ToString());
                    return 1;
                }

                //Warn if user's computer is not using United States number format
                string decimal_separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string period = ".";
                if (decimal_separator != period)
                {
                    ci.WriteLine("Warning: The decimal separator of your system is '" + decimal_separator + "'. Most LANDIS-II");
                    ci.WriteLine("Warning: scenario files use '" + period + "' for the decimal separator. The scenario");
                    ci.WriteLine("Warning: files may not run or your results could be inaccurate.");
                    ci.WriteLine();
                }

                IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
                //Pre-pending the GDAL directory is required to run the Console project in debug mode
                //string path = Environment.GetEnvironmentVariable("PATH");
                //string newPath = "C:\\Program Files\\LANDIS-II\\GDAL\\1.9;" + path;
                //Environment.SetEnvironmentVariable("PATH", newPath);
                RasterFactory rasterFactory = new RasterFactory();
                LandscapeFactory landscapeFactory = new LandscapeFactory();
                Model model = new Model(extensions, rasterFactory, landscapeFactory);
                model.Run(args[0], ci);
                return 0;
            }
            catch (ApplicationException exc) {
                ConsoleInterface ci = new ConsoleInterface();
                ci.WriteLine(exc.Message);
                return 1;
            }
            catch (Exception exc) {

                ConsoleInterface ci = new ConsoleInterface();
                ci.WriteLine("Internal error occurred within the program:");
                ci.WriteLine("  {0}", exc.Message);
                if (exc.InnerException != null) {
                    ci.WriteLine("  {0}", exc.InnerException.Message);
                }
                ci.WriteLine();
                ci.WriteLine("Stack trace:");
                ci.WriteLine(exc.StackTrace);
                return 1;
            }
        }

        //---------------------------------------------------------------------

        public static string GetAppSetting(string settingName)
        {
            string setting = ConfigurationManager.AppSettings[settingName];
            if (setting == null)
                throw new Exception("The application setting \"" + settingName + "\" is not set");
            return setting.Trim(null);
        }
    }
}
