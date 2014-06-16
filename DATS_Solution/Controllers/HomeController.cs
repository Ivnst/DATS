using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS.Controllers
{
    public class HomeController : BaseController
    {
        #region <Properties>
        
        /// <summary>
        /// Текущий стадион
        /// </summary>
        public Stadium CurrentStadium
        {
          get
          {
            //view cookie
            int stadiumId = ReadIntValueFromCookie("ss");//SS - selected stadium

            //search stadium (check cookie value)
            Stadium stadium = Repository.Stadiums.FirstOrDefault<Stadium>(s => s.Id == stadiumId);
            if(stadium == null)
            {
              //Если не нашли, берем первый стадион
              stadium = Repository.Stadiums.FirstOrDefault<Stadium>();
              //и сохраняем его
              CurrentStadium = stadium;
            }

            return stadium;
          }
          set
          {
            if (value == null) return;

            //create cookie
            WriteIntValueIntoCookie("ss", value.Id);
          }
        }

        /// <summary>
        /// Текущее мероприятие
        /// </summary>
        public Match CurrentMatch
        {
          get
          {
            Stadium stadium = this.CurrentStadium;
            if (stadium == null) return null;

            //view cookie
            int matchId = ReadIntValueFromCookie("sm");//SM - selected match

            Match match = Repository.Matches.FirstOrDefault<Match>(m => m.Id == matchId);
            if (match == null || match.StadiumId != stadium.Id)
            {
              //Если не нашли, берем первое мероприятие
              match = Repository.GetMatchesByStadium(stadium).FirstOrDefault<Match>();
              //и сохраняем его
              CurrentMatch = match;
            }

            return match;
          }
          set
          {
            if (value == null) return;

            //create cookie
            WriteIntValueIntoCookie("sm", value.Id);
          }
        }

        #endregion

        #region <Actions>
        //
        // GET: /Home/

        public ActionResult Index()
        {
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
          Stadium stadium = Repository.Stadiums.FirstOrDefault<Stadium>(s => s.Id == sid);
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
        /// Страница для продажи билетов
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult EditSector(int sid)
        {
          return View();
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

        #endregion

        #region <Methods>

        /// <summary>
        /// заполняем данные для View
        /// </summary>
        /// <param name="currentStadium"></param>
        /// <param name="currentMatch"></param>
        private void FillViewBag(Stadium currentStadium, Match currentMatch)
        {
          if(currentStadium == null)
          {
            currentStadium = new Stadium();
            currentStadium.Name = "Стадионы не найдены";
          }

          //заполняем данные для View
          ViewBag.Stadiums = Repository.GetAllStadiums();
          ViewBag.CurrentStadium = currentStadium;
          ViewBag.CurrentMatch = currentMatch;
          ViewBag.Matches = Repository.GetMatchesByStadium(currentStadium);
          ViewBag.SectorsInfo = Repository.GetSectorsStatistics(currentMatch);
        }

        #endregion
    }
}
