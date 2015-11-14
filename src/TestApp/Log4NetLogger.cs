// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogger.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Log4NetLogger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using log4net;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an implementation of <see cref=""/> that uses <c>log4net</c>. This class cannot be inherited.
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Instantiated via reflection.")]
    internal sealed class Log4NetLogger : ILogger
    {
        /// <summary>
        /// The <c>log4net</c> logger being wrapped by the interface. This field is read-only.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger("System.Data.SqlLocalDb");

        /// <inheritdoc/>
        public void WriteError(int id, string format, params object[] args) => Logger.ErrorFormat(CultureInfo.InvariantCulture, format, args);

        /// <inheritdoc/>
        public void WriteInformation(int id, string format, params object[] args) => Logger.InfoFormat(CultureInfo.InvariantCulture, format, args);

        /// <inheritdoc/>
        public void WriteVerbose(int id, string format, params object[] args) => Logger.DebugFormat(CultureInfo.InvariantCulture, format, args);

        /// <inheritdoc/>
        public void WriteWarning(int id, string format, params object[] args) => Logger.WarnFormat(CultureInfo.InvariantCulture, format, args);
    }
}
