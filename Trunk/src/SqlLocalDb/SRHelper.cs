// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SRHelper.cs" company="http://sqllocaldb.codeplex.com">
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
//   SRHelper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A static class containing helper methods for use with the <see cref="SR"/> class.
    /// </summary>
    internal static class SRHelper
    {
        #region Methods

        /// <summary>
        /// Replaces the format item in a specified string with the string representation
        /// of a corresponding object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// A copy of format in which the format items have been replaced by the string
        /// representation of the corresponding objects in args.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is null.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid or the index of a format item is less than zero,
        /// or greater than or equal to the length of the <paramref name="args"/> array.
        /// </exception>
        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            return string.Format(
                SR.Culture,
                format,
                args);
        }

        #endregion
    }
}