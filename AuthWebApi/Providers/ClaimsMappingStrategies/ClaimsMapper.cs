using System;
using System.Globalization;

namespace AuthWebApi.Providers.ClaimsMappingStrategies
{
    public abstract class ClaimsMapper
    {
        private static CultureInfo _enUs = new CultureInfo("en-US");

        /// <summary>
        /// User's id in our DB. Can be empty for a non-registered user
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// User's full name. Can be empty
        /// </summary>
        public string FullName { get; protected set; }

        /// <summary>
        /// User's avatar url. Can be empty
        /// </summary>
        public string AvatarUrl { get; protected set; }

        /// <summary>
        /// User's id from External Provider. Can be empty
        /// </summary>
        public string Sid { get; protected set; }

        /// <summary>
        /// Version of the user's info.
        /// Used to detect password's change. 
        /// </summary>
        public string Version { get; protected set; }

        /// <summary>
        /// Issuer of the user's info
        /// </summary>
        public string Issuer { get; protected set; }

        /// <summary>
        /// Original issuer of the user's info.
        /// Example:
        ///  Step 1. user is authenticated via Google.
        ///    <see cref="Issuer"/> - Google.
        ///    <see cref="OriginalIssuer"/> - Google.
        ///  Step 2. user is registered in our DB
        ///    <see cref="Issuer"/> - Local.
        ///    <see cref="OriginalIssuer"/> - Google.
        /// </summary>
        public string OriginalIssuer { get; protected set; }

        public string GetVersion(DateTime timeStamp)
        {
            return timeStamp.Ticks.ToString(_enUs);
        }

        public static DateTime GetTimeStamp(string version)
        {
            if (string.IsNullOrEmpty(version))
                return new DateTime();

            return new DateTime(long.Parse(version, _enUs));
        }
    }
}