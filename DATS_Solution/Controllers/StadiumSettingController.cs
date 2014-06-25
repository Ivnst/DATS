using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DATS.Controllers
{
    public class StadiumSettingController : BaseController
    {
        //
        // GET: /StadiumSetting/
        [ChildActionOnly]
        public ActionResult Index()
        {
            return PartialView(Repository.Stadiums);
        }

        [ChildActionOnly]
        public ActionResult Create()
        {
          return PartialView(new Stadium());
        }

        [HttpPost]
        public ActionResult Create(Stadium stadium)
        {
          if (ModelState.IsValid)
          {
            Repository.Stadiums.Add(stadium);
            Repository.SaveChanges();
            return RedirectToAction("Stadiums", "Settings");
          }
          else
          {
            return View(stadium);
          }
        }


        public ActionResult Edit(int id)
        {
          Stadium stadium = Repository.Stadiums.FirstOrDefault(p => p.Id == id);

          if (stadium == null)
          {
            return RedirectToAction("Index");
          }

          return PartialView(stadium);
        }

        [HttpPost]
        public ActionResult Edit(Stadium stadium)
        {
          if (ModelState.IsValid)
          {
            ((DbContext)Repository).Entry<Stadium>(stadium).State = EntityState.Modified;
            Repository.SaveChanges();
            return RedirectToAction("Stadiums", "Settings");
          }
          else
          {
            return View(stadium);
          }
        }


        /// <summary>
        /// Удаление стадиона
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
          Stadium item = Repository.Stadiums.FirstOrDefault(p => p.Id == id);
          return PartialView(item);
        }

        [HttpPost]
        public ActionResult Delete(Stadium stadium)
        {
          ((DbContext)Repository).Entry<Stadium>(stadium).State = EntityState.Deleted;
          Repository.SaveChanges();
          return RedirectToAction("Stadiums", "Settings");
        }

    }
}
