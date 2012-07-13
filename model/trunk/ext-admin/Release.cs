using System;
using System.Reflection;

namespace Edu.Wisc.Forest.Flel.Util
{
    /// <summary>
    /// A release of a particular version of a software product.
    /// </summary>
    public class Release
    {
        /// <summary>
        /// Different types of releases
        /// </summary>
        public enum Types
        {
            Alpha = 100,
            Beta = 200,
            Candidate = 300,
            Official = 400
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The minimum release number for a non-official release.
        /// </summary>
        public const int MinNumber = 1;

        //---------------------------------------------------------------------

        /// <summary>
        /// The maximum release number for a non-official release.
        /// </summary>
        public const int MaxNumber = 99;

        //---------------------------------------------------------------------

        private const int AlphaMin = ((int) Types.Alpha) + MinNumber;
        private const int AlphaMax = ((int) Types.Alpha) + MaxNumber;

        private const int BetaMin = ((int) Types.Beta) + MinNumber;
        private const int BetaMax = ((int) Types.Beta) + MaxNumber;

        private const int CandidateMin = ((int) Types.Candidate) + MinNumber;
        private const int CandidateMax = ((int) Types.Candidate) + MaxNumber;

        //---------------------------------------------------------------------

        private Types type;
        private int number;
        private int build;

        //---------------------------------------------------------------------

        private Release(Types type,
                        int   number)
        {
            this.type = type;
            this.number = number;
            this.build = ((int) type) + number;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The release's type.
        /// </summary>
        public Types Type
        {
            get {
                return type;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The release's number.
        /// </summary>
        public int Number
        {
            get {
                return number;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The build number that represents the release.
        /// </summary>
        public int BuildNumber
        {
            get {
                return build;
            }
        }

        //---------------------------------------------------------------------

        private static int ValidateNumber(int number)
        {
            if (number < MinNumber)
                throw new ArgumentException(string.Format("Release number is < {0}", MinNumber));
            if (number < MaxNumber)
                throw new ArgumentException(string.Format("Release number is > {0}", MaxNumber));
            return number;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance for an alpha release.
        /// </summary>
        public static Release Alpha(int number)
        {
            return new Release(Types.Alpha, ValidateNumber(number));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance for a beta release.
        /// </summary>
        public static Release Beta(int number)
        {
            return new Release(Types.Beta, ValidateNumber(number));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance for a release candidate.
        /// </summary>
        public static Release Candidate(int number)
        {
            return new Release(Types.Candidate, ValidateNumber(number));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance for an official release.
        /// </summary>
        public static Release Official()
        {
            return new Release(Types.Official, 0);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance from a build number.
        /// </summary>
        /// <returns>
        /// null if the build number doesn't represent a release.
        /// </returns>
        public static Release FromBuildNumber(int build)
        {
            if (AlphaMin <= build && build <= AlphaMax)
                return new Release(Types.Alpha, build - (int) Types.Alpha);
            if (BetaMin <= build && build <= BetaMax)
                return new Release(Types.Beta, build - (int) Types.Beta);
            if (CandidateMin <= build && build <= CandidateMax)
                return new Release(Types.Candidate, build - (int) Types.Candidate);
            if (build == (int) Types.Official)
                return Official();
            return null;
        }

        //---------------------------------------------------------------------

        public override string ToString()
        {
            switch (type) {
                case Types.Alpha:
                    return string.Format("alpha {0} release", number);

                case Types.Beta:
                    return string.Format("beta {0} release", number);

                case Types.Candidate:
                    return string.Format("release candidate {0}", number);

                case Types.Official:
                    return "official release";

                default:
                    return string.Format("unknown release type: {0}", type);
            }
        }
    }
}
