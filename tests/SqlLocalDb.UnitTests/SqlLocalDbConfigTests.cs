// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbConfigTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbConfigTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbConfig"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbConfigTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbConfigTests"/> class.
        /// </summary>
        public SqlLocalDbConfigTests()
        {
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbConfig uses the correct default values if the configuration section is not defined.")]
        public void SqlLocalDbConfig_Uses_Defaults_If_Not_Defined()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(false, SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(0, SqlLocalDbConfig.LanguageId, "SqlLocalDbConfig.LanguageId is incorrect.");
                    Assert.AreEqual(string.Empty, SqlLocalDbConfig.NativeApiOverrideVersionString, "SqlLocalDbConfig.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.None, SqlLocalDbConfig.StopOptions, "SqlLocalDbConfig.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromMinutes(1), SqlLocalDbConfig.StopTimeout, "SqlLocalDbConfig.StopTimeout is incorrect.");
                },
                configurationFile: "Empty.config");
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbConfig uses the correct default values if the configuration section is defined but no attributes are set.")]
        public void SqlLocalDbConfig_Uses_Defaults_If_Defined_But_No_Attributes_Specified()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(false, SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(0, SqlLocalDbConfig.LanguageId, "SqlLocalDbConfig.LanguageId is incorrect.");
                    Assert.AreEqual(string.Empty, SqlLocalDbConfig.NativeApiOverrideVersionString, "SqlLocalDbConfig.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.None, SqlLocalDbConfig.StopOptions, "SqlLocalDbConfig.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromMinutes(1), SqlLocalDbConfig.StopTimeout, "SqlLocalDbConfig.StopTimeout is incorrect.");
                },
                configurationFile: "SqlLocalDbConfigTests.DefinedButNotSpecified.config");
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbConfig uses the correct default values if the configuration section is defined and no attributes are set but the legacy appSettings override settings are present.")]
        public void SqlLocalDbConfig_Uses_Defaults_If_Defined_But_No_Attributes_Specified_And_Legacy_AppSettings_Present()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(true, SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(0, SqlLocalDbConfig.LanguageId, "SqlLocalDbConfig.LanguageId is incorrect.");
                    Assert.AreEqual("11.0", SqlLocalDbConfig.NativeApiOverrideVersionString, "SqlLocalDbConfig.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.None, SqlLocalDbConfig.StopOptions, "SqlLocalDbConfig.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromMinutes(1), SqlLocalDbConfig.StopTimeout, "SqlLocalDbConfig.StopTimeout is incorrect.");
                },
                configurationFile: "SqlLocalDbConfigTests.DefinedButNotSpecifiedWithLegacySettings.config");
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbConfig uses the correct values if the configuration section is defined and the attributes are set.")]
        public void SqlLocalDbConfig_Uses_User_Values_If_Defined_And_Attributes_Specified()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(true, SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(1036, SqlLocalDbConfig.LanguageId, "SqlLocalDbConfig.LanguageId is incorrect.");
                    Assert.AreEqual("11.0", SqlLocalDbConfig.NativeApiOverrideVersionString, "SqlLocalDbConfig.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.KillProcess | StopInstanceOptions.NoWait, SqlLocalDbConfig.StopOptions, "SqlLocalDbConfig.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromSeconds(30), SqlLocalDbConfig.StopTimeout, "SqlLocalDbConfig.StopTimeout is incorrect.");
                },
                configurationFile: "SqlLocalDbConfigTests.DefinedAndSpecified.config");
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbConfig uses the correct default values if the configuration section is defined and the attributes are set but the legacy appSettings override settings are present.")]
        public void SqlLocalDbConfig_Uses_User_Values_If_Defined_And_Attributes_Specified_With_Legacy_AppSettings_Present()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(false, SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles, "SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(1036, SqlLocalDbConfig.LanguageId, "SqlLocalDbConfig.LanguageId is incorrect.");
                    Assert.AreEqual("12.0", SqlLocalDbConfig.NativeApiOverrideVersionString, "SqlLocalDbConfig.NativeApiOverrideVersion is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.KillProcess | StopInstanceOptions.NoWait, SqlLocalDbConfig.StopOptions, "SqlLocalDbConfig.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromSeconds(30), SqlLocalDbConfig.StopTimeout, "SqlLocalDbConfig.StopTimeout is incorrect.");
                },
                configurationFile: "SqlLocalDbConfigTests.DefinedAndSpecifiedWithLegacySettings.config");
        }
    }
}
