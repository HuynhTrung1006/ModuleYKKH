using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using YKKH_Module.CustomModel;

namespace YKKH_Module
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            Session["User"] = null;
            Session["DATTKH"] = new List<ListDATTKH>();
            Session["DAGYKH"] = new List<ListDAGY>();
            Session["KhachHang"] = null;
            Session["MaCDKH"] = null;
        }
    }
}
