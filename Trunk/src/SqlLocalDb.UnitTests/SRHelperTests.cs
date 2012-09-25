// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SRHelperTests.cs" company="http://sqllocaldb.codeplex.com">
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
//   SRHelperTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SRHelper"/> class.
    /// </summary>
    [TestClass]
    public class SRHelperTests
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SRHelperTests"/> class.
        /// </summary>
        public SRHelperTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests Format() if format is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Format_ThrowsIfFormatIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SRHelper.Format(null, new object[0]),
                "format");
        }

        [TestMethod]
        [Description("Tests Format() if args is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Format_ThrowsIfArgsIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SRHelper.Format(string.Empty, null),
                "args");
        }

        [TestMethod]
        [Description("Tests Format().")]
        public void Format()
        {
            Task.Factory.StartNew(
                () =>
                {
                    // Use a non-default culture
                    var culture = CultureInfo.GetCultureInfo("en-GB");
                    Thread.CurrentThread.CurrentUICulture = culture;

                    // Use a date where the result is valid with the day and month either way around
                    // i.e. US format dates vs. UK format dates
                    DateTime value = new DateTime(2012, 2, 3, 12, 34, 56);

                    string result = SRHelper.Format(
                        "{0}",
                        value);

                    Assert.AreEqual(
                        value.ToString(culture),
                        result,
                        "Format() returned incorrect result.");
                }).Wait();
        }

        #endregion
    }
}