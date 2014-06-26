// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ILogger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// Defines a logger.
    /// </summary>
    public interface ILogger
    {
        #region Methods

        /// <summary>
        /// Writes an error trace event.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        void WriteError(int id, string format, params object[] args);

        /// <summary>
        /// Writes an informational trace event.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        void WriteInformation(int id, string format, params object[] args);

        /// <summary>
        /// Writes a verbose trace event.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        void WriteVerbose(int id, string format, params object[] args);

        /// <summary>
        /// Writes a warning trace event.
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more
        /// format items, which correspond to objects in the args array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        void WriteWarning(int id, string format, params object[] args);

        #endregion
    }
}