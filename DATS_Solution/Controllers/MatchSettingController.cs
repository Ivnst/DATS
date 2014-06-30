﻿using System;
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
       
          if(sid == null)
            {
            // если sid нет и в cookie выбераем первый встретившийся стадион
            var FindFirst = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == z.Id);

            if (FindFirst == null)
            {
            // если, справочник стадиона не заполнен, то показываем не фильтруя (мероприятия ещё не создавались)
                ViewBag.ChooseStadium = "Фильтровать по стадионам";
                return PartialView(Repository.Matches);
            }
            else
            {
                ViewBag.ChooseStadiumId = FindFirst.Id;
                ViewBag.ChooseStadium = FindFirst.Name;
                return PartialView(Repository.Matches.Where(p => p.StadiumId == FindFirst.Id));
            }
        } else
            {
            ViewBag.ChooseStadiumId = sid;
            ViewBag.ChooseStadium = Repository.Stadiums.Where(s => s.Id == sid).Distinct().Select(k => k.Name).Max();
            return PartialView(Repository.Matches.Where(p => p.StadiumId == sid));
            }

        }

        

        public ActionResult Edit(int id)
        {
            
            Match match = Repository.Matches
              .FirstOrDefault(p => p.Id == id);

            ViewBag.StadiumId = match.StadiumId;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == match.StadiumId).Select(p => p.Name).Max();

            return PartialView(match);
        }

        [HttpPost]
        public ActionResult Edit(Match match)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Match>(match).State = EntityState.Modified;
                Repository.SaveChanges();
                TempData["StadiumId"] = match.StadiumId;
                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно сохранено.", match.Name);
                return RedirectToAction("Matches", "Settings");
            }
            else
            {

                ViewBag.StadiumId = match.StadiumId;
                ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == match.StadiumId).Select(p => p.Name).Max();

                return View(match);

            }
        }


        [HttpGet]
        public ActionResult Create(int id)
        {

            ViewBag.StadiumId = id;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == id).Select(p => p.Name).Max();

            return PartialView(new Match());
        }

        [HttpPost]
        public ActionResult Create(Match match)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Match>(match).State = EntityState.Added;
                // Repository.Matches.Add(match);
                Repository.SaveChanges();
                TempData["StadiumId"] = match.StadiumId;
                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                return RedirectToAction("Matches", "Settings");
            }
            else
            {

                ViewBag.StadiumId = match.StadiumId;
                ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == match.StadiumId).Select(p => p.Name).Max();

                return View(match);

            }
        }


        public ActionResult Delete(int id)
        {
            Match match = Repository.Matches
              .FirstOrDefault(p => p.Id == id);

            ViewBag.StadiumId = match.StadiumId;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == match.StadiumId).Select(p => p.Name).Max();

            return PartialView(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
            ((DbContext)Repository).Entry<Match>(match).State = EntityState.Deleted;
                TempData["StadiumId"] = match.StadiumId;
                Repository.SaveChanges();
                TempData["message"] = string.Format(@"Мероприятие было удалено.", match.Name);
                return RedirectToAction("Matches", "Settings");
        }


    }
}
