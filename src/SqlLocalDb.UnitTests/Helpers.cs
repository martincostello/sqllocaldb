// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Helpers.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit test helper methods.  This class cannot be inherited.
    /// </summary>
    internal static class Helpers
    {
        #region Methods

        /// <summary>
        /// Ensures that the current user has Administrative privileges.
        /// </summary>
        /// <remarks>
        /// Any unit test calling this method is marked as Inconclusive if
        /// the current user does not have administrative privileges.
        /// </remarks>
        public static void EnsureUserIsAdmin()
        {
            string name;

            if (!IsCurrentUserAdmin(out name))
            {
                Assert.Inconclusive("The current user '{0}' does not have administrative privileges.", name);
            }
        }

        /// <summary>
        /// Ensures that SQL Server LocalDB is installed on the current machine.
        /// </summary>
        /// <remarks>
        /// Any unit test calling this method is marked as Inconclusive if SQL
        /// Server LocalDB is not installed on the local machine.
        /// </remarks>
        public static void EnsureLocalDBInstalled()
        {
            if (!SqlLocalDbApi.IsLocalDBInstalled())
            {
                Assert.Inconclusive("SQL Server LocalDB is not installed on {0}.", Environment.MachineName);
            }
        }

        /// <summary>
        /// Returns whether the current user has Administrative privileges.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current user has Administrative
        /// privileges; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsCurrentUserAdmin()
        {
            string name;
            return IsCurrentUserAdmin(out name);
        }

        /// <summary>
        /// Returns whether the current user has Administrative privileges.
        /// </summary>
        /// <param name="name">When the method returns, contains the name of the current user.</param>
        /// <returns>
        /// <see langword="true"/> if the current user has Administrative
        /// privileges; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsCurrentUserAdmin(out string name)
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                name = identity.Name;
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        #endregion
    }
}