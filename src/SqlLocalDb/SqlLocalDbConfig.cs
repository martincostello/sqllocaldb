// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbConfig.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApi.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using System.Data.SqlLocalDb.Configuration;
using System.Globalization;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing the configuration for the assembly. This class cannot be inherited.
    /// </summary>
    internal static class SqlLocalDbConfig
    {
        /// <summary>
        /// The name of the legacy application configuration setting for specifying whether to automatically delete instance files.
        /// </summary>
        private const string LegacyDeleteInstanceFilesSettingName = "SQLLocalDB:AutomaticallyDeleteInstanceFiles";

        /// <summary>
        /// The name of the legacy application configuration setting for overriding the native SQL LocalDB API version to use.
        /// </summary>
        private const string LegacyOverrideVersionSettingName = "SQLLocalDB:OverrideVersion";

        /// <summary>
        /// The current <see cref="SqlLocalDbConfigurationSection"/> loaded from the application configuration file.
        /// </summary>
        private static readonly SqlLocalDbConfigurationSection ConfigSection = SqlLocalDbConfigurationSection.GetSection();

        /// <summary>
        /// Gets a value indicating whether to automatically delete the files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        internal static bool AutomaticallyDeleteInstanceFiles
        {
            // Use the value fron app.config first, then if not specified try the legacy appSettings setting value
            get { return ConfigSection.IsAutomaticallyDeleteInstanceFilesSpecified ? ConfigSection.AutomaticallyDeleteInstanceFiles : LoadAutomaticDeletionValueFromConfig(); }
        }

        /// <summary>
        /// Gets the locale ID (LCID) to use for formatting error messages.
        /// </summary>
        internal static int LanguageId
        {
            get
            {
                CultureInfo culture = ConfigSection.Language;

                if (culture == null)
                {
                    // Zero is used by SQL LocalDB to mean to defer to the OS configuration
                    return 0;
                }

                // N.B. No checks as to the support of the configured culture's LCID for use
                // by SQL LocalDB are made here, it is left to the user to ensure that the
                // culture code they configure is supported. From experimentation, SQL LocalDB
                // supports the "main" language/region for cultures and not the neutral culture.
                // For example:
                // Supported: de-DE, en-US, es-ES, fr-FR;
                // Not supported: de, en, en-GB, es, es-MX, fr, fr-CA.
                return culture.LCID;
            }
        }

        /// <summary>
        /// Gets the type of the <see cref="ILogger"/> implementation to use, if any.
        /// </summary>
        internal static Type LoggerType
        {
            get { return ConfigSection.LoggerType; }
        }

        /// <summary>
        /// Gets the version string of the native SQL LocalDB API to load, if any.
        /// </summary>
        internal static string NativeApiOverrideVersionString
        {
            // Use the value fron app.config first, then if not specified try the legacy appSettings setting value
            get { return ConfigSection.IsNativeApiOverrideVersionSpecified ? ConfigSection.NativeApiOverrideVersion : (ConfigurationManager.AppSettings["SQLLocalDB:OverrideVersion"] ?? string.Empty); }
        }

        /// <summary>
        /// Gets the options to use when stopping instances of SQL LocalDB.
        /// </summary>
        internal static StopInstanceOptions StopOptions
        {
            get { return ConfigSection.StopOptions; }
        }

        /// <summary>
        /// Gets the default timeout to use when stopping instances of SQL LocalDB.
        /// </summary>
        internal static TimeSpan StopTimeout
        {
            get { return ConfigSection.StopTimeout; }
        }

        /// <summary>
        /// Loads the default value of the <see cref="AutomaticallyDeleteInstanceFiles"/> property from the application setting section of the configuration file.
        /// </summary>
        /// <returns>
        /// The default value to use for the <see cref="AutomaticallyDeleteInstanceFiles"/> property.
        /// </returns>
        private static bool LoadAutomaticDeletionValueFromConfig()
        {
            string value = ConfigurationManager.AppSettings[LegacyDeleteInstanceFilesSettingName];
            bool automaticallyDeleteInstanceFiles;

            if (string.IsNullOrEmpty(value) ||
                !bool.TryParse(value, out automaticallyDeleteInstanceFiles))
            {
                automaticallyDeleteInstanceFiles = false;
            }

            return automaticallyDeleteInstanceFiles;
        }
    }
}
