using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SistemRecrutare
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string con = ConfigurationManager.ConnectionStrings["sqlConStringDependencies"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //setare sqlDependencies Start si Stop pentru SignalR
            SqlDependency.Start(con);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Notificari n = new Notificari();
            var data_curenta = DateTime.Now;
            HttpContext.Current.Application["LastUpdated"] = data_curenta;
            n.InregistrareNotificari(data_curenta/*HttpContext.Current.Application["Nume"].ToString()*/);
        }

        protected void Application_End()
        {
            SqlDependency.Stop(con);
        }
    }
}
