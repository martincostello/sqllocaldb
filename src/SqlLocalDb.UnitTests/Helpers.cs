// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Helpers.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        /// Invokes the specified delegate in a new <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="callBackDelegate">The delegate to invoke in the new <see cref="AppDomain"/>.</param>
        /// <param name="appDomainData">The optional data to set for the <see cref="AppDomain"/>.</param>
        /// <param name="configurationFile">The optional name of the configuration file to use.</param>
        /// <param name="callerMemberName">The optional name of the caller of this method.</param>
        public static void InvokeInNewAppDomain(
            CrossAppDomainDelegate callBackDelegate,
            IDictionary<string, object> appDomainData = null,
            string configurationFile = null,
            [CallerMemberName] string callerMemberName = null)
        {
            AppDomainSetup info = AppDomain.CurrentDomain.SetupInformation;

            if (!string.IsNullOrEmpty(configurationFile))
            {
                info.ConfigurationFile = configurationFile;
            }

            AppDomain domain = AppDomain.CreateDomain(
                callerMemberName,
                null,
                info);

            try
            {
                if (appDomainData != null)
                {
                    foreach (var pair in appDomainData)
                    {
                        domain.SetData(pair.Key, pair.Value);
                    }
                }

                if (Logger.DefaultLogger.GetType().IsMarshalByRef)
                {
                    // Create an instance of the type that configures logging in the new AppDomain
                    Type helperType = typeof(LoggingHelper);
                    var handle = domain.CreateInstanceFrom(helperType.Assembly.Location, helperType.FullName);

                    var helper = (LoggingHelper)handle.Unwrap();
                    helper.SetLogger(Logger.DefaultLogger);
                }

                domain.DoCallBack(callBackDelegate);
            }
            finally
            {
                AppDomain.Unload(domain);
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

        #region Classes

        /// <summary>
        /// A helper class that is used to configure logging in an <see cref="AppDomain"/> used by a test. This class cannot be inherited.
        /// </summary>
        [Serializable]
        private sealed class LoggingHelper : MarshalByRefObject
        {
            #region Methods

            /// <summary>
            /// Sets the <see cref="ILogger"/> implementation in use by the <see cref="AppDomain"/>.
            /// </summary>
            /// <param name="logger">The <see cref="ILogger"/> to use.</param>
            [Diagnostics.CodeAnalysis.SuppressMessage(
                "Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This is an instance method to allow calling it across AppDomain boundaries.")]
            internal void SetLogger(ILogger logger)
            {
                Logger.SetLogger(logger);
            }

            #endregion
        }

        #endregion
    }
}