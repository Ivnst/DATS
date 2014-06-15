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
            var cookie = Request.Cookies["ss"]; //SS - selected stadium
            int stadiumId = Convert.ToInt32(cookie == null ? "-1" : cookie.Value);

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
            var cookie = new HttpCookie("ss", value.Id.ToString());
            Response.SetCookie(cookie);
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

            //view cookie
            var cookie = Request.Cookies["sm"]; //SM - selected match
            int matchId = Convert.ToInt32(cookie == null ? "-1" : cookie.Value);

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
            var cookie = new HttpCookie("sm", value.Id.ToString());
            Response.SetCookie(cookie);
          }
        }

        #endregion

        //
        // GET: /Home/

        public ActionResult Index()
        {
          List<Stadium> stadiums = Repository.GetAllStadiums();
          if(stadiums.Count == 0)
          {
            //RedirectToAction("NoStadiums");
            throw new Exception();
          }

          //определяем текущий стадион (Выбранный)
          Stadium currentStadium = this.CurrentStadium;
          Match currentMatch = this.CurrentMatch;

          //заполняем данные для View
          ViewBag.Stadiums = stadiums;
          ViewBag.CurrentStadium = currentStadium;
          ViewBag.CurrentMatch = currentMatch;
          ViewBag.Matches = Repository.GetMatchesByStadium(currentStadium);
          ViewBag.SectorsInfo = Repository.GetSectorsStatistics(currentMatch);

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


        public ActionResult EditSector(int sid)
        {
          return View();
        }


        public ActionResult Example()
        {
          return View();
        }

    }
}
