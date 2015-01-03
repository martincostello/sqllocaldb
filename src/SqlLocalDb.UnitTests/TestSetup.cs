// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetup.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TestSetup.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing the setup class for the test assembly.  This class cannot be inherited.
    /// </summary>
    [TestClass]
    public sealed class TestSetup
    {
        #region Fields

        /// <summary>
        /// The SQL LocalDB instance names that existed at the start of the test run.
        /// </summary>
        private static readonly List<string> InstanceNames = new List<string>();

        #endregion

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

            // Store the names of all the SQL LocalDB instance folders that existed before the tests are run
            string[] instanceNames = GetInstanceNames();
            InstanceNames.AddRange(instanceNames);
        }

        /// <summary>
        /// Cleans up the test assembly.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Get the names of all the SQL LocalDB instance folders that existed after the tests are run
            string[] instanceNames = GetInstanceNames();

            // Filter the list down to just the names of the instances that were created in the test run
            string[] createdInstanceNames = instanceNames
                .Except(InstanceNames)
                .ToArray();

            // Try and delete all the leftover file(s) from the test run
            foreach (string instanceName in createdInstanceNames)
            {
                SqlLocalDbApi.DeleteInstanceFiles(instanceName);
            }
        }

        /// <summary>
        /// Returns the SQL LocalDB instance names using the instance folder names on the current machine.
        /// </summary>
        /// <returns>
        /// An <see cref="Array"/> containing the names of all the instance folder on the current machine.
        /// </returns>
        private static string[] GetInstanceNames()
        {
            string path = SqlLocalDbApi.GetInstancesFolderPath();
            return Directory.GetDirectories(path);
        }

        #endregion
    }
}