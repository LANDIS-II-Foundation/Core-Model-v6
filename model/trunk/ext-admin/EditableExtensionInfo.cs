using Edu.Wisc.Forest.Flel.Util;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Landis.Core;

namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// Editable information about an extension.
    /// </summary>
    public class EditableExtensionInfo
    	: IEditable<ExtensionInfo>
    {
        /// <summary>
        /// The dataset to use to determine if the extension name is already
        /// being used.  If null, then the extension name read from the text
        /// input is not checked to see if it's already in use.
        /// </summary>
        public static Dataset Dataset = null;

    	private InputValue<string> name;
    	private InputValue<string> version;
    	private InputValue<string> type;
    	private InputValue<string> assemblyName;
    	private InputValue<string> className;
    	private InputValue<string> description;
    	private InputValue<string> userGuidePath;
    	private InputValue<string> coreVersion;
    	private System.Version coreVersion_;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public EditableExtensionInfo()
        {
            if (App.InstallingExtension) {
                // TODO: Scan the appropriate directory (current directory?,
                // the directory where the extension info file is located?)
                // to look for assembly files (*.dll).  Or perhaps, those files
                // are in a bin/ subfolder.  Search assembly files for a class
                // that's a subclass of Landis.PlugIns.PlugIn.  Instantiate that
                // class to query it for its plug-in (this will require that the
                // assembly files are already installed in LANDIS-II's bin
                // folder).  (Alternative, reflection-only load the assembly,
                // but then the plug-in type will need to stored as an assembly
                // or class attribute.)
            }
        }

        //---------------------------------------------------------------------

        public InputValue<string> Name
        {
        	get {
        		return name;
        	}

        	set {
                if (value != null) {
                    CheckNotEmptyOrBlank(value);
                    if (Dataset != null && Dataset[value.Actual] != null)
                        throw new InputValueException(value.String,
                    	                              "The name \"{0}\" is already in the extensions database",
                    	                              value.Actual);
                }
        		name = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> Version
        {
        	get {
        		return version;
        	}

        	set {
                if (value != null)
                    CheckNotEmptyOrBlank(value);
        		version = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> Type
        {
        	get {
        		return type;
        	}

        	set {
                if (App.InstallingExtension)
                    throw new InvalidOperationException();
                if (value != null)
                    CheckNotEmptyOrBlank(value);
        		type = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> AssemblyName
        {
        	get {
        		return assemblyName;
        	}

        	set {
                if (App.InstallingExtension)
                    throw new InvalidOperationException();
                if (value != null) {
                    CheckNotEmptyOrBlank(value);
                    // TODO: Check value if properly structured assembly name
                }
        		assemblyName = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> ClassName
        {
        	get {
        		return className;
        	}

        	set {
                if (value != null) {
                    CheckNotEmptyOrBlank(value);
                    // TODO: Check value if properly structured class name
                }
        		className = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> Description
        {
        	get {
        		return description;
        	}

        	set {
                if (value != null)
                    CheckNotEmptyOrBlank(value);  // HACK to get existing tests to pass
        		description = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> UserGuidePath
        {
        	get {
        		return userGuidePath;
        	}

        	set {
                if (value != null) {
                    CheckNotEmptyOrBlank(value);
                    if (App.InstallingExtension)
                        CheckFileExists(value);
                }
        		userGuidePath = value;
        	}
        }

		//----------------------------------------------------------------------

        public InputValue<string> CoreVersion
        {
        	get {
        		return coreVersion;
        	}

        	set {
                if (value != null) {
                    CheckNotEmptyOrBlank(value);
                    coreVersion_ = GetVersion(value.Actual); // HACK
                }
        		coreVersion = value;
        	}
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
        	get {
        		//  Make sure the fields needed for core are present
        		foreach (object parameter in new object[]{ name,
        		                                           type,
        		                                           assemblyName,
        		                                           className}) {
        			if (parameter == null)
        				return false;
        		}
        		return true;
        	}
        }

        //---------------------------------------------------------------------

        private void CheckNotEmptyOrBlank(InputValue<string> strValue)
        {
            if (strValue.Actual.Length == 0)
                throw new InputValueException(strValue.String, "Empty string is not permitted");
            if (strValue.Actual.Trim(null) == "")
                throw new InputValueException(strValue.String, "String with just whitespace is not permitted");
        }

        //---------------------------------------------------------------------

        private void CheckFileExists(InputValue<string> strValue)
        {
            string path = strValue.Actual;
            if (! File.Exists(path)) {
                if (System.IO.Directory.Exists(path))
                    throw new InputValueException(strValue.String, "{0} is not a file; it's a directory", path);
                throw new InputValueException(strValue.String, "The file {0} does not exist", path);
            }
        }

        //---------------------------------------------------------------------

        private Assembly LoadAssembly(InputValue<string> name)
        {
            try {
                string fullPath = Path.GetFullPath(name.Actual);
                return Assembly.LoadFile(fullPath);
            }
            catch (System.Exception) {
                throw new InputValueException(name.String, "Could not load the assembly {0}", name.Actual);
            }
        }

        //---------------------------------------------------------------------

        private System.Type GetInterfaceType(string   className,
                                             Assembly assembly)
        {
            System.Type classType;
            try {
                classType = assembly.GetType(className);
            }
            catch (System.Exception) {
                throw new InputValueException(className, "Could not get the class {0} from the assembly", className);
            }
            if (classType == null)
                throw new InputValueException(className, "The assembly has no {0} class", className);
            if (! classType.IsClass)
                throw new InputValueException(className, "{0} is not a class", className);

            System.Type plugInBaseClass = typeof(ExtensionMain);
            if (classType.IsSubclassOf(plugInBaseClass))
                return plugInBaseClass;
            string[] message = new string[]{
                string.Format("The class {0} is not", className),
                "a LANDIS-II plug-in."
            };
            throw new InputValueException(className, message);
        }

        //---------------------------------------------------------------------

        private System.Version GetVersion(string version)
        {
            Regex pattern = new Regex(@"^\d+(\.\d+){1,3}$");
            if (! pattern.IsMatch(version))
                throw new InputValueException(version, "\"{0}\" is not a proper version number", version);
            try {
                return new System.Version(version);
            }
            catch (System.OverflowException) {
                throw new InputValueException(version, "One or more parts of \"{0}\" is too big", version);
            }
        }

        //---------------------------------------------------------------------

        public ExtensionInfo GetComplete()
        {
            if (IsComplete) {
                return new ExtensionInfo(
                    name.Actual,
        	        version == null ? null : version.Actual,
        	        type.Actual,
        	        assemblyName.Actual,
        	        className.Actual,
        	        description == null ? null : description.Actual,
        	        userGuidePath == null ? null : userGuidePath.Actual,
        	        coreVersion_
        	    );
            }
        	else
	        	return null;
        }
    }
}
