using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Widgets
{
    class LauncherUtil
    {

        public static string GetAppSetting(string settingName)
        {
            string setting = System.Configuration.ConfigurationManager.AppSettings[settingName];
            if (setting == null)
                throw new Exception("The application setting \"" + settingName + "\" is not set");
            return setting.Trim(null);
        }

        public static string GetAssemblySetting(string settingName)
        {
            Type t = Type.GetType("Landis.Model,Landis.Core.Implementation");
            Assembly a = System.Reflection.Assembly.GetAssembly(t);
            string config = string.Empty;
            string version = string.Empty;
            if (a != null) {
                object[] customAttributes = a.GetCustomAttributes(false);
                if (settingName == "release") {
                    foreach (object attribute in customAttributes)
                    {  
                        if (attribute.GetType() == typeof(System.Reflection.AssemblyConfigurationAttribute))
                        {
                            config = ((System.Reflection.AssemblyConfigurationAttribute) attribute).Configuration;
                            return config;
                        }
                    }
                }
                else if (settingName == "version")
                {
                    Version assemblyVersion = a.GetName().Version;
                    string majorVersion = assemblyVersion.Major.ToString();
                    string minorVersion = assemblyVersion.Minor.ToString();
                    version = majorVersion + "." + minorVersion;
                    return version;
                }

             }
            return string.Empty;
        }
    }
}
