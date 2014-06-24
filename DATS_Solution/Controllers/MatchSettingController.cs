using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DATS.Controllers
{
    public class MatchSettingController : BaseController
    {


        public ViewResult Index()
        {
            return View(Repository.Matches);

        }

        

        public ViewResult Edit(int id)
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

            return View(match);
        }

        [HttpPost]
        public ActionResult Edit(Match match)
        {
            if (ModelState.IsValid)
            {
                Repository.SaveMatch(match);
                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно сохранено.", match.Name);
                return RedirectToAction("Index");
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
        public ViewResult Create()
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

            return View(new Match() );
        }

        [HttpPost]
        public ActionResult Create(Match match)
        {
            if (ModelState.IsValid)
            {
                Repository.SaveMatch(match);
                TempData["message"] = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                return RedirectToAction("Index");
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


        public ViewResult Delete(int id)
        {
            Match match = Repository.Matches
              .FirstOrDefault(p => p.Id == id);
            return View(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
                Repository.DeleteMatch(match);
                TempData["message"] = string.Format(@"Мероприятие было удалено.", match.Name);
                return RedirectToAction("Index");
        }


    }
}
