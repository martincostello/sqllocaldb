// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SRHelper.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
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