using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
          //запись ip-адреса посетителя
          logger.Info("Enter to homepage:" + Request.UserHostAddress);

          List<Stadium> stadiums = Repository.GetAllStadiums();
          if(stadiums.Count == 0)
          {
            return RedirectToAction("Error", new { e = 1 });
          }

          //определяем текущий стадион (Выбранный)
          Stadium currentStadium = this.CurrentStadium;
          if (currentStadium == null) return RedirectToAction("Error", new { e = 1 });

          Match currentMatch = this.CurrentMatch;
          if (currentMatch == null) return RedirectToAction("Error", new { e = 2 });

          FillViewBag(currentStadium, currentMatch);
          return View();
        }


        /// <summary>
        /// Ссылка для выбора стадиона
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Stadium(int sid)
        {
          Stadium stadium = Repository.FindStadium(sid);
          if(stadium != null)
          {
            this.CurrentStadium = stadium;
          }

          return RedirectToAction("Index");
        }


        /// <summary>
        /// Ссылка для выбора мероприятия
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        public ActionResult Match(int mid)
        {
          Match match = Repository.Matches.FirstOrDefault<Match>(m => m.Id == mid);
          if (match != null)
          {
            this.CurrentMatch = match;
          }

          return RedirectToAction("Index");
        }


        /// <summary>
        /// Страница с ошибкой
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public ActionResult Error(int e)
        {
          FillViewBag(this.CurrentStadium, this.CurrentMatch);

          switch (e)
          {
            case 1:
              ViewBag.Message = "Стадионы не найдены! Добавить стадион можно на странице настроек.";
              break;
            case 2:
              ViewBag.Message = "У выбранного стадиона нет активных мероприятий! Добавить мероприятие можно на странице настроек.";
              break;
            default:
              ViewBag.Message = "Неизвестная ошибка.";
              break;
          }
          return View();
        }


        /// <summary>
        /// Пример использования Twitter Bootstrap
        /// </summary>
        /// <returns></returns>
        public ActionResult Example()
        {
          return View();
        }
    }
}
