// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHelper.cs" company="http://sqllocaldb.codeplex.com">
//   New BSD License (BSD)
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the New BSD Licence (BSD). 
//   A copy of the license can be found in the License.txt file at the root of this
//   distribution.  By using this source code in any fashion, you are agreeing to be
//   bound by the terms of the New BSD Licence. You must not remove this notice, or
//   any other, from this software.
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