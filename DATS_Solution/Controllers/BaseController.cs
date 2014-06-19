using System.Web.Mvc;
using Ninject;
using System;
using System.Web;

namespace DATS.Controllers
{
    public class BaseController : Controller
    {
      [Inject]
      public IRepository Repository { get; set; }

      protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


      #region <Functions>
      
      /// <summary>
      /// Считывает целочисленное значение из куки
      /// </summary>
      /// <param name="cookieName"></param>
      /// <returns></returns>
      protected int ReadIntValueFromCookie(string cookieName)
      {
        if (string.IsNullOrEmpty(cookieName)) throw new ArgumentNullException("cookieName");
        var cookie = Request.Cookies[cookieName]; //SS - selected stadium
        return Convert.ToInt32(cookie == null ? "-1" : cookie.Value);
      }

      /// <summary>
      /// Записывает целочисленное значение в куку
      /// </summary>
      /// <param name="cookieName"></param>
      /// <param name="value"></param>
      protected void WriteIntValueIntoCookie(string cookieName, int value)
      {
        if (string.IsNullOrEmpty(cookieName)) throw new ArgumentNullException("cookieName");
        var cookie = new HttpCookie(cookieName, value.ToString());
        Response.SetCookie(cookie);
      }

      #endregion
    }
}
