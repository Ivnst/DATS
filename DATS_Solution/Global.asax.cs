using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;

namespace DATS
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    Thread reservationTimeoutThread;

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      //запуск отдельного потока, который постоянно будет проверять бронь на истечение "срока годности"
      reservationTimeoutThread = new Thread(new ThreadStart(ReturnReservationFunc));
      reservationTimeoutThread.IsBackground = true;
      reservationTimeoutThread.Start();

      //настройка часового пояса логгера
      NLog.Time.TimeSource.Current = new CustomTimeZoneTimeSource() { Zone = "FLE Standard Time" };
    }

    /// <summary>
    /// Вызов в бесконечном цикле проверки на истечения срока брони с учётом настроек стадионов и мероприятий.
    /// </summary>
    private void ReturnReservationFunc()
    {
      for (; ; )
      {
        IRepository repository = DependencyResolver.Current.GetService<IRepository>();
        repository.RemoveReservationsByTimeout();
        Thread.Sleep(TimeSpan.FromMinutes(1));
      }
    }
  }
}