// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHelper.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ErrorHelper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing helper methods for error handling.  This class cannot be inherited.
    /// </summary>
    internal static class ErrorHelper
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified exception is fatal.
        /// </summary>
        /// <param name="exception">The exception to test.</param>
        /// <returns>
        /// <see langword="true"/> if the specified exception is fatal; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if (((exception is OutOfMemoryException) && !(exception is InsufficientMemoryException)) ||
                    (((exception is ThreadAbortException) ||
                      (exception is AccessViolationException)) ||
                     (exception is StackOverflowException) ||
                     (exception is SEHException)))
                {
                    return true;
                }

                if (!(exception is TypeInitializationException) && !(exception is TargetInvocationException))
                {
                    break;
                }

                exception = exception.InnerException;
            }

            return false;
        }

        #endregion
    }
}