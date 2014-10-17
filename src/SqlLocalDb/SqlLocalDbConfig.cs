// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbConfig.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApi.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing the configuration for the assembly. This class cannot be inherited.
    /// </summary>
    internal static class SqlLocalDbConfig
    {
        #region Fields

        /// <summary>
        /// Whether to automatically delete the files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        private static readonly bool DeleteInstanceFiles = LoadAutomaticDeletionValueFromConfig();

        /// <summary>
        /// The version string of the native SQL LocalDB API to load, if any.
        /// </summary>
        private static readonly string OverrideVersionString = ConfigurationManager.AppSettings["SQLLocalDB:OverrideVersion"];

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to automatically delete the files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        internal static bool AutomaticallyDeleteInstanceFiles
        {
            get { return DeleteInstanceFiles; }
        }

        /// <summary>
        /// Gets the version string of the native SQL LocalDB API to load, if any.
        /// </summary>
        internal static string NativeApiOverrideVersionString
        {
            get { return OverrideVersionString; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the default value of the <see cref="AutomaticallyDeleteInstanceFiles"/> property from the configuration file.
        /// </summary>
        /// <returns>
        /// The default value to use for the <see cref="AutomaticallyDeleteInstanceFiles"/> property.
        /// </returns>
        private static bool LoadAutomaticDeletionValueFromConfig()
        {
            string value = ConfigurationManager.AppSettings["SQLLocalDB:AutomaticallyDeleteInstanceFiles"];
            bool automaticallyDeleteInstanceFiles;

            if (string.IsNullOrEmpty(value) ||
                !bool.TryParse(value, out automaticallyDeleteInstanceFiles))
            {
                automaticallyDeleteInstanceFiles = false;
            }

            return automaticallyDeleteInstanceFiles;
        }

        #endregion
    }
}