// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCategories.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ErrorAssert.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing the names of test categories.  This class cannot be inherited.
    /// </summary>
    internal static class TestCategories
    {
        /// <summary>
        /// The name of the test category for tests that should only run as part of a continuous integration.
        /// </summary>
        internal const string CIOnly = "CI Only";

        /// <summary>
        /// The name of the integration test category.
        /// </summary>
        internal const string Integration = "Integration";

        /// <summary>
        /// The name of the test category for tests that require administrative permissions to run.
        /// </summary>
        internal const string RequiresAdministrativePermissions = "Requires Administrative Permissions";

        /// <summary>
        /// The name of the test category for tests that specifically require SQL Server 2012 or test its behavior.
        /// </summary>
        internal const string SqlServer2012 = "SQL Server 2012";

        /// <summary>
        /// The name of the test category for tests that specifically require SQL Server 2014 or test its behavior.
        /// </summary>
        internal const string SqlServer2014 = "SQL Server 2014";
    }
}