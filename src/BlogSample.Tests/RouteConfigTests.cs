using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample
{
    [TestClass]
    public class RouteConfigTests
    {
        [TestMethod]
        public void RouteConfig_RegisterRoutes_Adds_Routes()
        {
            // Act
            RouteCollection routes = new RouteCollection();

            // Act
            RouteConfig.RegisterRoutes(routes);

            // Assert
            Assert.AreNotEqual(0, routes.Count);
        }
    }
}
