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
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (!new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Assert.Inconclusive("The current user '{0}' does not have Administrator privileges.", identity.Name);
                }
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

        #endregion
    }
}