// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbConfigurationSection.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbConfigurationSection.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;

namespace System.Data.SqlLocalDb.Configuration
{
    /// <summary>
    /// A class representing the configuration section for the <c>System.Data.SqlLocalDb</c> assembly.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class SqlLocalDbConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The name of the SQL LocalDB configuration section.
        /// </summary>
        public const string SectionName = "system.data.sqlLocalDb";

        /// <summary>
        /// The name of the <see cref="AutomaticallyDeleteInstanceFiles"/> configuration attribute.
        /// </summary>
        private const string AutomaticallyDeleteInstanceFilesAttributeName = "automaticallyDeleteInstanceFiles";

        /// <summary>
        /// The name of the <see cref="NativeApiOverrideVersion"/> configuration attribute.
        /// </summary>
        private const string NativeApiOverrideVersionAttributeName = "overrideVersion";

        /// <summary>
        /// The name of the <see cref="StopOptions"/> configuration attribute.
        /// </summary>
        private const string StopOptionsAttributeName = "stopOptions";

        /// <summary>
        /// The name of the <see cref="StopTimeout"/> configuration attribute.
        /// </summary>
        private const string StopTimeoutAttributeName = "stopTimeout";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbConfigurationSection"/> class.
        /// </summary>
        public SqlLocalDbConfigurationSection()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically delete the
        /// files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        [ConfigurationProperty(AutomaticallyDeleteInstanceFilesAttributeName, IsRequired = false, DefaultValue = false)]
        public bool AutomaticallyDeleteInstanceFiles
        {
            get { return (bool)base[AutomaticallyDeleteInstanceFilesAttributeName]; }
            set { base[AutomaticallyDeleteInstanceFilesAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the override version string of the native SQL LocalDB API to load, if any.
        /// </summary>
        [ConfigurationProperty(NativeApiOverrideVersionAttributeName, IsRequired = false, DefaultValue = "")]
        public string NativeApiOverrideVersion
        {
            get { return base[NativeApiOverrideVersionAttributeName] as string; }
            set { base[NativeApiOverrideVersionAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the options to use when stopping instances of SQL LocalDB.
        /// </summary>
        [ConfigurationProperty(StopOptionsAttributeName, IsRequired = false, DefaultValue = StopInstanceOptions.None)]
        public StopInstanceOptions StopOptions
        {
            get { return (StopInstanceOptions)base[StopOptionsAttributeName]; }
            set { base[StopOptionsAttributeName] = value; }
        }

        /// <summary>
        /// Gets or sets the default timeout to use when stopping instances of SQL LocalDB.
        /// </summary>
        [ConfigurationProperty(StopTimeoutAttributeName, IsRequired = false, DefaultValue = "00:01:00")]
        [TimeSpanValidator(MinValueString = "00:00:00")]
        public TimeSpan StopTimeout
        {
            get { return (TimeSpan)base[StopTimeoutAttributeName]; }
            set { base[StopTimeoutAttributeName] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="AutomaticallyDeleteInstanceFiles"/> is set by the application configuration file.
        /// </summary>
        internal bool IsAutomaticallyDeleteInstanceFilesSpecified
        {
            get { return IsPropertySetInConfigurationFile(AutomaticallyDeleteInstanceFilesAttributeName); }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NativeApiOverrideVersion"/> is set by the application configuration file.
        /// </summary>
        internal bool IsNativeApiOverrideVersionSpecified
        {
            get { return IsPropertySetInConfigurationFile(NativeApiOverrideVersionAttributeName); }
        }

        /// <summary>
        /// Returns the <see cref="SqlLocalDbConfigurationSection"/> from the current application configuration file.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlLocalDbConfigurationSection"/> read from the current application configuration file.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires loading the entire configuration file, which may be non-trivial.")]
        public static SqlLocalDbConfigurationSection GetSection()
        {
            SqlLocalDbConfigurationSection section = ConfigurationManager.GetSection(SectionName) as SqlLocalDbConfigurationSection;

            if (section == null)
            {
                section = new SqlLocalDbConfigurationSection();
            }

            return section;
        }

        /// <summary>
        /// Returns whether the configuration property with the specified name gets its
        /// value from it being explicitly set by the user in the application configuration file.
        /// </summary>
        /// <param name="propertyName">The name of the configuration attribute.</param>
        /// <returns>
        /// <see langword="true"/> if the configuration property value is defined in the
        /// application configuration file; otherwise <see langword="false"/>.
        /// </returns>
        private bool IsPropertySetInConfigurationFile(string propertyName)
        {
            return this.ElementInformation.Properties[propertyName].ValueOrigin == PropertyValueOrigin.SetHere;
        }
    }
}