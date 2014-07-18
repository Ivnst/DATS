using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace DATS.Controllers
{
    public class MatchSettingController : BaseController
    {

        public ActionResult Index(int? sid)
        {
          ViewBag.Tab = 3;
          ViewBag.Stadiumes = Repository.GetAllStadiums();

          if (sid == null)
          {
            // если sid нет, выбераем первый встретившийся стадион
            var FindFirstStadium = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == z.Id);

            if (FindFirstStadium == null)
            {
              throw new ArgumentNullException("Ошибка! Отсутсвуют стадионы в справочнике стадионов.");
            }
            else
            {
              ViewBag.Stadium = FindFirstStadium;
              return View(Repository.Matches.Where(p => p.StadiumId == FindFirstStadium.Id));
            }
          }
          else
          {
            ViewBag.Stadium = Repository.FindStadium((int)sid);
            return View(Repository.Matches.Where(p => p.StadiumId == (int)sid));
          }

        }

        

        public ActionResult Edit(int id)
        {

            Match match = Repository.FindMatch(id);

            ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

            return PartialView(match);
        }

        [HttpPost]
        public ActionResult Edit(Match match)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Match>(match).State = EntityState.Modified;
                Repository.SaveChanges();

                string msg = string.Format(@"Мероприятие ""{0}"" успешно сохранено.", match.Name);
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

                return View(match);

            }
        }


        [HttpGet]
        public ActionResult Create(int id)
        {
            ViewBag.Stadium = Repository.FindStadium(id);

            Match match = new Match();
            match.StadiumId = id;

            return PartialView(match);
        }

        [HttpPost]
        public ActionResult Create(Match match)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Match>(match).State = EntityState.Added;
                Repository.SaveChanges();
                
                string msg = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
            }
            else
            {
                ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

                return View(match);
            }
        }


        public ActionResult Delete(int id)
        {
            Match match = Repository.FindMatch(id);

            return PartialView(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
          ((DbContext)Repository).Entry<Match>(match).State = EntityState.Deleted;
          Repository.SaveChanges();

          string msg = string.Format(@"Мероприятие было удалено.", match.Name);
          TempData["message"] = msg;
          logger.Info(msg);
          return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
        }


    }
}
