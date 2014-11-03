// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbConfigurationSectionTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbConfigurationSectionTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb.Configuration
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbConfigurationSection"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbConfigurationSectionTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbConfigurationSectionTests"/> class.
        /// </summary>
        public SqlLocalDbConfigurationSectionTests()
        {
        }

        [TestMethod]
        [Description("Tests that GetSection() returns an object with the correct default values if the configuration section is not defined.")]
        public void SqlLocalDbConfigurationSection_GetSection_Returns_Section_With_Defaults_If_Not_Defined()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    SqlLocalDbConfigurationSection result = SqlLocalDbConfigurationSection.GetSection();

                    // Assert
                    Assert.IsNotNull(result, "GetSection() returned null.");
                    Assert.AreEqual(false, result.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfigurationSection.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(false, result.IsAutomaticallyDeleteInstanceFilesSpecified, "SqlLocalDbConfigurationSection.IsAutomaticallyDeleteInstanceFilesSpecified is incorrect.");
                    Assert.AreEqual(false, result.IsNativeApiOverrideVersionSpecified, "SqlLocalDbConfigurationSection.IsNativeApiOverrideVersionSpecified is incorrect.");
                    Assert.AreEqual(string.Empty, result.NativeApiOverrideVersion, "SqlLocalDbConfigurationSection.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.None, result.StopOptions, "SqlLocalDbConfigurationSection.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromMinutes(1), result.StopTimeout, "SqlLocalDbConfigurationSection.StopTimeout is incorrect.");
                },
                configurationFile: "Empty.config");
        }

        [TestMethod]
        [Description("Tests that GetSection() returns an object with the correct default values if the configuration section is defined but no attributes are set.")]
        public void SqlLocalDbConfigurationSection_GetSection_Returns_Section_With_Defaults_If_Defined_But_No_Attributes_Specified()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    SqlLocalDbConfigurationSection result = SqlLocalDbConfigurationSection.GetSection();

                    // Assert
                    Assert.IsNotNull(result, "GetSection() returned null.");
                    Assert.AreEqual(false, result.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfigurationSection.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(false, result.IsAutomaticallyDeleteInstanceFilesSpecified, "SqlLocalDbConfigurationSection.IsAutomaticallyDeleteInstanceFilesSpecified is incorrect.");
                    Assert.AreEqual(false, result.IsNativeApiOverrideVersionSpecified, "SqlLocalDbConfigurationSection.IsNativeApiOverrideVersionSpecified is incorrect.");
                    Assert.AreEqual(string.Empty, result.NativeApiOverrideVersion, "SqlLocalDbConfigurationSection.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.None, result.StopOptions, "SqlLocalDbConfigurationSection.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromMinutes(1), result.StopTimeout, "SqlLocalDbConfigurationSection.StopTimeout is incorrect.");
                },
                configurationFile: @"Configuration\SqlLocalDbConfigurationSectionTests.DefinedButNotSpecified.config");
        }

        [TestMethod]
        [Description("Tests that GetSection() returns an object with the correct values if the configuration section is defined and all attributes are set.")]
        public void SqlLocalDbConfigurationSection_GetSection_Returns_Section_With_User_Values_If_Defined_And_All_Attributes_Specified()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    SqlLocalDbConfigurationSection result = SqlLocalDbConfigurationSection.GetSection();

                    // Assert
                    Assert.IsNotNull(result, "GetSection() returned null.");
                    Assert.AreEqual(true, result.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfigurationSection.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(true, result.IsAutomaticallyDeleteInstanceFilesSpecified, "SqlLocalDbConfigurationSection.IsAutomaticallyDeleteInstanceFilesSpecified is incorrect.");
                    Assert.AreEqual(true, result.IsNativeApiOverrideVersionSpecified, "SqlLocalDbConfigurationSection.IsNativeApiOverrideVersionSpecified is incorrect.");
                    Assert.AreEqual("11.0", result.NativeApiOverrideVersion, "SqlLocalDbConfigurationSection.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.KillProcess | StopInstanceOptions.NoWait, result.StopOptions, "SqlLocalDbConfigurationSection.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromSeconds(30), result.StopTimeout, "SqlLocalDbConfigurationSection.StopTimeout is incorrect.");
                },
                configurationFile: @"Configuration\SqlLocalDbConfigurationSectionTests.DefinedAndSpecified.config");
        }

        [TestMethod]
        [Description("Tests that the configuration properties of an instance of SqlLocalDbConfigurationSection can be set.")]
        public void SqlLocalDbConfigurationSection_Can_Set_Properties()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    bool automaticallyDeleteInstanceFiles = true;
                    string nativeApiOverrideVersion = "11.0";
                    StopInstanceOptions stopOptions = StopInstanceOptions.KillProcess | StopInstanceOptions.NoWait;
                    TimeSpan stopTimeout = TimeSpan.FromSeconds(30);

                    SqlLocalDbConfigurationSection target = new SqlLocalDbConfigurationSection();

                    // Act
                    target.AutomaticallyDeleteInstanceFiles = automaticallyDeleteInstanceFiles;
                    target.NativeApiOverrideVersion = nativeApiOverrideVersion;
                    target.StopOptions = stopOptions;
                    target.StopTimeout = stopTimeout;

                    // Assert
                    Assert.AreEqual(automaticallyDeleteInstanceFiles, target.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfigurationSection.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(nativeApiOverrideVersion, target.NativeApiOverrideVersion, "SqlLocalDbConfigurationSection.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(stopOptions, target.StopOptions, "SqlLocalDbConfigurationSection.StopOptions is incorrect.");
                    Assert.AreEqual(stopTimeout, target.StopTimeout, "SqlLocalDbConfigurationSection.StopTimeout is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests that the StopTimeout property of an instance of SqlLocalDbConfigurationSection cannot be set to less than TimeSpan.Zero.")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void SqlLocalDbConfigurationSection_StopTimeout_Cannot_Be_Less_Than_Zero()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    TimeSpan value = TimeSpan.FromTicks(-1);

                    SqlLocalDbConfigurationSection target = new SqlLocalDbConfigurationSection();

                    // Act and Assert
                    throw ErrorAssert.Throws<ConfigurationErrorsException>(
                        () => target.StopTimeout = value);
                });
        }
    }
}