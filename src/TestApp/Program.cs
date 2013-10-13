// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
// </copyright>
// <license>
//   See license.txt in the project root for license information.
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
            ISqlLocalDbApi localDB = new SqlLocalDbApiWrapper();

            if (!localDB.IsLocalDBInstalled())
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

            foreach (ISqlLocalDbInstanceInfo instanceInfo in instances)
            {
                Console.WriteLine(instanceInfo.Name);
            }

            Console.WriteLine();

            string instanceName = Guid.NewGuid().ToString();

            ISqlLocalDbInstance instance = factory.CreateInstance(instanceName);

            instance.Start();

            try
            {
                // SQL LocalDB will let you call Share() successfully if the process
                // is not running elevated, but won't actually share the instance, causing
                // the complementary call to Unshare() to fail.
                if (IsCurrentUserAdmin())
                {
                    instance.Share(Guid.NewGuid().ToString());
                }

                try
                {
                    using (SqlConnection connection = instance.CreateConnection())
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
                        instance.Unshare();
                    }
                }
            }
            finally
            {
                instance.Stop();
                localDB.DeleteInstance(instance.Name);
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