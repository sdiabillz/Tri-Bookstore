using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BookStore.DAL;
using System.Security.Principal;

namespace BookStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new BookStoreInitializer());  
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public MvcApplication()
        {
            AuthorizeRequest += new EventHandler(MvcApplication_AuthorizeRequest);
        }
        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {

            IIdentity id = Context.User.Identity;
            if (id.IsAuthenticated)
            {
                string role = new BookstoreContext().GetSecurityLevel(id.Name);
                Context.User = new GenericPrincipal(id, new string[] { role });
            }
        }
    }
}
