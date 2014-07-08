using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DATS.Controllers
{
    public class SectorSettingController : BaseController
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
                    return PartialView(Repository.Sectors.Where(p => p.StadiumId == FindFirstStadium.Id));
                }
            }
            else
            {
                ViewBag.Stadium = Repository.FindStadium((int)sid);
                return PartialView(Repository.Sectors.Where(p => p.StadiumId == (int)sid));
            }

        }


        public ActionResult Edit(int id)
        {

            Sector sector = Repository.FindSector(id);

            ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

            return PartialView(sector);
        }

        [HttpPost]
        public ActionResult Edit(Sector sector)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Modified;
                Repository.SaveChanges();
                int TDStadiumId = sector.StadiumId;
                TempData["message"] = string.Format(@"Сектор ""{0}"" успешно сохранен.", sector.Name);
                return RedirectToAction("Sectors", "Settings", new { sid = TDStadiumId });
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

                return View(sector);

            }
        }


        [HttpGet]
        public ActionResult Create(int id)
        {

            ViewBag.Stadium = Repository.FindStadium(id);

            Sector sector = new Sector();
            sector.StadiumId = id;

            return PartialView(sector);
        }

        [HttpPost]
        public ActionResult Create(Sector sector)
        {
            if (ModelState.IsValid)
            {
                ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Added;
                Repository.SaveChanges();
                int TDStadiumId = sector.StadiumId;
                TempData["message"] = string.Format(@"Сектор ""{0}"" успешно создан.", sector.Name);
                return RedirectToAction("Sectors", "Settings", new { sid = TDStadiumId });
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

                return View(sector);

            }
        }


        public ActionResult Delete(int id)
        {
            Sector sector = Repository.FindSector(id);

            return PartialView(sector);
        }



        [HttpPost]
        public ActionResult Delete(Sector sector)
        {
            int TDStadiumId = sector.StadiumId;
            ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Deleted;
            Repository.SaveChanges();
            TempData["message"] = string.Format(@"Сектор был удален.", sector.Name);
            return RedirectToAction("Sectors", "Settings", new { sid = TDStadiumId });
        }


    }
}
