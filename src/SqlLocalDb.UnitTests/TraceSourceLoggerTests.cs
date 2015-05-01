// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceSourceLoggerTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TraceSourceLoggerTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing tests for the <see cref="TraceSourceLogger"/> class.
    /// </summary>
    [TestClass]
    public class TraceSourceLoggerTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSourceLoggerTests"/> class.
        /// </summary>
        public TraceSourceLoggerTests()
        {
        }

        [TestMethod]
        [Description("Tests TraceSourceLogger does not log if some logging levels are not enabled.")]
        public void TraceSourceLogger_Verbose_Does_Not_Log_If_Some_Logging_Levels_Are_Not_Enabled()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    TraceSourceLogger target = TraceSourceLogger.Instance;

                    // Act
                    target.WriteError(1, "Letter 1 is {0}.", 'a');
                    target.WriteWarning(2, "Letter 2 is {0}.", 'b');
                    target.WriteInformation(3, "Letter 3 is {0}.", 'c');
                    target.WriteVerbose(4, "Letter 4 is {0}.", 'd');

                    // Assert
                    var data = TestTraceListener.LogData;

                    Assert.AreEqual(2, data.Count);

                    int count = data
                        .Where((p) => p.Item1 == TraceEventType.Error)
                        .Where((p) => p.Item2 == 1)
                        .Where((p) => p.Item3 == "Letter 1 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'a')
                        .Count();

                    Assert.AreEqual(1, count, "The error message was not logged correctly.");

                    count = data
                        .Where((p) => p.Item1 == TraceEventType.Warning)
                        .Where((p) => p.Item2 == 2)
                        .Where((p) => p.Item3 == "Letter 2 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'b')
                        .Count();

                    Assert.AreEqual(1, count, "The warning message was not logged correctly.");
                },
                configurationFile: "TraceSourceLoggerTests.SomeDisabled.config");
        }

        [TestMethod]
        [Description("Tests TraceSourceLogger does not log if all logging levels are not enabled.")]
        public void TraceSourceLogger_Verbose_Does_Not_Log_If_All_Logging_Levels_Are_Not_Enabled()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    TraceSourceLogger target = TraceSourceLogger.Instance;

                    // Act
                    target.WriteError(1, "Letter 1 is {0}.", 'a');
                    target.WriteWarning(2, "Letter 2 is {0}.", 'b');
                    target.WriteInformation(3, "Letter 3 is {0}.", 'c');
                    target.WriteVerbose(4, "Letter 4 is {0}.", 'd');

                    // Assert
                    var data = TestTraceListener.LogData;

                    Assert.AreEqual(4, data.Count);

                    int count = data
                        .Where((p) => p.Item1 == TraceEventType.Error)
                        .Where((p) => p.Item2 == 1)
                        .Where((p) => p.Item3 == "Letter 1 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'a')
                        .Count();

                    Assert.AreEqual(1, count, "The error message was not logged correctly.");

                    count = data
                        .Where((p) => p.Item1 == TraceEventType.Warning)
                        .Where((p) => p.Item2 == 2)
                        .Where((p) => p.Item3 == "Letter 2 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'b')
                        .Count();

                    Assert.AreEqual(1, count, "The warning message was not logged correctly.");

                    count = data
                        .Where((p) => p.Item1 == TraceEventType.Information)
                        .Where((p) => p.Item2 == 3)
                        .Where((p) => p.Item3 == "Letter 3 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'c')
                        .Count();

                    Assert.AreEqual(1, count, "The information message was not logged correctly.");

                    count = data
                        .Where((p) => p.Item1 == TraceEventType.Verbose)
                        .Where((p) => p.Item2 == 4)
                        .Where((p) => p.Item3 == "Letter 4 is {0}.")
                        .Where((p) => p.Item4 != null)
                        .Where((p) => p.Item4.Length == 1)
                        .Where((p) => (char)p.Item4[0] == 'd')
                        .Count();

                    Assert.AreEqual(1, count, "The verbose message was not logged correctly.");
                },
                configurationFile: "TraceSourceLoggerTests.AllEnabled.config");
        }

        /// <summary>
        /// A class representing an implementation of <see cref="TraceListener"/> to test logging behavior. This class cannot be inherited.
        /// </summary>
        internal sealed class TestTraceListener : TraceListener
        {
            /// <summary>
            /// The data logged to the trace listener. This field is read-only.
            /// </summary>
            internal static readonly List<Tuple<TraceEventType, int, string, object[]>> LogData = new List<Tuple<TraceEventType, int, string, object[]>>();

            /// <inheritdoc />
            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, object[] args)
            {
                LogData.Add(Tuple.Create(eventType, id, format, args));
            }

            /// <inheritdoc />
            public override void Write(string message)
            {
            }

            /// <inheritdoc />
            public override void WriteLine(string message)
            {
            }
        }
    }
}