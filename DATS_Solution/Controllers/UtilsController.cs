using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS.Controllers
{
    public class UtilsController : BaseController
    {
        //
        // GET: /Utils/

        /// <summary>
        /// Всплывающее окно с заданным содержимым и заголовком
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult MessageBox(string message, string header)
        {
          ViewBag.Message = message;
          ViewBag.Header = header;
          return PartialView();
        }


        /// <summary>
        /// Сохранение данных в кэше
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CacheData(string data)
        {
          string key = StoreDataInCache(data);
          return Content(key);
        }


        /// <summary>
        /// Возвращение данных из кэша
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCachedData(string key)
        {
          string data = GetDataFromCache(key);
          return Content(data);
        }
    }
}
