using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// A collection of information about installed plug-ins.
    /// </summary>
    public class Dataset
        : IDataset
    {
        private static string defaultPath;

        private string path;
        private List<ExtensionInfo> plugIns;

        //---------------------------------------------------------------------

        static Dataset()
        {
            defaultPath = System.IO.Path.Combine(Application.Directory, "plug-ins.xml");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Default path to the plug-ins dataset.
        /// </summary>
        public static string DefaultPath
        {
            get {
                return defaultPath;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads the plug-ins dataset from a file if the file exists;
        /// otherwise creates an empty dataset and saves it to that file.
        /// </summary>
        public static Dataset LoadOrCreate(string path)
        {
        	if (path == null)
        		throw new ArgumentNullException();
        	if (path.Trim(null) == "")
        		throw new ArgumentException("Path is empty or just whitespace.");

            if (File.Exists(path))
                return new Dataset(path);
            else {
                Dataset dataset = new Dataset();
                dataset.SaveAs(path);
                return dataset;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A handler for events where a dataset is saved.
        /// </summary>
        public delegate void SavedEventHandler(Dataset dataset);

        //---------------------------------------------------------------------

        /// <summary>
        /// An event that occurs when a dataset is saved to a file (with either
        /// the Save or SaveAs method).
        /// </summary>
        public static event SavedEventHandler SavedEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with no plug-ins.
        /// </summary>
        /// <remarks>
        /// The dataset's Path is the DefaultPath.
        /// </remarks>
        public Dataset()
        {
            path = DefaultPath;
            plugIns = new List<ExtensionInfo>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance by loading the dataset from a file.
        /// </summary>
        /// <param name="path">
        /// Path to the file where the dataset is stored
        /// </param>
        public Dataset(string path)
        {
        	if (path == null)
        		throw new ArgumentNullException();
        	if (path.Trim(null) == "")
        		throw new ArgumentException("path is empty or just whitespace.");

            this.path = path;
            PersistentDataset dataset = PersistentDataset.Load(path);

            plugIns = new List<ExtensionInfo>();
            foreach (PersistentDataset.PlugInInfo info in dataset.PlugIns) {
            	plugIns.Add(new ExtensionInfo(info));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of plug-ins in the dataset.
        /// </summary>
        public int Count
        {
            get {
                return plugIns.Count;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The path to the dataset's file.
        /// </summary>
        /// <remarks>
        /// If the dataset was loaded from a file, then this is the path to
        /// that file.  If the SaveAs method has been called, then this is
        /// the path passed to the most recent call.
        /// </remarks>
        public string Path
        {
        	get {
        		return path;
        	}
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the information about an extension.
        /// </summary>
        /// <param name="name">
        /// The name of the extension.
        /// </param>
        /// <remarks>
        /// The information retrieved is the minimal set needed by the core
        /// framework.
        /// </remarks>
        /// <returns>
        /// null if there is no extension with the given name.
        /// </returns>
        public ExtensionInfo this[string name]
        {
            get {
                foreach (ExtensionInfo extensionInfo in plugIns)
                    if (extensionInfo.Name == name)
                        return extensionInfo;
                return null;
            }
        }

        //---------------------------------------------------------------------

        PlugInInfo IDataset.this[string name]
        {
            get {
        		ExtensionInfo extensionInfo = ((Dataset)this)[name];
                if (extensionInfo != null)
                    return extensionInfo.CoreInfo;
                return null;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the information for an extension at a particular index.
        /// </summary>
        public ExtensionInfo this[int index]
        {
            get {
        		return plugIns[index];
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new plug-in to the dataset.
        /// </summary>
        /// <param name="plugIn">
        /// Information about the new plug-in.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// There is already a plug-in with the same name in the dataset.
        /// </exception>
        public void Add(ExtensionInfo plugIn)
        {
        	Require.ArgumentNotNull(plugIn);
        	if (string.IsNullOrEmpty(plugIn.Name))
        		throw new ArgumentException("The plug-in's name is null or empty.");
        	if (this[plugIn.Name] != null) {
        		string mesg = string.Format("A plug-in with the name \"{0}\" is already in the dataset",
        		                            plugIn.Name);
        		throw new InvalidOperationException(mesg);
        	}

        	plugIns.Add(plugIn);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes a plug-in from the dataset.
        /// </summary>
        /// <param name="name">
        /// Name of the plug-in to remove.
        /// </param>
        /// <returns>
        /// The information about the plug-in that was removed, or null if there
        /// was no plug-in with the specified name.
        /// </returns>
        public ExtensionInfo Remove(string name)
        {
        	if (name == null)
        		throw new ArgumentNullException();
        	if (name.Trim(null) == "")
        		throw new ArgumentException("The plug-in's name is empty or just whitespace.");

        	ExtensionInfo plugIn;
        	for (int i = 0; i < plugIns.Count; i++) {
        		plugIn = plugIns[i];
        		if (plugIn.Name == name) {
        			plugIns.RemoveAt(i);
        			return plugIn;
        		}
        	}
        	return null;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Saves the dataset to the its associated file.
        /// </summary>
        public void Save()
        {
        	SaveAs(path);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Saves the dataset to a file, and associates that file with the
        /// dataset.
        /// </summary>
        public void SaveAs(string path)
        {
        	if (path == null)
        		throw new ArgumentNullException();
        	if (path.Trim(null) == "")
        		throw new ArgumentException();

        	PersistentDataset persistentDataset = new PersistentDataset();
        	foreach (ExtensionInfo extensionInfo in plugIns)
        		persistentDataset.PlugIns.Add(extensionInfo.PersistentInfo);
        	persistentDataset.Save(path);
        	
        	this.path = path;

        	if (SavedEvent != null)
        	    SavedEvent(this);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a library is referenced by any of the entries
        /// </summary>
        public bool ReferencedByEntries(string library)
        {
        	// TODO
        	return false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines which libraries in a given list are referenced by the
        /// entries.
        /// </summary>
        public IList<string> ReferencedByEntries(IList<string> libs)
        {
        	// TODO
        	return null;
        }
    }
}
