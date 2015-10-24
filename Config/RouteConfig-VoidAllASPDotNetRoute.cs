using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MovieRating.WebService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //exclude template folder from routing
            routes.IgnoreRoute("Folder_Where_Angular_View_Reside/{*pathInfo}");

            //add routes for api calls and stuff you need mapped to an asp controller here
            /*
             ...
            */

            //always deliver the main template for all requests
            routes.MapRoute(
                name: "SPA",
                url: "{*catchall}",
                defaults: new { controller = "Home", action = "Index" }
            );


            //============== default when first create web api controller ===========================
            //============== also, uncomment this if decide not to go for the spa way ===============
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}