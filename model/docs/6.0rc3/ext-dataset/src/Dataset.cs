using Landis.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Landis.Extensions
{
    /// <summary>
    /// A collection of information about installed extensions.
    /// </summary>
    public class Dataset
        : IExtensionDataset
    {
        private static string defaultPath;

        private string path;
        private List<ExtensionInfo> extensions;

        //---------------------------------------------------------------------

        static Dataset()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            System.Uri thisAssemblyUri = new System.Uri(thisAssembly.CodeBase);
            string thisAssemblyPath = thisAssemblyUri.LocalPath;
            string thisAssemblyDir = System.IO.Path.GetDirectoryName(thisAssemblyPath);
            defaultPath = System.IO.Path.Combine(thisAssemblyDir, "extensions.xml");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Default path to the extensions dataset.
        /// </summary>
        /// <remarks>
        /// It is the "extensions.xml" file in the directory where this library
        /// is located.
        /// </remarks>
        public static string DefaultPath
        {
            get {
                return defaultPath;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads the extensions dataset from a file if the file exists;
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
        /// Loads the extensions dataset from the DefaultPath if the file
        /// exists; otherwise creates an empty dataset and saves it to that
        /// path.
        /// </summary>
        public static Dataset LoadOrCreate()
        {
            return LoadOrCreate(DefaultPath);
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
        /// Initializes a new instance with no extensions.
        /// </summary>
        /// <remarks>
        /// The dataset's Path is the DefaultPath.
        /// </remarks>
        public Dataset()
        {
            path = DefaultPath;
            extensions = new List<ExtensionInfo>();
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

            extensions = new List<ExtensionInfo>();
            foreach (PersistentDataset.ExtensionInfo info in dataset.Extensions) {
                extensions.Add(new ExtensionInfo(info));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of extensions in the dataset.
        /// </summary>
        public int Count
        {
            get {
                return extensions.Count;
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
                foreach (ExtensionInfo extensionInfo in extensions)
                    if (extensionInfo.Name == name)
                        return extensionInfo;
                return null;
            }
        }

        //---------------------------------------------------------------------

        Landis.Core.ExtensionInfo IExtensionDataset.this[string name]
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
                return extensions[index];
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new extension to the dataset.
        /// </summary>
        /// <param name="extension">
        /// Information about the new extension.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// There is already a extension with the same name in the dataset.
        /// </exception>
        public void Add(ExtensionInfo extension)
        {
            if (extension == null)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(extension.Name))
                throw new ArgumentException("The extension's name is null or empty.");
            if (this[extension.Name] != null) {
                string mesg = string.Format("A extension with the name \"{0}\" is already in the dataset",
                                            extension.Name);
                throw new InvalidOperationException(mesg);
            }

            extensions.Add(extension);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes a extension from the dataset.
        /// </summary>
        /// <param name="name">
        /// Name of the extension to remove.
        /// </param>
        /// <returns>
        /// The information about the extension that was removed, or null if there
        /// was no extension with the specified name.
        /// </returns>
        public ExtensionInfo Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException();
            if (name.Trim(null) == "")
                throw new ArgumentException("The extension's name is empty or just whitespace.");

            ExtensionInfo extension;
            for (int i = 0; i < extensions.Count; i++) {
                extension = extensions[i];
                if (extension.Name == name) {
                    extensions.RemoveAt(i);
                    return extension;
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
            foreach (ExtensionInfo extensionInfo in extensions)
                persistentDataset.Extensions.Add(extensionInfo.PersistentInfo);
            persistentDataset.Save(path);
            
            this.path = path;

            if (SavedEvent != null)
                SavedEvent(this);
        }
    }
}
