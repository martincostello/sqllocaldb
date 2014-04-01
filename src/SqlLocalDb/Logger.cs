// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   LocalDBVersionInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class that performs logging for the <c>System.Data.SqlLocalDb</c> assembly.
    /// </summary>
    [DebuggerStepThrough]
#if NET40
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal static class Logger
    {
        #region Constants

        /// <summary>
        /// The name of the <c>System.Data.SqlLocalDb</c> <see cref="TraceListener"/>.
        /// </summary>
        private const string TraceSourceName = "System.Data.SqlLocalDb";

        /// <summary>
        /// The Trace condition string.
        /// </summary>
        private const string TraceCondition = "TRACE";

        #endregion

        #region Fields

        /// <summary>
        /// Whether the application domain has shutdown.
        /// </summary>
        private static bool _appDomainShutdown;

        /// <summary>
        /// Whether logging is enabled.
        /// </summary>
        private static bool _enabled;

        /// <summary>
        /// Whether logging has been initialized.
        /// </summary>
        private static bool _initialized;

        /// <summary>
        /// The synchronization object for the logging class.
        /// </summary>
        private static object _syncRoot;

        /// <summary>
        /// The trace source for <c>System.Data.SqlLocalDb</c>.
        /// </summary>
        private static TraceSource _traceSource;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether logging is enabled.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                }

                return _enabled;
            }
        }

        /// <summary>
        /// Gets the trace source used for the <c>System.Data.SqlLocalDb</c> assembly
        /// if logging is enabled; otherwise <see langword="null"/>.
        /// </summary>
        public static TraceSource Source
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                }

                if (!_enabled)
                {
                    return null;
                }

                return _traceSource;
            }
        }

        /// <summary>
        /// Gets the synchronization object used by the class.
        /// </summary>
        private static object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    object syncRoot = new object();
                    Interlocked.CompareExchange(ref _syncRoot, syncRoot, null);
                }

                return _syncRoot;
            }
        }

        #endregion

        #region Methods

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
            if (ValidateSettings(Source, TraceEventType.Error))
            {
                Source.TraceEvent(TraceEventType.Error, id, format, args);
            }
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
            if (ValidateSettings(Source, TraceEventType.Information))
            {
                Source.TraceEvent(TraceEventType.Information, id, format, args);
            }
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
            if (ValidateSettings(Source, TraceEventType.Verbose))
            {
                Source.TraceEvent(TraceEventType.Verbose, id, format, args);
            }
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
            if (ValidateSettings(Source, TraceEventType.Warning))
            {
                Source.TraceEvent(TraceEventType.Warning, id, format, args);
            }
        }

        /// <summary>
        /// Closes the trace sources.
        /// </summary>
        [SecurityCritical]
        private static void Close()
        {
            if (_traceSource != null)
            {
                _traceSource.Close();
            }
        }

        /// <summary>
        /// Event handler for when the current application domain unloads or the current process exits.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [SecurityCritical]
        private static void DomainUnloadOrProcessExit(object sender, EventArgs e)
        {
            Close();
            _appDomainShutdown = true;

            // Remove the event handlers to prevent memory leaks
            AppDomain current = AppDomain.CurrentDomain;
            current.DomainUnload -= DomainUnloadOrProcessExit;
            current.ProcessExit -= DomainUnloadOrProcessExit;
        }

        /// <summary>
        /// Initializes the <see cref="Logger"/> class.
        /// </summary>
        [SecurityCritical]
        private static void Initialize()
        {
            lock (SyncRoot)
            {
                if (!_initialized)
                {
                    _traceSource = new TraceSource(TraceSourceName);

                    bool loggingEnabled;

                    try
                    {
                        loggingEnabled = _traceSource.Switch.ShouldTrace(TraceEventType.Critical);
                    }
                    catch (SecurityException)
                    {
                        Close();
                        loggingEnabled = false;
                    }

                    if (loggingEnabled)
                    {
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.DomainUnload += DomainUnloadOrProcessExit;
                        currentDomain.ProcessExit += DomainUnloadOrProcessExit;
                    }

                    _enabled = loggingEnabled;
                    _initialized = true;
                }
            }
        }

        /// <summary>
        /// Validates the settings.
        /// </summary>
        /// <param name="traceSource">The trace source to write to.</param>
        /// <param name="traceLevel">The trace level of the message.</param>
        /// <returns>
        /// <see langword="true"/> if the settings are valid;
        /// otherwise <see langword="false"/>.
        /// </returns>
        private static bool ValidateSettings(TraceSource traceSource, TraceEventType traceLevel)
        {
            if (!_enabled)
            {
                return false;
            }

            if (!_initialized)
            {
                Initialize();
            }

            if ((traceSource == null) || !traceSource.Switch.ShouldTrace(traceLevel))
            {
                return false;
            }

            if (_appDomainShutdown)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Classes

        /// <summary>
        /// A class containing trace event Ids.  This class cannot be inherited.
        /// </summary>
        internal static class TraceEvent
        {
            #region Fields

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

            #endregion
        }

        #endregion
    }
}