// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SRHelperTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SRHelperTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        /// <summary>
        /// Initializes a new instance of the <see cref="SRHelperTests"/> class.
        /// </summary>
        public SRHelperTests()
        {
        }

        [TestMethod]
        [Description("Tests Format() if format is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SRHelper_Format_Throws_If_Format_Is_Null()
        {
            // Arrange
            string format = null;
            object[] args = new object[0];

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SRHelper.Format(format, args),
                "format");
        }

        [TestMethod]
        [Description("Tests Format() if args is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SRHelper_Format_Throws_If_Args_Is_Null()
        {
            // Arrange
            string format = string.Empty;
            object[] args = null;

            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SRHelper.Format(format, args),
                "args");
        }

        [TestMethod]
        [Description("Tests Format().")]
        public async Task SRHelper_Format_Formats_Parameters()
        {
            // Arrange
            await Task.Factory.StartNew(
                () =>
                {
                    // Use a non-default culture
                    var culture = CultureInfo.GetCultureInfo("en-GB");

                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;

                    // Use a date where the result is valid with the day and month either way around
                    // i.e. US format dates vs. UK format dates
                    DateTime value = new DateTime(2012, 2, 3, 12, 34, 56);

                    // Act
                    string result = SRHelper.Format(
                        "{0}",
                        value);

                    // Assert
                    Assert.AreEqual(
                        value.ToString(culture),
                        result,
                        "Format() returned incorrect result.");
                });
        }
    }
}