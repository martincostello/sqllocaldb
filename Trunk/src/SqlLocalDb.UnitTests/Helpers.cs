// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="http://sqllocaldb.codeplex.com">
//   New BSD License (BSD)
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the New BSD Licence (BSD). 
//   A copy of the license can be found in the License.txt file at the root of this
//   distribution.  By using this source code in any fashion, you are agreeing to be
//   bound by the terms of the New BSD Licence. You must not remove this notice, or
//   any other, from this software.
// </license>
// <summary>
//   Helpers.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;
using System.Threading;
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