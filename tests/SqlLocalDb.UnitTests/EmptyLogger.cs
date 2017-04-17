// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyLogger.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   EmptyLogger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an empty implementation of <see cref="ILogger"/> for use in tests. This class cannot be inherited.
    /// </summary>
    internal sealed class EmptyLogger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyLogger"/> class.
        /// </summary>
        internal EmptyLogger()
        {
        }

        /// <inheritdoc />
        public void WriteError(int id, string format, params object[] args)
        {
        }

        /// <inheritdoc />
        public void WriteInformation(int id, string format, params object[] args)
        {
        }

        /// <inheritdoc />
        public void WriteVerbose(int id, string format, params object[] args)
        {
        }

        /// <inheritdoc />
        public void WriteWarning(int id, string format, params object[] args)
        {
        }
    }
}
