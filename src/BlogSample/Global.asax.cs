using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BlogSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        [ExcludeFromCodeCoverage]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
