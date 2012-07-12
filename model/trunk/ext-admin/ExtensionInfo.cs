
using Landis.Core;

namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// Information about an extension.
    /// </summary>
    public class ExtensionInfo
    {
    	private PersistentDataset.PlugInInfo persistentInfo;
    	private System.Version coreVersion;

        //---------------------------------------------------------------------

        /// <summary>
        /// The information that is persisted for an extension.
        /// </summary>
        public PersistentDataset.PlugInInfo PersistentInfo
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
		public PlugInInfo CoreInfo
		{
			get {
				ExtensionType plugInType = null;
				if (! string.IsNullOrEmpty(Type))
					plugInType = new ExtensionType(Type);

				string implementationName = null;
				if (! string.IsNullOrEmpty(ClassName)) {
					implementationName = ClassName;
					if (! string.IsNullOrEmpty(AssemblyName))
						implementationName = implementationName + "," + AssemblyName;
				}
				return new PlugInInfo(Name, plugInType, implementationName);
			}
		}

        //---------------------------------------------------------------------

        public ExtensionInfo(PersistentDataset.PlugInInfo info)
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
        	persistentInfo = new PersistentDataset.PlugInInfo();
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
