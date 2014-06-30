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
              int CookieId = ReadIntValueFromCookie("CurrSadiumForMatches");
              if (CookieId != -1)
              {
                  sid = CookieId;
              }
          }
        
          if(sid == null)
            {
            ViewBag.ChooseStadium = "Фильтровать по стадионам";
            return PartialView(Repository.Matches);
        } else
            {
                ViewBag.ChooseStadium = Repository.Stadiums.Where(s => s.Id == sid).Distinct().Select(k => k.Name).Max();
            WriteIntValueIntoCookie("CurrSadiumForMatches", (int)sid);
            return PartialView(Repository.Matches.Where(p => p.StadiumId == sid));
            }

        }

        

        public ActionResult Edit(int id)
        {
            
            Match match = Repository.Matches
              .FirstOrDefault(p => p.Id == id);

            // start selectList
            IEnumerable<SelectListItem> selectList =
            from s in Repository.Stadiums
            select new SelectListItem
            {
                Selected = (s.Id == match.StadiumId),
                Value = s.Id.ToString(),
                Text = s.Name
            };
            ViewBag.Stadiumes = selectList;
            // end selectList

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
                return RedirectToAction("Matches", "Settings");
            }
            else
            {
                // start selectList
                IEnumerable<SelectListItem> selectList =
                from s in Repository.Stadiums
                select new SelectListItem
                {
                    Selected = (s.Id == match.StadiumId),
                    Value = s.Id.ToString(),
                    Text = s.Name
                };
                ViewBag.Stadiumes = selectList;
                // end selectList

                return View(match);

            }
        }


        [HttpGet]
        public ActionResult Create()
        {
            // start selectList
            IEnumerable<SelectListItem> selectList =
            from s in Repository.Stadiums
            select new SelectListItem
            {
                Selected = false,
                Value = s.Id.ToString(),
                Text = s.Name
            };
            ViewBag.Stadiumes = selectList;
            // end selectList

            return PartialView(new Match());
        }

        [HttpPost]
        public ActionResult Create(Match match)
        {
            if (ModelState.IsValid)
            {
                Repository.Matches.Add(match);
                Repository.SaveChanges();
                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                return RedirectToAction("Matches", "Settings");
            }
            else
            {
                // start selectList
                IEnumerable<SelectListItem> selectList =
                from s in Repository.Stadiums
                select new SelectListItem
                {
                    Selected = (s.Id == match.StadiumId),
                    Value = s.Id.ToString(),
                    Text = s.Name
                };
                ViewBag.Stadiumes = selectList;
                // end selectList

                return View(match);

            }
        }


        public ActionResult Delete(int id)
        {
            Match match = Repository.Matches
              .FirstOrDefault(p => p.Id == id);
            return PartialView(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
            ((DbContext)Repository).Entry<Match>(match).State = EntityState.Deleted;
            Repository.SaveChanges();
                TempData["message"] = string.Format(@"Мероприятие было удалено.", match.Name);
                return RedirectToAction("Matches", "Settings");
        }


    }
}
