using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DATS
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

// ловим случаи ошибочного ввода URL без параметров
routes.MapRoute("CatchNullCreate", "{controller}/Create", new { action = "RouteError" } );
routes.MapRoute("CatchNullEdit", "{controller}/Edit", new { action = "RouteError" } );
routes.MapRoute("CatchNullDelete", "{controller}/Delete", new { action = "RouteError" } );
routes.MapRoute("CatchNullSave", "{controller}/Save", new { action = "RouteError" });
routes.MapRoute("CatchNullCopy", "{controller}/Copy", new { action = "RouteError" });

// основной
      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}