using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DATS.Controllers
{
    public class MatchSettingController : BaseController
    {

        private IRepository repository;

        public MatchSettingController(IRepository repo)
        {
            repository = repo;
        }



        //
        // GET: /Setting/

        public ViewResult Index()
        {
            return View(repository.Matches);

        }

        

        public ViewResult Edit(int id)
        {
            
            Match match = repository.Matches
              .FirstOrDefault(p => p.Id == id);

            // start selectList
            IEnumerable<SelectListItem> selectList =
            from s in repository.Stadiumes
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
                repository.SaveMatch(match);
                TempData["message"] = string.Format("{0} has been saved", match.Name);
                return RedirectToAction("Index");
            }
            else
            {
                // start selectList
                IEnumerable<SelectListItem> selectList =
                from s in repository.Stadiumes
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
            from s in repository.Stadiumes
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
                repository.SaveMatch(match);
                TempData["message"] = string.Format("{0} has been saved", match.Name);
                return RedirectToAction("Index");
            }
            else
            {
                // start selectList
                IEnumerable<SelectListItem> selectList =
                from s in repository.Stadiumes
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
            Match match = repository.Matches
              .FirstOrDefault(p => p.Id == id);
            return View(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
                repository.DeleteMatch(match);
                TempData["message"] = string.Format("{0} has been deleted", match.Name);
                return RedirectToAction("Index");
        }


    }
}
