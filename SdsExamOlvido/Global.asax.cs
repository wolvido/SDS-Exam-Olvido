using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SdsExamOlvido.DbContexts;
using Microsoft.Ajax.Utilities;
using SdsExamOlvido.ServiceContracts;
using SdsExamOlvido.Services;
using Unity;
using Unity.Lifetime;
using Unity.AspNet.Mvc;


namespace SdsExamOlvido
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //unity container
            var container = new UnityContainer();

            //DbContext
            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());

            // Register services here
            container.RegisterType<IRecyclableTypeService, RecyclableTypeService>(new HierarchicalLifetimeManager());

            //unity dependency resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
