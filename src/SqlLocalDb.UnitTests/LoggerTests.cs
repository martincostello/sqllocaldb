// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   LoggerTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="Logger"/> class.
    /// </summary>
    [TestClass]
    public class LoggerTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerTests"/> class.
        /// </summary>
        public LoggerTests()
        {
        }

        [TestMethod]
        [Description("Tests SetLogger() sets the logging implementation in use and setting it to null resets the logging implementation.")]
        public void Logger_SetLogger_Sets_Logger_And_Allows_Logger_To_Be_Reset()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    Mock<ILogger> mock = new Mock<ILogger>();

                    ILogger logger = mock.Object;

                    int id = 42;
                    string format = "The machine name is {0}.";
                    object[] args = new object[] { Environment.MachineName };

                    // Act
                    Logger.SetLogger(logger);

                    Logger.Error(id, format, args);
                    Logger.Information(id, format, args);
                    Logger.Verbose(id, format, args);
                    Logger.Warning(id, format, args);

                    // Assert
                    mock.Verify((p) => p.WriteError(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteInformation(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteVerbose(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteWarning(id, format, args), Times.Once());

                    // Act
                    Logger.SetLogger(null);

                    Logger.Error(id, format, args);
                    Logger.Information(id, format, args);
                    Logger.Verbose(id, format, args);
                    Logger.Warning(id, format, args);

                    // Assert
                    mock.Verify((p) => p.WriteError(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteInformation(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteVerbose(id, format, args), Times.Once());
                    mock.Verify((p) => p.WriteWarning(id, format, args), Times.Once());
                });
        }

        [TestMethod]
        [Description("Tests that the Logger class uses the custom ILogger implementation configured in the application configuration file.")]
        public void Logger_SetLogger_Uses_Custom_ILogger_Implementation_From_Configuration_File()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    int id = 42;
                    string format = "The machine name is {0}.";
                    object[] args = new object[] { Environment.MachineName };

                    Logger.SetLogger(null); // Clear the special logger the cross-AppDomain logging for the tests sets up

                    // Act
                    Logger.Error(id, format, args);
                    Logger.Information(id, format, args);
                    Logger.Verbose(id, format, args);
                    Logger.Warning(id, format, args);

                    // Assert
                    Assert.AreEqual(4, TestLogger.InvocationCount, "The custom logger was not used.");
                },
                configurationFile: "LoggerTests.CustomLoggerType.config");
        }

        [TestMethod]
        [Description("Tests that all event ID values in the Logger.TraceEvent class are unique.")]
        public void Logger_TraceEvent_All_Id_Values_Are_Unique()
        {
            // Arrange
            Type type = typeof(Logger.TraceEvent);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            IList<int> values = new List<int>();

            // Act
            foreach (FieldInfo field in fields)
            {
                int value = (int)field.GetValue(null);
                values.Add(value);
            }

            // Assert
            Assert.AreNotEqual(0, values.Count, "No values were obtained for the {0} class.", type.FullName);
            Assert.AreEqual(values.Distinct().Count(), values.Count, "The {0} class contains one or more duplicate event ID.", type.FullName);
        }

        /// <summary>
        /// A class for testing changing the default <see cref="ILogger"/> implementation. This class cannot be inherited.
        /// </summary>
        private sealed class TestLogger : ILogger
        {
            /// <summary>
            /// The number of times any logger has been invoked.
            /// </summary>
            private static int _invocationCount;

            /// <summary>
            /// Prevents a default instance of the <see cref="TestLogger"/> class from being created.
            /// </summary>
            private TestLogger()
            {
                // Private constructor and class used to test that non-public loggers can be used
            }

            /// <summary>
            /// Gets the number of times any logger has been invoked.
            /// </summary>
            internal static int InvocationCount
            {
                get { return _invocationCount; }
            }

            /// <inheritdoc />
            public void WriteError(int id, string format, params object[] args)
            {
                _invocationCount++;
            }

            /// <inheritdoc />
            public void WriteInformation(int id, string format, params object[] args)
            {
                _invocationCount++;
            }

            /// <inheritdoc />
            public void WriteVerbose(int id, string format, params object[] args)
            {
                _invocationCount++;
            }

            /// <inheritdoc />
            public void WriteWarning(int id, string format, params object[] args)
            {
                _invocationCount++;
            }
        }
    }
}