// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Logger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class that performs logging for the <c>System.Data.SqlLocalDb</c> assembly.
    /// </summary>
    [DebuggerStepThrough]
    [Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Logger
    {
        /// <summary>
        /// The default <see cref="ILogger"/> to use. This field is read-only.
        /// </summary>
        internal static readonly ILogger DefaultLogger = CreateDefaultLogger();

        /// <summary>
        /// The Trace condition string.
        /// </summary>
        private const string TraceCondition = "TRACE";

        /// <summary>
        /// The <see cref="ILogger"/> to use.
        /// </summary>
        private static ILogger _logger = DefaultLogger;

        /// <summary>
        /// Sets the <see cref="ILogger"/> implementation in use by the assembly.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use, or <see langword="null"/> to use the default implementation.</param>
        [Conditional(TraceCondition)]
        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? DefaultLogger;
        }

        /// <summary>
        /// Writes an error trace event to the trace listeners for the assembly's trace source.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        [Conditional(TraceCondition)]
        public static void Error(int id, string format, params object[] args)
        {
            _logger.WriteError(id, format, args);
        }

        /// <summary>
        /// Writes an informational trace event to the trace listeners for the assembly's trace source.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        [Conditional(TraceCondition)]
        public static void Information(int id, string format, params object[] args)
        {
            _logger.WriteInformation(id, format, args);
        }

        /// <summary>
        /// Writes a verbose trace event to the trace listeners for the assembly's trace source.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        [Conditional(TraceCondition)]
        public static void Verbose(int id, string format, params object[] args)
        {
            _logger.WriteVerbose(id, format, args);
        }

        /// <summary>
        /// Writes a warning trace event to the trace listeners for the assembly's trace source.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        [Conditional(TraceCondition)]
        public static void Warning(int id, string format, params object[] args)
        {
            _logger.WriteWarning(id, format, args);
        }

        /// <summary>
        /// Creates the default <see cref="ILogger"/> implementation to use.
        /// </summary>
        /// <returns>
        /// The default implementation of <see cref="ILogger"/>.
        /// </returns>
        private static ILogger CreateDefaultLogger()
        {
            Type loggerType = SqlLocalDbConfig.LoggerType;

            ILogger logger;

            if (loggerType == null)
            {
                logger = TraceSourceLogger.Instance;
            }
            else
            {
                try
                {
                    // This cast is safe as the configuration section validates that the type implements ILogger
                    logger = (ILogger)Activator.CreateInstance(loggerType, nonPublic: true);
                }
                catch (Reflection.TargetInvocationException ex)
                {
                    // Log directly to Trace if we cannot create the custom ILogger
                    Trace.TraceError(SR.Logger_FailedToCreateCustomLoggerFormat, loggerType.AssemblyQualifiedName, (ex.InnerException ?? ex).Message);
                    throw;
                }
                catch (Exception ex)
                {
                    // Log directly to Trace if we cannot create the custom ILogger
                    Trace.TraceError(SR.Logger_FailedToCreateCustomLoggerFormat, loggerType.AssemblyQualifiedName, ex.Message);
                    throw;
                }
            }

            return logger;
        }

        /// <summary>
        /// A class containing trace event Ids.  This class cannot be inherited.
        /// </summary>
        internal static class TraceEvent
        {
            /// <summary>
            /// General usage.
            /// </summary>
            internal static readonly int General = 0;

            /// <summary>
            /// Creating a SQL LocalDB instance.
            /// </summary>
            internal static readonly int CreateInstance = 1;

            /// <summary>
            /// Deleting a SQL LocalDB instance.
            /// </summary>
            internal static readonly int DeleteInstance = 2;

            /// <summary>
            /// Getting information about a SQL LocalDB instance.
            /// </summary>
            internal static readonly int GetInstanceInfo = 3;

            /// <summary>
            /// Getting instance names for SQL LocalDB.
            /// </summary>
            internal static readonly int GetInstanceNames = 4;

            /// <summary>
            /// Getting version information for SQL LocalDB.
            /// </summary>
            internal static readonly int GetVersionInfo = 5;

            /// <summary>
            /// Getting installed versions of SQL LocalDB.
            /// </summary>
            internal static readonly int GetVersions = 6;

            /// <summary>
            /// Sharing a SQL LocalDB instance.
            /// </summary>
            internal static readonly int ShareInstance = 7;

            /// <summary>
            /// Starting a SQL LocalDB instance.
            /// </summary>
            internal static readonly int StartInstance = 8;

            /// <summary>
            /// Starting tracing for a SQL LocalDB.
            /// </summary>
            internal static readonly int StartTracing = 9;

            /// <summary>
            /// Stopping a SQL LocalDB instance.
            /// </summary>
            internal static readonly int StopInstance = 10;

            /// <summary>
            /// Stopping tracing for SQL LocalDB.
            /// </summary>
            internal static readonly int StopTracing = 11;

            /// <summary>
            /// Stopping sharing of an instance of SQL LocalDB.
            /// </summary>
            internal static readonly int UnshareInstance = 12;

            /// <summary>
            /// The SQL LocalDB registry key could not be found or opened.
            /// </summary>
            internal static readonly int RegistryKeyNotFound = 13;

            /// <summary>
            /// An invalid registry key was processed.
            /// </summary>
            internal static readonly int InvalidRegistryKey = 14;

            /// <summary>
            /// An invalid registry key was processed.
            /// </summary>
            internal static readonly int NoNativeApiFound = 15;

            /// <summary>
            /// The native SQL LocalDB API DLL could not be found.
            /// </summary>
            internal static readonly int NativeApiPathNotFound = 16;

            /// <summary>
            /// The native SQL LocalDB API DLL failed to load.
            /// </summary>
            internal static readonly int NativeApiLoadFailed = 17;

            /// <summary>
            /// The native SQL LocalDB API DLL was not loaded.
            /// </summary>
            internal static readonly int NativeApiNotLoaded = 18;

            /// <summary>
            /// The native SQL LocalDB API function could not be found.
            /// </summary>
            internal static readonly int FunctionNotFound = 19;

            /// <summary>
            /// The native SQL LocalDB API was loaded.
            /// </summary>
            internal static readonly int NativeApiLoaded = 20;

            /// <summary>
            /// The version of the native SQL LocalDB API loaded was overridden by the user.
            /// </summary>
            internal static readonly int NativeApiVersionOverriddenByUser = 21;

            /// <summary>
            /// A user instance of SQL LocalDB could not be deleted as it is in use.
            /// </summary>
            internal static readonly int DeleteFailedAsInstanceInUse = 22;

            /// <summary>
            /// The files(s) for a user instance of SQL LocalDB are being deleted.
            /// </summary>
            internal static readonly int DeletingInstanceFiles = 23;

            /// <summary>
            /// The files(s) for a user instance of SQL LocalDB were deleted.
            /// </summary>
            internal static readonly int DeletedInstanceFiles = 24;

            /// <summary>
            /// The files(s) for a user instance of SQL LocalDB could not be deleted.
            /// </summary>
            internal static readonly int DeletingInstanceFilesFailed = 25;

            /// <summary>
            /// The value of the <see cref="SqlLocalDbApi.LanguageId"/> property is invalid.
            /// </summary>
            internal static readonly int InvalidLanguageId = 26;

            /// <summary>
            /// The version of the native SQL LocalDB API specified as the override by the user cannot be found.
            /// </summary>
            internal static readonly int NativeApiVersionOverrideNotFound = 27;

            /// <summary>
            /// A user instance of SQL LocalDB could not be stopped.
            /// </summary>
            internal static readonly int StopFailed = 28;

            /// <summary>
            /// A user instance of SQL LocalDB could not be deleted.
            /// </summary>
            internal static readonly int DeleteFailed = 29;
        }
    }
}
