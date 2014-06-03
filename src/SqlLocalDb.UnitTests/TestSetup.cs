// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetup.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TestSetup.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing the setup class for the test assembly.  This class cannot be inherited.
    /// </summary>
    [TestClass]
    public sealed class TestSetup
    {
        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="TestSetup"/> class from being created.
        /// </summary>
        private TestSetup()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the test assembly.
        /// </summary>
        /// <param name="context">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            SqlLocalDbApi.StopOptions = StopInstanceOptions.NoWait;
        }

        #endregion
    }
}