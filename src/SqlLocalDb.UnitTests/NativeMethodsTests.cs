// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethodsTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   NativeMethodsTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="NativeMethods"/> class.
    /// </summary>
    [TestClass]
    public class NativeMethodsTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMethodsTests"/> class.
        /// </summary>
        public NativeMethodsTests()
        {
        }

        [TestMethod]
        [Description("Tests that all P/Invoke methods to the SQL LocalDB Instance API return the correct value if it is not installed.")]
        public void NativeMethods_Methods_Return_Correct_Value_If_Sql_LocalDb_Instance_Api_Not_Installed()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    NativeMethods.Registry = Mock.Of<NativeMethods.IRegistry>();

                    int intValue = default(int);
                    IntPtr ptrValue = default(IntPtr);
                    StopInstanceOptions stopValue = default(StopInstanceOptions);
                    string stringValue = default(string);
                    StringBuilder builderValue = default(StringBuilder);

                    // Act and Assert
                    AssertIsNotInstalledResult(() => NativeMethods.CreateInstance(stringValue, stringValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.DeleteInstance(stringValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.GetInstanceInfo(stringValue, ptrValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.GetInstanceNames(ptrValue, ref intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.GetLocalDbError(intValue, intValue, builderValue, ref intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.GetVersionInfo(stringValue, ptrValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.GetVersions(ptrValue, ref intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.ShareInstance(ptrValue, stringValue, stringValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.StartInstance(stringValue, intValue, builderValue, ref intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.StartTracing());
                    AssertIsNotInstalledResult(() => NativeMethods.StopInstance(stringValue, stopValue, intValue));
                    AssertIsNotInstalledResult(() => NativeMethods.StopTracing());
                    AssertIsNotInstalledResult(() => NativeMethods.UnshareInstance(stringValue, intValue));
                });
        }

        [Ignore]    // Test causes a UI pop-up from Windows when building from the command-line
        [TestMethod]
        [Description("Tests that P/Invoke methods to the SQL LocalDB Instance API return the correct value if the DLL cannot be loaded.")]
        public void NativeMethods_Methods_Return_Correct_Value_If_Sql_LocalDb_Instance_Api_Cannot_Be_Loaded()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string fileName = Path.GetTempFileName();   // Empty file so not a valid DLL

                    try
                    {
                        var versions = new Tuple<string, string>[]
                        {
                            Tuple.Create("12.0", fileName),
                        };

                        NativeMethods.Registry = CreateRegistry(versions);

                        // Act and Assert
                        AssertIsNotInstalledResult(() => NativeMethods.StartTracing());
                    }
                    finally
                    {
                        File.Delete(fileName);
                    }
                });
        }

        [TestMethod]
        [Description("Tests that P/Invoke methods to the SQL LocalDB Instance API return the correct value if the native function in the DLL cannot be found.")]
        public void NativeMethods_Methods_Return_Correct_Value_If_Sql_LocalDb_Function_Not_Found()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string fileName = typeof(NativeMethods).Assembly.Location;  // .NET assembly not a native DLL, so GetProcAddress() will fail

                    var versions = new Tuple<string, string>[]
                    {
                        Tuple.Create("12.0", fileName),
                    };

                    NativeMethods.Registry = CreateRegistry(versions);

                    // Act and Assert
                    AssertIsNotInstalledResult(() => NativeMethods.StartTracing());
                });
        }

        [TestMethod]
        [Description("Tests that P/Invoke methods to the SQL LocalDB Instance API return the correct value if the native DLL cannot be found.")]
        public void NativeMethods_Methods_Return_Correct_Value_If_Sql_LocalDb_Api_Path_Not_Found()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    var versions = new Tuple<string, string>[]
                    {
                        Tuple.Create("12.0", string.Empty),
                    };

                    NativeMethods.Registry = CreateRegistry(versions);

                    // Act and Assert
                    AssertIsNotInstalledResult(() => NativeMethods.StartTracing());
                });
        }

        [TestMethod]
        [Description("Tests TryGetLocalDbApiPath() correctly enumerates the registry to get the path of the latest installed version of SQL LocalDB ignoring any invalid registry keys.")]
        public void NativeMethods_TryGetLocalDbApiPath_Enumerates_Installed_Sql_LocalDb_Versions_Correctly_Ignoring_Invalid_Registry_Keys()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string expected = typeof(NativeMethods).Assembly.Location;

                    var versions = new Tuple<string, string>[]
                    {
                        Tuple.Create("11.0", "SqlLocalDb_11_0.dll"),
                        Tuple.Create("11.1", "SqlLocalDb_11_1.dll"),
                        Tuple.Create("12.0", string.Empty),
                        Tuple.Create("12.0.1", typeof(NativeMethodsTests).Assembly.Location),
                        Tuple.Create("12.0.1.2", Path.GetFileName(expected)),
                        Tuple.Create("NotAVersion", string.Empty),
                        Tuple.Create("12.0.0.a", string.Empty),
                        Tuple.Create("13.0.0.0.0", string.Empty),
                        Tuple.Create("2147483648.0.0.0", string.Empty),
                    };

                    NativeMethods.Registry = CreateRegistry(versions);

                    // Act
                    string fileName;
                    bool result = NativeMethods.TryGetLocalDbApiPath(out fileName);

                    // Assert
                    Assert.IsTrue(result, "TryGetLocalDbApiPath() returned incorrect result.");
                    Assert.AreEqual(expected, fileName, "fileName is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests TryGetLocalDbApiPath() returns false if the value specified in the registry cannot be found.")]
        public void NativeMethods_TryGetLocalDbApiPath_Returns_False_If_File_Not_Found()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    var versions = new Tuple<string, string>[]
                    {
                        Tuple.Create("11.0", "NonExistentFile.dll"),
                    };

                    NativeMethods.Registry = CreateRegistry(versions);

                    // Act
                    string fileName;
                    bool result = NativeMethods.TryGetLocalDbApiPath(out fileName);

                    // Assert
                    Assert.IsFalse(result, "TryGetLocalDbApiPath() returned incorrect result.");
                    Assert.IsNull(fileName, "fileName is not null.");
                });
        }

        [TestMethod]
        [Description("Tests IRegistry.OpenSubKey() if the sub-key is not found.")]
        public void NativeMethods_Registry_OpenSubkey_If_Subkey_Not_Found()
        {
            // Arrange
            string keyName = Guid.NewGuid().ToString();

            // Act
            var result = NativeMethods.Registry.OpenSubKey(keyName);

            try
            {
                // Assert
                Assert.IsNull(result, "OpenSubKey() did not return null.");
            }
            finally
            {
                if (result != null)
                {
                    result.Dispose();
                }
            }
        }

        [TestMethod]
        [Description("Tests IRegistryKey.OpenSubKey() if the sub-key is not found.")]
        public void NativeMethods_RegistryKey_OpenSubkey_If_Subkey_Not_Found()
        {
            // Arrange
            using (var key = NativeMethods.Registry.OpenSubKey(@"SOFTWARE\Microsoft"))
            {
                string keyName = Guid.NewGuid().ToString();

                // Act
                var result = key.OpenSubKey(keyName);

                try
                {
                    // Assert
                    Assert.IsNull(result, "OpenSubKey() did not return null.");
                }
                finally
                {
                    if (result != null)
                    {
                        result.Dispose();
                    }
                }
            }
        }

        [TestMethod]
        [Description("Tests IRegistryKey.GetValue() if the value is not found.")]
        public void NativeMethods_RegistryKey_GetValue_If_Value_Not_Found()
        {
            // Arrange
            using (var key = NativeMethods.Registry.OpenSubKey(@"SOFTWARE\Microsoft"))
            {
                string name = Guid.NewGuid().ToString();

                // Act
                string result = key.GetValue(name);
                
                // Assert
                Assert.IsNull(result, "GetValue() did not return null.");
            }
        }

        /// <summary>
        /// Asserts that the specified delegate returns <see cref="SqlLocalDbErrors.NotInstalled"/>.
        /// </summary>
        /// <param name="func">The delegate to invoke to assert on the return value of.</param>
        private static void AssertIsNotInstalledResult(Func<int> func)
        {
            int actual = func();
            Assert.AreEqual(SqlLocalDbErrors.NotInstalled, actual, "Method did not return {0:X}. Value: {1:X}", SqlLocalDbErrors.NotInstalled, actual);
        }

        /// <summary>
        /// Creates a <see cref="NativeMethods.IRegistry"/> mock set up to contain the
        /// specified SQL LocalDB Instance API versions and the paths to their DLLs.
        /// </summary>
        /// <param name="collection">An optional collection of version-path values to set up the registry for.</param>
        /// <returns>
        /// A mock implementation of <see cref="NativeMethods.IRegistry"/> set up as specified.
        /// </returns>
        private static NativeMethods.IRegistry CreateRegistry(IEnumerable<Tuple<string, string>> collection = null)
        {
            IList<Tuple<string, NativeMethods.IRegistryKey>> versionKeys = new List<Tuple<string, NativeMethods.IRegistryKey>>();

            if (collection != null)
            {
                foreach (var version in collection)
                {
                    Mock<NativeMethods.IRegistryKey> versionKey = new Mock<NativeMethods.IRegistryKey>();

                    versionKey
                        .Setup((p) => p.GetValue("InstanceAPIPath"))
                        .Returns(version.Item2);

                    versionKeys.Add(Tuple.Create(version.Item1, versionKey.Object));
                }
            }

            Mock<NativeMethods.IRegistryKey> sqlLocalDbKey = new Mock<NativeMethods.IRegistryKey>();

            sqlLocalDbKey.Setup((p) => p.GetSubKeyNames())
                .Returns(versionKeys.Select((p) => p.Item1).ToArray());

            foreach (var version in versionKeys)
            {
                sqlLocalDbKey
                    .Setup((p) => p.OpenSubKey(version.Item1))
                    .Returns(version.Item2);
            }

            Mock<NativeMethods.IRegistry> mockRegistry = new Mock<NativeMethods.IRegistry>();

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                mockRegistry
                    .Setup((p) => p.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SQL Server Local DB\Installed Versions"))
                    .Returns(sqlLocalDbKey.Object);
            }
            else
            {
                mockRegistry
                    .Setup((p) => p.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions"))
                    .Returns(sqlLocalDbKey.Object);
            }

            return mockRegistry.Object;
        }
    }
}