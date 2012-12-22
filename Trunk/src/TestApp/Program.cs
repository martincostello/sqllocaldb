// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="http://sqllocaldb.codeplex.com">
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
//   Program.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Principal;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// An application that acts as a test harness for the <c>System.Data.SqlLocalDb</c> assembly.
    /// </summary>
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "It isn't.")]
        internal static void Main()
        {
            if (!SqlLocalDbApi.IsLocalDBInstalled())
            {
                Console.WriteLine(SR.SqlLocalDbApi_NotInstalledFormat, Environment.MachineName);
                return;
            }

            ISqlLocalDbProvider factory = new SqlLocalDbProvider();

            IList<ISqlLocalDbVersionInfo> versions = factory.GetVersions();

            Console.WriteLine(Strings.Program_VersionsListHeader);
            Console.WriteLine();

            foreach (ISqlLocalDbVersionInfo version in versions)
            {
                Console.WriteLine(version.Name);
            }

            Console.WriteLine();

            IList<ISqlLocalDbInstanceInfo> instances = factory.GetInstances();

            Console.WriteLine(Strings.Program_InstancesListHeader);
            Console.WriteLine();

            foreach (ISqlLocalDbInstanceInfo instance in instances)
            {
                Console.WriteLine(instance.Name);
            }

            Console.WriteLine();

            string instanceName = Guid.NewGuid().ToString();

            ISqlLocalDbInstance localDb = factory.CreateInstance(instanceName);

            localDb.Start();

            try
            {
                // SQL LocalDB will let you call Share() successfully if the process
                // is not running elevated, but won't actually share the instance, causing
                // the complementary call to Unshare() to fail.
                if (IsCurrentUserAdmin())
                {
                    localDb.Share(Guid.NewGuid().ToString());
                }

                try
                {
                    using (SqlConnection connection = localDb.CreateConnection())
                    {
                        connection.Open();

                        try
                        {
                            using (SqlCommand command = new SqlCommand("create database [MyDatabase]", connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            using (SqlCommand command = new SqlCommand("drop database [MyDatabase]", connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                finally
                {
                    if (IsCurrentUserAdmin())
                    {
                        localDb.Unshare();
                    }
                }
            }
            finally
            {
                localDb.Stop();
                SqlLocalDbInstance.Delete(localDb);
            }

            Console.WriteLine();
            Console.Write(Strings.Program_ExitPrompt);
            Console.ReadKey();
        }

        /// <summary>
        /// Returns whether the current user is in the administrators group on the local machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current user is in the administrators
        /// group on the local machine; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsCurrentUserAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        #endregion
    }
}