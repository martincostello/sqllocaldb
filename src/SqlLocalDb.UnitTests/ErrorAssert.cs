// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorAssert.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ErrorAssert.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing assert extension methods.  This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Based on the Assert.Throws&lt;T&gt;() method in the xUnit.net Test Framework.
    /// Designed to work around CA1804 and CA1806 being raised by <c>FxCop</c> for unused objects
    /// constructed in the process of testing constructors and properties.
    /// </remarks>
    internal static class ErrorAssert
    {
        #region Methods

        /// <summary>
        /// Verifies that the specified delegate throws an exception of the specified
        /// type derived from <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ArgumentException"/> expected to be thrown.</typeparam>
        /// <param name="testCode">An <see cref="Action"/> representing the code that should throw the exception.</param>
        /// <param name="paramName">The expected name of the exception parameter.</param>
        /// <returns>
        /// The exception thrown by invoking <paramref name="testCode"/>.
        /// </returns>
        public static T Throws<T>(Action testCode, string paramName)
            where T : ArgumentException
        {
            T exception = Throws<T>(testCode);

            Assert.AreEqual(
                paramName,
                exception.ParamName,
                "The value of the {0}.ParamName property is incorrect.",
                typeof(T).FullName);

            return exception;
        }

        /// <summary>
        /// Verifies that the specified delegate throws an exception of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Exception"/> expected to be thrown.</typeparam>
        /// <param name="testCode">An <see cref="Action"/> representing the code that should throw the exception.</param>
        /// <returns>
        /// The exception thrown by invoking <paramref name="testCode"/>.
        /// </returns>
        public static T Throws<T>(Action testCode)
            where T : Exception
        {
            T exception = Invoke<T>(testCode);

            if (exception == null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "An exception of type {0} was not thrown.",
                    typeof(T).FullName);

                Assert.Fail(message);
            }

            Type thrownType = exception.GetType();
            Type expectedType = typeof(T);

            if (thrownType != expectedType)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "An exception of type {0} was thrown, but an exception of type {1} was expected.",
                    thrownType.FullName,
                    expectedType.FullName);

                Assert.Fail(message);
            }

            return exception;
        }

        /// <summary>
        /// Verifies that the specified delegate throws an exception of the specified
        /// type derived from <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ArgumentException"/> expected to be thrown.</typeparam>
        /// <param name="testCode">An <see cref="Func&lt;Object&gt;"/> representing the code that should throw the exception.</param>
        /// <param name="paramName">The expected name of the exception parameter.</param>
        /// <returns>
        /// The exception thrown by invoking <paramref name="testCode"/>.
        /// </returns>
        public static T Throws<T>(Func<object> testCode, string paramName)
            where T : ArgumentException
        {
            T exception = Throws<T>(testCode);

            Assert.AreEqual(
                paramName,
                exception.ParamName,
                "The value of the {0}.ParamName property is incorrect.",
                typeof(T).FullName);

            return exception;
        }

        /// <summary>
        /// Verifies that the specified delegate throws an exception of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Exception"/> expected to be thrown.</typeparam>
        /// <param name="testCode">An <see cref="Func&lt;Object&gt;"/> representing the code that should throw the exception.</param>
        /// <returns>
        /// The exception thrown by invoking <paramref name="testCode"/>.
        /// </returns>
        public static T Throws<T>(Func<object> testCode)
            where T : Exception
        {
            return Throws<T>(new Action(() => testCode()));
        }

        /// <summary>
        /// Invokes the specified <see cref="Action"/> delegate and
        /// returns any <see cref="Exception"/> thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
        /// <param name="testCode">An <see cref="Action"/> representing the code that should throw the exception.</param>
        /// <returns>
        /// The <typeparamref name="T"/> exception thrown by invoking <paramref name="testCode"/>,
        /// if any was thrown; otherwise <see langword="null"/>.
        /// </returns>
        private static T Invoke<T>(Action testCode)
            where T : Exception
        {
            Contract.Requires(testCode != null);

            try
            {
                testCode();
                return null;
            }
            catch (T e)
            {
                return e;
            }
        }

        #endregion
    }
}