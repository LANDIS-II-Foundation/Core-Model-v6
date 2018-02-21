using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis.Extensions.Admin
{
    /// <summary>
    /// A parser that reads information about an extension from text input.
    /// </summary>
    public class ExtensionParser
        : Landis.TextParser<ExtensionInfo>
    {
        public override string LandisDataValue
        {
            get {
                return "Extension";
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ExtensionParser()
        {
        }

        //---------------------------------------------------------------------

        protected override ExtensionInfo Parse()
        {
            ReadLandisDataVar();

            EditableExtensionInfo extension = new EditableExtensionInfo();

            InputVar<string> name = new InputVar<string>("Name");
            ReadVar(name);
            extension.Name = name.Value;

            InputVar<string> version = new InputVar<string>("Version");
            if (ReadOptionalVar(version))
                extension.Version = version.Value;

            //  If the extension is not being installed, but rather just an
            //  entry for it is being added to the dataset, then we need its
            //  type, assembly and class info.
            if (! App.InstallingExtension) {
                InputVar<string> type = new InputVar<string>("Type");
                ReadVar(type);
                extension.Type = type.Value;
    
                InputVar<string> assemblyPath = new InputVar<string>("Assembly");
                ReadVar(assemblyPath);
                extension.AssemblyName = assemblyPath.Value;
    
                InputVar<string> className = new InputVar<string>("Class");
                ReadVar(className);
                extension.ClassName = className.Value;
            }

            InputVar<string> description = new InputVar<string>("Description");
            if (ReadOptionalVar(description))
                extension.Description = description.Value;

            InputVar<string> userGuide = new InputVar<string>("UserGuide");
            if (ReadOptionalVar(userGuide))
                extension.UserGuidePath = userGuide.Value;

            //  CoreVersion is optional.  We don't use ReadOptionalVar because
            //  we want to be able to check for any data after the last known
            //  parameter.  But that last known parameter could Name, Version,
            //  Class, Description, or UserGuide.  And we want the error
            //  message to list any optional variables that weren't found.  So
            //  instead, we use ReadVar if there's a non-blank line left in
            //  the input.
            if (! AtEndOfInput) {
                InputVar<string> coreVersion = new InputVar<string>("CoreVersion");
                ReadVar(coreVersion);
                extension.CoreVersion = coreVersion.Value;

                CheckNoDataAfter(string.Format("the {0} parameter", coreVersion.Name));
            }

            return extension.GetComplete();
        }
    }
}
