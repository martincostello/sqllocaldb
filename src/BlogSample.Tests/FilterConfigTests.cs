using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample
{
    [TestClass]
    public class FilterConfigTests
    {
        [TestMethod]
        public void FilterConfig_RegisterGlobalFilters_Adds_Filters()
        {
            // Arrange
            GlobalFilterCollection filters = new GlobalFilterCollection();

            // Act
            FilterConfig.RegisterGlobalFilters(filters);

            // Assert
            Assert.AreNotEqual(0, filters.Count);
        }
    }
}
