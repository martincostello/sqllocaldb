// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ISqlLocalDbInstanceManager"/> interface.  This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ISqlLocalDbInstanceManagerExtensions
    {
        /// <summary>
        /// Creates a connection to the LocalDB instance.
        /// </summary>
        /// <param name="manager">The <see cref="ISqlLocalDbInstanceManager"/> associated with the instance to create a connection to.</param>
        /// <returns>
        /// An instance of <see cref="SqlConnection"/> that can be used to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="manager"/> is <see langword="null"/>.
        /// </exception>
        public static SqlConnection CreateConnection(this ISqlLocalDbInstanceManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            return manager.GetInstanceInfo().CreateConnection();
        }

        /// <summary>
        /// Restarts the specified <see cref="ISqlLocalDbInstanceManager"/> instance.
        /// </summary>
        /// <param name="manager">The <see cref="ISqlLocalDbInstanceManager"/> associated with the instance to restart.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="manager"/> is <see langword="null"/>.
        /// </exception>
        public static void Restart(this ISqlLocalDbInstanceManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            manager.Stop();
            manager.Start();
        }
    }
}
