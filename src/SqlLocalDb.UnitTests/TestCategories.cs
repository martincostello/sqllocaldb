// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCategories.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
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
        #region Constants

        /// <summary>
        /// The name of the integration test category.
        /// </summary>
        internal const string Integration = "Integration";

        /// <summary>
        /// The name of the test category for test that require administrative permissions to run.
        /// </summary>
        internal const string RequiresAdministrativePermissions = "Requires Administrative Permissions";

        #endregion
    }
}