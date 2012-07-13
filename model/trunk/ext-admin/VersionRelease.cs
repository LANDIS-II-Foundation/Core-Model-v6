using System;
using System.Reflection;

namespace Edu.Wisc.Forest.Flel.Util
{
    /// <summary>
    /// A release of a particular version of a software product.
    /// </summary>
    public class VersionRelease
    {
        private Version version;
        private Release release;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance from a version number (major and minor
        /// components) and a release.
        /// </summary>
        public VersionRelease(int     major,
                              int     minor,
                              Release release)
        {
            ValidateVersionParam(major, "major");
            ValidateVersionParam(minor, "minor");
            if (release == null)
                throw new ArgumentNullException();

            this.version = new System.Version(major, minor, release.BuildNumber);
            this.release = release;
        }

        //---------------------------------------------------------------------

        private void ValidateVersionParam(int    versionParam,
                                          string paramName)
        {
            string error = null;
            int maxValue = short.MaxValue - 1;
            if (versionParam > maxValue)
                error = string.Format("is > {0}", maxValue);
            else if (versionParam < 0)
                error = "is < 0";
            if (error != null)
                throw new ArgumentException(string.Format("The parameter \"{0}\" {1}", paramName, error));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance from a system version.
        /// </summary>
        public VersionRelease(System.Version systemVersion)
        {
            if (systemVersion == null)
                throw new ArgumentNullException();
            this.version = systemVersion;
            this.release = Release.FromBuildNumber(systemVersion.Build);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance from an assembly's version.
        /// </summary>
        public VersionRelease(Assembly assembly)
            : this(GetAssemblyVersion(assembly))
        {
        }

        //---------------------------------------------------------------------

        private static System.Version GetAssemblyVersion(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException();
            return assembly.GetName().Version;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The major component of the version number.
        /// </summary>
        public int Major
        {
            get {
                return version.Major;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The minor component of the version number.
        /// </summary>
        public int Minor
        {
            get {
                return version.Minor;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The release component.
        /// </summary>
        public Release Release
        {
            get {
                return release;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The version and release represented as a single system version
        /// instance.
        /// </summary>
        public System.Version SystemVersion
        {
            get {
                return version;
            }
        }

        //---------------------------------------------------------------------

        public override string ToString()
        {
            string versionStr = string.Format("{0}.{1}", version.Major, version.Minor);
            string releaseStr = "";  // for official releases
            if (release == null) {
                //  Constructed from system version with a build number that
                //  did not represent a known release.
                releaseStr = string.Format("build {0}", version.Build);
            }
            else if (release.Type != Release.Types.Official) {
                releaseStr = release.ToString();
            }
            if (releaseStr.Length > 0)
                return string.Format("{0} ({1})", versionStr, releaseStr);
            else
                return versionStr;
        }
    }
}
