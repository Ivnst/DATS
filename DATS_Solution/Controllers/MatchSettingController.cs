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

        [ChildActionOnly]
        public ActionResult Index(int? sid)
        {
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
              return PartialView(Repository.Matches.Where(p => p.StadiumId == FindFirstStadium.Id));
            }
          }
          else
          {
            ViewBag.Stadium = Repository.FindStadium((int)sid);
            return PartialView(Repository.Matches.Where(p => p.StadiumId == (int)sid));
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

                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно сохранено.", match.Name);
                logger.Info(TempData["message"]);
                return RedirectToAction("Matches", "Settings", new { sid = match.StadiumId });
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

                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                logger.Info(TempData["message"]);
                return RedirectToAction("Matches", "Settings", new { sid = match.StadiumId });
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

          TempData["message"] = string.Format(@"Мероприятие было удалено.", match.Name);
          logger.Info(TempData["message"]);
          return RedirectToAction("Matches", "Settings", new { sid = match.StadiumId });
        }


    }
}
