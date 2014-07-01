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
        // That's our custom to redirect console output to form
        TextWriter _writer = null;
        
        public Launcher()
        {
            InitializeComponent();

            // Prepend system path with LANDIS GDAL folder so app can find the libraries
            string path = Environment.GetEnvironmentVariable(Constants.ENV_PATH);
            string newPath = Constants.GDAL_FOLDER + ";" + path;
            Environment.SetEnvironmentVariable(Constants.ENV_PATH, newPath);

            // Instantiate text writer
            _writer = new TextBoxStreamWriter(TxtBoxStatus);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = _writer;

            string workingDirectory = Path.GetDirectoryName(txtFilePath.Text);
            try
            {
                //Reset the textbox color to black
                TxtBoxStatus.ForeColor = Color.Black;
                TxtBoxStatus.Text = "";
                
                // The log4net section in the application's configuration file
                // requires the environment variable WORKING_DIR be set to the
                // current working directory.
                // This will be the folder containing the scenario .txt file
                Environment.SetEnvironmentVariable(Constants.ENV_WORKING_DIR, workingDirectory);
                log4net.Config.XmlConfigurator.Configure();

                // Set the working directory for the Model
                Directory.SetCurrentDirectory(workingDirectory);

                // Get the installed LANDIS version from the console
                //string version = Landis.App.GetAppSetting("version");
                string version = LauncherUtil.GetAssemblySetting("version");
                if (version == "")
                    throw new Exception("The LANDIS application setting \"version\" is empty or blank");
                string release = LauncherUtil.GetAssemblySetting("release");
                if (release != "")
                    release = string.Format(" ({0})", release);
                wi.WriteLine("LANDIS-II {0}{1}", version, release);

                // Get the installed Widgets version from the config file

                //@ToDo: Is it okay to hard-code this path? If the widget runs from the LANDIS-II bin, shouldn't be needed
                //Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
                string extFolder = Constants.EXTENSIONS_FOLDER + Constants.EXTENSIONS_XML;
                Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate(extFolder);
                RasterFactory rasterFactory = new RasterFactory();
                Landis.Landscapes.LandscapeFactory landscapeFactory = new Landis.Landscapes.LandscapeFactory();
                Landis.Model model = new Landis.Model(extensions, rasterFactory, landscapeFactory);
                model.Run(txtFilePath.Text, wi);
                MessageBox.Show("The end!");
            }
            catch (Exception exc)
            {
                //Change the text color to red to alert the user
                TxtBoxStatus.ForeColor = Color.Red;
                //Print a warning message
                string strError = "\r\nA program error occurred.\r\nThe error log is available at " + workingDirectory + Constants.ERROR_LOG;
                wi.WriteLine(strError);
                using (TextWriter writer = File.CreateText(workingDirectory + Constants.ERROR_LOG))
                {
                    writer.WriteLine("Internal error occurred within the program:");
                    writer.WriteLine("  {0}", exc.Message);
                    if (exc.InnerException != null)
                    {
                        writer.WriteLine("  {0}", exc.InnerException.Message);
                    }
                    writer.WriteLine();
                    writer.WriteLine("Stack trace:");
                    writer.WriteLine(exc.StackTrace);
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
