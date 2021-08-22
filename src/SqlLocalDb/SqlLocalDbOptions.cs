// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing options for using the SQL LocalDB API.
    /// </summary>
    public class SqlLocalDbOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbOptions"/> class.
        /// </summary>
        public SqlLocalDbOptions()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically delete the
        /// files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="false"/>.
        /// </remarks>
        public bool AutomaticallyDeleteInstanceFiles { get; set; }

        /// <summary>
        /// Gets or sets the override language to use to format error messages.
        /// </summary>
        public CultureInfo? Language { get; set; }

        /// <summary>
        /// Gets or sets the override version string of the native SQL LocalDB API to load, if any.
        /// </summary>
        public string NativeApiOverrideVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the options to use when stopping instances of SQL LocalDB.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="StopInstanceOptions.None"/>.
        /// </remarks>
        public StopInstanceOptions StopOptions { get; set; } = StopInstanceOptions.None;

        /// <summary>
        /// Gets or sets the default timeout to use when stopping instances of SQL LocalDB.
        /// </summary>
        /// <remarks>
        /// The default value is 1 minute.
        /// </remarks>
        public TimeSpan StopTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets the locale ID (LCID) to use for formatting error messages.
        /// </summary>
        internal int LanguageId
        {
            get
            {
                CultureInfo? culture = Language;

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
    }
}
