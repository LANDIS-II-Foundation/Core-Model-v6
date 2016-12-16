using Landis.Core;

namespace Landis.Extensions
{
    /// <summary>
    /// Information about an extension.
    /// </summary>
    public class ExtensionInfo
    {
        private PersistentDataset.ExtensionInfo persistentInfo;
        private System.Version coreVersion;

        //---------------------------------------------------------------------

        /// <summary>
        /// The information that is persisted for an extension.
        /// </summary>
        public PersistentDataset.ExtensionInfo PersistentInfo
        {
            get {
                return persistentInfo;
            }
        }

        //----------------------------------------------------------------------

        public string Name
        {
            get {
                return persistentInfo.Name;
            }

            set {
                persistentInfo.Name = value;
            }
        }

        //----------------------------------------------------------------------

        public string Type
        {
            get {
                return persistentInfo.TypeName;
            }

            set {
                persistentInfo.TypeName = value;
            }
        }

        //----------------------------------------------------------------------

        public string Version
        {
            get {
                return persistentInfo.Version;
            }

            set {
                persistentInfo.Version = value;
            }
        }

        //----------------------------------------------------------------------

        public string AssemblyName
        {
            get {
                return persistentInfo.AssemblyName;
            }

            set {
                persistentInfo.AssemblyName = value;
            }
        }

        //----------------------------------------------------------------------

        public string ClassName
        {
            get {
                return persistentInfo.ClassName;
            }

            set {
                persistentInfo.ClassName = value;
            }
        }

        //----------------------------------------------------------------------

        public string Description
        {
            get {
                return persistentInfo.Description;
            }
        }

        //----------------------------------------------------------------------

        public string UserGuidePath
        {
            get {
                return persistentInfo.UserGuidePath;
            }

            set {
                persistentInfo.UserGuidePath = value;
            }
        }

        //----------------------------------------------------------------------

        public System.Version CoreVersion
        {
            get {
                return coreVersion;
            }
        }

        //----------------------------------------------------------------------

        /// <summary>
        /// Gets just the information about the extension that the core
        /// framework needs.
        /// </summary>
        public Landis.Core.ExtensionInfo CoreInfo
        {
            get {
                ExtensionType extensionType = null;
                if (! string.IsNullOrEmpty(Type))
                    extensionType = new ExtensionType(Type);

                string implementationName = null;
                if (! string.IsNullOrEmpty(ClassName)) {
                    implementationName = ClassName;
                    if (! string.IsNullOrEmpty(AssemblyName))
                        implementationName = implementationName + "," + AssemblyName;
                }
                return new Landis.Core.ExtensionInfo(Name, extensionType, implementationName);
            }
        }

        //---------------------------------------------------------------------

        public ExtensionInfo(PersistentDataset.ExtensionInfo info)
        {
            persistentInfo = info;
        }

        //---------------------------------------------------------------------

        public ExtensionInfo(string         name,
                             string         version,
                             string         type,
                             string         assemblyName,
                             string         className,
                             string         description,
                             string         userGuidePath,
                             System.Version coreVersion)
        {
            persistentInfo = new PersistentDataset.ExtensionInfo();
            persistentInfo.Name = name;
            persistentInfo.Version = version;
            persistentInfo.TypeName = type;
            persistentInfo.AssemblyName = assemblyName;
            persistentInfo.ClassName = className;
            persistentInfo.Description = description;
            persistentInfo.UserGuidePath = userGuidePath;

            this.coreVersion = coreVersion;
        }
    }
}
