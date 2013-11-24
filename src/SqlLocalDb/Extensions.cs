// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
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
using System.Linq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for use with SQL LocalDB instances.  This class cannot be inherited.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class Extensions
    {
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
            const string InitialCatalogParameterName = "Initial Catalog";
            const string ProviderConnectionStringParameterName = "Provider Connection String";

            // First assume it's an SQL connection string
            object initialCatalogAsObject;
            string initialCatalog = null;

            if (value.TryGetValue(InitialCatalogParameterName, out initialCatalogAsObject))
            {
                initialCatalog = initialCatalogAsObject as string;
            }

            if (!string.IsNullOrEmpty(initialCatalog))
            {
                return initialCatalog;
            }

            object providerConnectionStringAsObject;
            string providerConnectionString = null;

            // If it wasn't SQL, see if it's an entity connection string
            // by trying to extract the provider connection string
            if (value.TryGetValue(ProviderConnectionStringParameterName, out providerConnectionStringAsObject))
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

                object initialCatalogFromProviderAsObject;

                // Try and extract the initial catalog from the provider's connection string
                if (builder.TryGetValue(InitialCatalogParameterName, out initialCatalogFromProviderAsObject))
                {
                    initialCatalog = initialCatalogFromProviderAsObject as string;
                }
            }

            // Derived types of DbConnectionStringBuilder may return the empty string instead of null
            // if they key is missing/not set, so in those cases explicitly return null instead.
            if (string.IsNullOrEmpty(initialCatalog))
            {
                return null;
            }

            return initialCatalog;
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

        #endregion
    }
}