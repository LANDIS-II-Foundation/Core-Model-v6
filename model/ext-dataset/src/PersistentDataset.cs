using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Landis.Extensions
{
    /// <summary>
    /// A persistent collection of information about installed extensions.
    /// </summary>
    [XmlRoot("ExtensionDataset")]
    public class PersistentDataset
    {
        /// <summary>
        /// Information about a extension.
        /// </summary>
        public class ExtensionInfo
        {
            /// <summary>
            /// The extension's name.
            /// </summary>
            [XmlAttribute]
            public string Name;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The extension's version.
            /// </summary>
            [XmlAttribute("Version")]
            public string Version;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The extension's type.
            /// </summary>
            [XmlElement("Type")]
            public string TypeName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The extension's assembly.
            /// </summary>
            [XmlElement("Assembly")]
            public string AssemblyName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The class in the extension's assembly that represents the extension.
            /// </summary>
            [XmlElement("Class")]
            public string ClassName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// A brief description of the extension.
            /// </summary>
            public string Description;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The path to the extension's user guide.
            /// </summary>
            /// <remarks>
            /// The path is relative to the documentation directory of a
            /// LANDIS-II installation.
            /// </remarks>
            [XmlElement("UserGuide")]
            public string UserGuidePath;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance with all empty fields.
            /// </summary>
            public ExtensionInfo()
            {
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The extensions in the collection.
        /// </summary>
        [XmlArrayItem("Extension")]
        public List<ExtensionInfo> Extensions;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with an empty list of extensions.
        /// </summary>
        public PersistentDataset()
        {
            Extensions = new List<ExtensionInfo>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads a extension information dataset from a file.
        /// </summary>
        public static PersistentDataset Load(string path)
        {
            PersistentDataset dataset;
            using (TextReader reader = new StreamReader(path)) {
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDataset));
                dataset = (PersistentDataset) serializer.Deserialize(reader);
            }
            return dataset;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Saves the driver dataset to a file.
        /// </summary>
        public void Save(string path)
        {
            using (TextWriter writer = new StreamWriter(path)) {
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDataset));
                serializer.Serialize(writer, this);
            }
        }
    }
}
