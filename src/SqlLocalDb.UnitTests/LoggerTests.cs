// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   LoggerTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerTests"/> class.
        /// </summary>
        public LoggerTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests SetLogger() sets the logging implementation in use and setting it to null resets the logging implementation.")]
        public void SetLogger_Sets_Logger_And_Allows_Logger_To_Be_Reset()
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

        #endregion
    }
}