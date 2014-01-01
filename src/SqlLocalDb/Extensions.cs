// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Extensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for use with SQL LocalDB instances.  This class cannot be inherited.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class Extensions
    {
        #region Constants

        /// <summary>
        /// The connection string keyword for the attached database file name.
        /// </summary>
        private const string AttachDBFilenameKeywordName = "AttachDBFilename";

        /// <summary>
        /// The connection string keyword for the Initial Catalog.
        /// </summary>
        private const string InitialCatalogKeywordName = "Initial Catalog";

        /// <summary>
        /// The connection string keyword for the Provider Connection String.
        /// </summary>
        private const string ProviderConnectionStringKeywordName = "Provider Connection String";

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of <see cref="DbConnection"/> that can be used to connect to the SQL LocalDB instance
        /// using the only connection string configured in the current application configuration file.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No connection strings are configured in the application configuration file or more than one
        /// connection string is configured in the application configuration file.
        /// </exception>
        /// <remarks>
        /// Connection strings may be inherited from outside the current application's configuration file, such as from
        /// <c>machine.config</c>.  To use this overload it is recommended that, unless otherwise needed, a <c>&lt;clear/&gt;</c>
        /// element is added to the <c>&lt;connectionStrings&gt;</c> section of your application configuration file to
        /// prevent the default connection strings from being inherited.
        /// </remarks>
        public static DbConnection GetConnectionForDefaultModel(this ISqlLocalDbInstance instance)
        {
            DbConnectionStringBuilder builder = instance.GetConnectionStringForDefaultModel();
            return new EntityConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnection"/> that can be used to connect to the SQL LocalDB instance
        /// using the only connection string configured in the current application configuration file.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="initialCatalog">The optional name to use for the Initial Catalog in the provider connection string to override the default, if present.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No connection strings are configured in the application configuration file or more than one
        /// connection string is configured in the application configuration file.
        /// </exception>
        /// <remarks>
        /// Connection strings may be inherited from outside the current application's configuration file, such as from
        /// <c>machine.config</c>.  To use this overload it is recommended that, unless otherwise needed, a <c>&lt;clear/&gt;</c>
        /// element is added to the <c>&lt;connectionStrings&gt;</c> section of your application configuration file to
        /// prevent the default connection strings from being inherited.
        /// </remarks>
        public static DbConnection GetConnectionForDefaultModel(this ISqlLocalDbInstance instance, string initialCatalog)
        {
            DbConnectionStringBuilder builder = instance.GetConnectionStringForDefaultModel(initialCatalog);
            return new EntityConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnection"/> that can be used to connect to the SQL LocalDB instance.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="modelConnectionStringName">The name of the connection string for the model in the application configuration file.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No connection string with the name specified by <paramref name="modelConnectionStringName"/> can be found in the application configuration file.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public static DbConnection GetConnectionForModel(this ISqlLocalDbInstance instance, string modelConnectionStringName)
        {
            DbConnectionStringBuilder builder = instance.GetConnectionStringForModel(modelConnectionStringName);
            return new EntityConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnection"/> that can be used to connect to the SQL LocalDB instance.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="modelConnectionStringName">The name of the connection string for the model in the application configuration file.</param>
        /// <param name="initialCatalog">The optional name to use for the Initial Catalog in the provider connection string to override the default, if present.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No connection string with the name specified by <paramref name="modelConnectionStringName"/> can be found in the application configuration file.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public static DbConnection GetConnectionForModel(this ISqlLocalDbInstance instance, string modelConnectionStringName, string initialCatalog)
        {
            DbConnectionStringBuilder builder = instance.GetConnectionStringForModel(modelConnectionStringName, initialCatalog);
            return new EntityConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnectionStringBuilder"/> that can be used to connect to the SQL LocalDB instance
        /// using the only connection string configured in the current application configuration file.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No connection strings are configured in the application configuration file or more than one
        /// connection string is configured in the application configuration file.
        /// </exception>
        /// <remarks>
        /// Connection strings may be inherited from outside the current application's configuration file, such as from
        /// <c>machine.config</c>.  To use this overload it is recommended that, unless otherwise needed, a <c>&lt;clear/&gt;</c>
        /// element is added to the <c>&lt;connectionStrings&gt;</c> section of your application configuration file to
        /// prevent the default connection strings from being inherited.
        /// </remarks>
        public static DbConnectionStringBuilder GetConnectionStringForDefaultModel(this ISqlLocalDbInstance instance)
        {
            return instance.GetConnectionStringForDefaultModel(null);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnectionStringBuilder"/> that can be used to connect to the SQL LocalDB instance
        /// using the only connection string configured in the current application configuration file.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="initialCatalog">The optional name to use for the Initial Catalog in the provider connection string to override the default, if present.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No connection strings are configured in the application configuration file or more than one
        /// connection string is configured in the application configuration file.
        /// </exception>
        /// <remarks>
        /// Connection strings may be inherited from outside the current application's configuration file, such as from
        /// <c>machine.config</c>.  To use this overload it is recommended that, unless otherwise needed, a <c>&lt;clear/&gt;</c>
        /// element is added to the <c>&lt;connectionStrings&gt;</c> section of your application configuration file to
        /// prevent the default connection strings from being inherited.
        /// </remarks>
        public static DbConnectionStringBuilder GetConnectionStringForDefaultModel(this ISqlLocalDbInstance instance, string initialCatalog)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var connectionStrings = ConfigurationManager.ConnectionStrings
                .OfType<ConnectionStringSettings>()
                .ToList();

            if (connectionStrings.Count < 1)
            {
                throw new InvalidOperationException(SR.Extensions_NoConnectionStrings);
            }
            else if (connectionStrings.Count > 1)
            {
                throw new InvalidOperationException(SR.Extensions_NoSingleConnectionString);
            }

            return CreateBuilder(connectionStrings[0], instance.NamedPipe, initialCatalog);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnectionStringBuilder"/> that can be used to connect to the SQL LocalDB instance.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="modelConnectionStringName">The name of the connection string for the model in the application configuration file.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No connection string with the name specified by <paramref name="modelConnectionStringName"/> can be found in the application configuration file.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public static DbConnectionStringBuilder GetConnectionStringForModel(this ISqlLocalDbInstance instance, string modelConnectionStringName)
        {
            return instance.GetConnectionStringForModel(modelConnectionStringName, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnectionStringBuilder"/> that can be used to connect to the SQL LocalDB instance.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> to get the connection string builder for.</param>
        /// <param name="modelConnectionStringName">The name of the connection string for the model in the application configuration file.</param>
        /// <param name="initialCatalog">The optional name to use for the Initial Catalog in the provider connection string to override the default, if present.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No connection string with the name specified by <paramref name="modelConnectionStringName"/> can be found in the application configuration file.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public static DbConnectionStringBuilder GetConnectionStringForModel(this ISqlLocalDbInstance instance, string modelConnectionStringName, string initialCatalog)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var connectionStringSettings = ConfigurationManager.ConnectionStrings
                .OfType<ConnectionStringSettings>()
                .Where((p) => string.Equals(p.Name, modelConnectionStringName, StringComparison.Ordinal))
                .FirstOrDefault();

            if (connectionStringSettings == null)
            {
                throw new ArgumentException(SRHelper.Format(SR.Extensions_NoConnectionStringFormat, modelConnectionStringName), "modelConnectionStringName");
            }

            return CreateBuilder(connectionStringSettings, instance.NamedPipe, initialCatalog);
        }

        /// <summary>
        /// Gets the Initial Catalog name from the specified <see cref="DbConnectionStringBuilder"/>, if present.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to extract the Initial Catalog name from.</param>
        /// <returns>
        /// The name of the Initial Catalog present in <paramref name="value"/>, if any; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetInitialCatalogName(this DbConnectionStringBuilder value)
        {
            return value.ExtractStringValueFromConnectionString(InitialCatalogKeywordName);
        }

        /// <summary>
        /// Gets the physical file name from the specified <see cref="DbConnectionStringBuilder"/>, if present.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to extract the physical file name from.</param>
        /// <returns>
        /// The name of the physical file name present in <paramref name="value"/>, if any; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetPhysicalFileName(this DbConnectionStringBuilder value)
        {
            return value.ExtractStringValueFromConnectionString(AttachDBFilenameKeywordName);
        }

        /// <summary>
        /// Gets an SQL Local DB instance with the specified name if it exists, otherwise a new instance with the specified name is created.
        /// </summary>
        /// <param name="value">The <see cref="ISqlLocalDbProvider"/> to use to get or create the instance.</param>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to get or create.</param>
        /// <returns>
        /// An SQL Local DB instance with the name specified by <paramref name="instanceName"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="value"/> returns <see langword="null"/> when queried for instances.
        /// </exception>
        public static ISqlLocalDbInstance GetOrCreateInstance(this ISqlLocalDbProvider value, string instanceName)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            bool instanceExists = false;
            IList<ISqlLocalDbInstanceInfo> instances = value.GetInstances();

            if (instances != null)
            {
                // Instance names in SQL Local DB are case-insensitive
                instanceExists = instances
                    .Where((p) => p != null)
                    .Where((p) => string.Equals(p.Name, instanceName, StringComparison.OrdinalIgnoreCase))
                    .Any();
            }

            if (instanceExists)
            {
                return value.GetInstance(instanceName);
            }
            else
            {
                return value.CreateInstance(instanceName);
            }
        }

        /// <summary>
        /// Sets the Initial Catalog name in the specified <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to set the Initial Catalog name for.</param>
        /// <param name="initialCatalog">The name of the Initial Catalog to set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static void SetInitialCatalogName(this DbConnectionStringBuilder value, string initialCatalog)
        {
            value.SetKeywordValueAsString(InitialCatalogKeywordName, initialCatalog);
        }

        /// <summary>
        /// Sets the physical database file name in the specified <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to set the physical database file name for.</param>
        /// <param name="fileName">The physical file name to set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/> or invalid.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="fileName"/> references the Data Directory for the current <see cref="AppDomain"/>
        /// but the Data Directory for the current <see cref="AppDomain"/> has no value set.
        /// </exception>
        public static void SetPhysicalFileName(this DbConnectionStringBuilder value, string fileName)
        {
            string fullPath = null;

            if (fileName != null)
            {
                fullPath = ExpandDataDirectoryIfPresent(fileName);

                try
                {
                    // Only full file paths are supported, so ensure that the path is fully qualified before it is set
                    fullPath = Path.GetFullPath(fullPath);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(SRHelper.Format(SR.Extensions_InvalidPathFormat, ex.Message), "fileName", ex);
                }
                catch (NotSupportedException ex)
                {
                    throw new ArgumentException(SRHelper.Format(SR.Extensions_InvalidPathFormat, ex.Message), "fileName", ex);
                }
            }

            value.SetKeywordValueAsString(AttachDBFilenameKeywordName, fullPath);
        }

        /// <summary>
        /// Creates an instance of <see cref="DbConnectionStringBuilder"/> using the specified base
        /// connection string, SQL Local DB named pipe and optional Initial Catalog name.
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings for the base connection string.</param>
        /// <param name="namedPipe">The SQL Local DB named pipe to use as the data source.</param>
        /// <param name="initialCatalog">The optional name to use for the Initial Catalog in the provider connection string to override the default, if present.</param>
        /// <returns>
        /// The created instance of <see cref="DbConnectionStringBuilder"/>.
        /// </returns>
        private static DbConnectionStringBuilder CreateBuilder(
            ConnectionStringSettings connectionStringSettings,
            string namedPipe,
            string initialCatalog)
        {
            Debug.Assert(connectionStringSettings != null, "connectionStringSettings cannot be null.");
            Debug.Assert(namedPipe != null, "namedPipe cannot be null.");

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(connectionStringSettings.ConnectionString);

            SqlConnectionStringBuilder providerBuilder = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString)
            {
                DataSource = namedPipe,
            };

            if (!string.IsNullOrEmpty(initialCatalog))
            {
                providerBuilder.InitialCatalog = initialCatalog;
            }

            entityBuilder.ProviderConnectionString = providerBuilder.ConnectionString;
            return entityBuilder;
        }

        /// <summary>
        /// Expands any reference to the Data Directory for the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="fileName">The file name to expand.</param>
        /// <returns>
        /// The expanded representation of <paramref name="fileName"/> if the Data Directory for
        /// the current <see cref="AppDomain"/> is referenced and set; otherwise <paramref name="fileName"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The Data Directory is not set for the current <see cref="AppDomain"/>.
        /// </exception>
        private static string ExpandDataDirectoryIfPresent(string fileName)
        {
            const string DataDirectorySubstitution = "|DataDirectory|";

            if (fileName.Contains(DataDirectorySubstitution))
            {
                string dataDirectoryPath = AppDomain.CurrentDomain.GetData("DataDirectory") as string;

                if (dataDirectoryPath == null)
                {
                    throw new NotSupportedException(SR.Extensions_NoAppDomainDataDirectory);
                }

                fileName = fileName.Replace(DataDirectorySubstitution, dataDirectoryPath);
            }

            return fileName;
        }

        /// <summary>
        /// Extracts the <see cref="String"/> value of the specified keyword from the specified <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to extract the value from.</param>
        /// <param name="keyword">The keyword to extract the value of.</param>
        /// <returns>
        /// The <see cref="String"/> value of the keyword specified by <paramref name="keyword"/> present in <paramref name="value"/>, if any; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        private static string ExtractStringValueFromConnectionString(this DbConnectionStringBuilder value, string keyword)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // N.B. Keywords are used here rather than the strongly-typed derived classes
            // of DbConnectionStringBuilder.  This is so that custom derived classes can be
            // used and also so that both of the following entity connection string builders
            // can be used without using reflection and hard-coded type names:
            // 1) System.Data.EntityClient.EntityClientConnectionStringBuilder (System.Data.Entity.dll)
            // 2) System.Data.Entity.Core.EntityClient.EntityClientConnectionStringBuilder (EntityFramework.dll)

            // First assume it's an SQL connection string
            object resultAsObject;
            string result = null;

            if (value.TryGetValue(keyword, out resultAsObject))
            {
                result = resultAsObject as string;
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            object providerConnectionStringAsObject;
            string providerConnectionString = null;

            // If it wasn't SQL, see if it's an entity connection string
            // by trying to extract the provider connection string
            if (value.TryGetValue(ProviderConnectionStringKeywordName, out providerConnectionStringAsObject))
            {
                // It wasn't an entity connection string, nothing further to try
                providerConnectionString = providerConnectionStringAsObject as string;
            }

            if (!string.IsNullOrEmpty(providerConnectionString))
            {
                // Build a connection string from the provider connection string
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder()
                {
                    ConnectionString = providerConnectionString,
                };

                object resultFromProviderAsObject;

                // Try and extract the initial catalog from the provider's connection string
                if (builder.TryGetValue(keyword, out resultFromProviderAsObject))
                {
                    result = resultFromProviderAsObject as string;
                }
            }

            // Derived types of DbConnectionStringBuilder may return the empty string instead of null
            // if they key is missing/not set, so in those cases explicitly return null instead.
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Sets the specified keyword's value to the specified <see cref="String"/> value in the specified <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="DbConnectionStringBuilder"/> to set the keyword value for.</param>
        /// <param name="keyword">The keyword to set the value for.</param>
        /// <param name="keywordValue">The value to set the keyword to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        private static void SetKeywordValueAsString(this DbConnectionStringBuilder value, string keyword, string keywordValue)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // N.B. Keywords are used here rather than the strongly-typed derived classes
            // of DbConnectionStringBuilder.  This is so that custom derived classes can be
            // used and also so that both of the following entity connection string builders
            // can be used without using reflection and hard-coded type names:
            // 1) System.Data.EntityClient.EntityClientConnectionStringBuilder (System.Data.Entity.dll)
            // 2) System.Data.Entity.Core.EntityClient.EntityClientConnectionStringBuilder (EntityFramework.dll)
            object providerConnectionStringAsObject;

            if (value.TryGetValue(ProviderConnectionStringKeywordName, out providerConnectionStringAsObject))
            {
                string providerConnectionString = providerConnectionStringAsObject as string;

                if (!string.IsNullOrEmpty(providerConnectionString))
                {
                    // Build a connection string from the provider connection string
                    DbConnectionStringBuilder builder = new DbConnectionStringBuilder()
                    {
                        ConnectionString = providerConnectionString,
                    };

                    // Set the keyword value in the Provider Connection String and replace
                    // the initial Provider Connection String with the updated one
                    builder[keyword] = keywordValue;
                    value[ProviderConnectionStringKeywordName] = builder.ConnectionString;
                }
            }
            else
            {
                value[keyword] = keywordValue;
            }
        }

        #endregion
    }
}