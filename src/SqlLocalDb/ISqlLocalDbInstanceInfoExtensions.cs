// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ISqlLocalDbInstanceInfo"/> interface.  This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ISqlLocalDbInstanceInfoExtensions
    {
        /// <summary>
        /// Creates a connection to the LocalDB instance.
        /// </summary>
        /// <param name="instance">The SQL LocalDB instance to create a connection to.</param>
        /// <returns>
        /// An instance of <see cref="SqlConnection"/> that can be used to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The SQL LocalDB instance specified by <paramref name="instance"/> is not running.
        /// </exception>
        public static SqlConnection CreateConnection(this ISqlLocalDbInstanceInfo instance)
        {
            SqlConnectionStringBuilder builder = instance.CreateConnectionStringBuilder();
            return new SqlConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <param name="instance">The SQL LocalDB instance to create a connection string builder for.</param>
        /// <returns>
        /// An instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The SQL LocalDB instance specified by <paramref name="instance"/> is not running.
        /// </exception>
        public static SqlConnectionStringBuilder CreateConnectionStringBuilder(this ISqlLocalDbInstanceInfo instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!instance.IsRunning)
            {
                string message = SRHelper.Format(SR.ISqlLocalDbInstanceInfoExtensions_NotRunningFormat, instance.Name);
                throw new InvalidOperationException(message);
            }

            return new SqlConnectionStringBuilder()
            {
                DataSource = instance.NamedPipe,
            };
        }

        /// <summary>
        /// Gets the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <param name="instance">The SQL LocalDB instance to get the default connection string for.</param>
        /// <returns>
        /// The default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The SQL LocalDB instance specified by <paramref name="instance"/> is not running.
        /// </exception>
        public static string GetConnectionString(this ISqlLocalDbInstanceInfo instance)
            => instance.CreateConnectionStringBuilder().ConnectionString;

        /// <summary>
        /// Returns an <see cref="ISqlLocalDbInstanceManager"/> that can be used to manage the instance.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstanceInfo"/> to manage.</param>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceManager"/> that can be used to manage the SQL LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="instance"/> does not implement <see cref="ISqlLocalDbApiAdapter"/>.
        /// </exception>
        public static ISqlLocalDbInstanceManager Manage(this ISqlLocalDbInstanceInfo instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (instance is ISqlLocalDbApiAdapter adapter)
            {
                return new SqlLocalDbInstanceManager(instance, adapter.LocalDb);
            }

            string message = SRHelper.Format(
                SR.ISqlLocalDbInstanceInfoExtensions_DoesNotImplementAdapterFormat,
                nameof(ISqlLocalDbInstanceInfo),
                nameof(ISqlLocalDbApiAdapter));

            throw new ArgumentException(message, nameof(instance));
        }
    }
}
