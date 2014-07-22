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
        /// <param name="message">Текст сообщения</param>
        /// <param name="header">Заголовок сообщения</param>
        /// <param name="error">Индикатор ошибки. True - иконка с ошибкой, False или отсутствие аргумента - иконка с информацией</param>
        /// <returns></returns>
        public ActionResult MessageBox(string message, string header, bool? error)
        {
          ViewBag.Message = message;
          ViewBag.Header = header;

          if (error.HasValue)
            ViewBag.Error = error;
          else
            ViewBag.Error = false;

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
