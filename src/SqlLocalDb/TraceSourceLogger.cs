// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceSourceLogger.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TraceSourceLogger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an implementation of <see cref="ILogger"/> that logs to a <see cref="TraceSource"/>.
    /// </summary>
    internal sealed class TraceSourceLogger : ILogger
    {
        #region Constants and Fields

        /// <summary>
        /// The singleton instance of <see cref="TraceSourceLogger"/>.  This field is read-only.
        /// </summary>
        internal static readonly TraceSourceLogger Instance = new TraceSourceLogger();

        /// <summary>
        /// The name of the <c>System.Data.SqlLocalDb</c> <see cref="TraceListener"/>.
        /// </summary>
        private const string TraceSourceName = "System.Data.SqlLocalDb";

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

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="TraceSourceLogger"/> class from being created.
        /// </summary>
        private TraceSourceLogger()
        {
        }

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
        public void WriteError(int id, string format, params object[] args)
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
        public void WriteInformation(int id, string format, params object[] args)
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
        public void WriteVerbose(int id, string format, params object[] args)
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
        public void WriteWarning(int id, string format, params object[] args)
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
    }
}