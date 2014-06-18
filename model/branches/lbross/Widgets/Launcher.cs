using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;


namespace Widgets
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();

            // Will this fix my GDAL problem ?
            string path = Environment.GetEnvironmentVariable("PATH");
            string newPath = "C:\\Program Files\\LANDIS-II\\GDAL\\1.9;" + path;
            Environment.SetEnvironmentVariable("PATH", newPath);

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected static string GetAppSetting(string settingName)
        {
            string setting = System.Configuration.ConfigurationManager.AppSettings[settingName];
            if (setting == null)
                throw new Exception("The application setting \"" + settingName + "\" is not set");
            return setting.Trim(null);
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            string workingDirectory = Path.GetDirectoryName(txtFilePath.Text);
            try
            {
                // The log4net section in the application's configuration file
                // requires the environment variable WORKING_DIR be set to the
                // current working directory.
                // This will be the folder containing the scenario .txt file
                Environment.SetEnvironmentVariable("WORKING_DIR", workingDirectory);
                log4net.Config.XmlConfigurator.Configure();

                WidgetInterface wi = new WidgetInterface();

                //
                // Create a new TextWriter in the resource acquisition statement.
                // @ToDo: Check to make sure user has write access to workingDirectory
                using (TextWriter writer = File.CreateText(workingDirectory + "\\mLog.txt"))
                {
                    wi.TextWriter = writer;
                    string version = GetAppSetting("version");
                    if (version == "")
                        throw new Exception("The application setting \"version\" is empty or blank");
                    string release = GetAppSetting("release");
                    if (release != "")
                        release = string.Format(" ({0})", release);
                    wi.WriteLine("LANDIS-II {0}{1}", version, release);


                    Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
                    RasterFactory rasterFactory = new RasterFactory();
                    Landis.Landscapes.LandscapeFactory landscapeFactory = new Landis.Landscapes.LandscapeFactory();
                    Landis.Model model = new Landis.Model(extensions, rasterFactory, landscapeFactory);
                    model.Run(txtFilePath.Text, wi);
                }
            }
            catch (Exception exc)
            {
                WidgetInterface wi = new WidgetInterface();
                using (TextWriter writer = File.CreateText(workingDirectory + "\\errorLog.txt"))
                {
                    wi.TextWriter = writer;
                    wi.WriteLine("Internal error occurred within the program:");
                    wi.WriteLine("  {0}", exc.Message);
                    if (exc.InnerException != null)
                    {
                        wi.WriteLine("  {0}", exc.InnerException.Message);
                    }
                    wi.WriteLine();
                    wi.WriteLine("Stack trace:");
                    wi.WriteLine(exc.StackTrace);
                }
            }

        }

        private void BtnFile_Click(object sender, EventArgs e)
        {
            openFD.Title = "Scenario file";
            // @ToDo: Where to set the initial directory?
            openFD.InitialDirectory = "C:";
            openFD.FileName = "";
            openFD.Filter = "Text|*.txt";
            openFD.ShowDialog();

            string filePath = openFD.FileName;
            txtFilePath.Text = filePath;
        }

        private void lblWww_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            this.lblWww.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start(lblWww.Text);
        }

    }
}
