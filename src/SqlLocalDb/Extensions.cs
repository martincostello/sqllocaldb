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

using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.SqlClient;
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
            
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(connectionStringSettings.ConnectionString);

            SqlConnectionStringBuilder providerBuilder = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString)
            {
                DataSource = instance.NamedPipe,
            };

            entityBuilder.ProviderConnectionString = providerBuilder.ConnectionString;
            return entityBuilder;
        }

        #endregion
    }
}