using System.Web.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample
{
    [TestClass]
    public class BundleConfigTests
    {
        [TestMethod]
        public void BundleConfig_RegisterBundles_Adds_Bundles()
        {
            // Arrange
            BundleCollection bundles = new BundleCollection();

            // Act
            BundleConfig.RegisterBundles(bundles);

            // Assert
            Assert.AreNotEqual(0, bundles.Count);
        }
    }
}
