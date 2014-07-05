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
                // если sid нет выбераем первый встретившийся стадион
                var FindFirst = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == z.Id);

                if (FindFirst == null)
                {
                    // если, справочник стадиона не заполнен, то показываем не фильтруя (сектора ещё не создавались)
                    ViewBag.ChooseStadium = "Сначала необходимо добавить хотябы один стадион.";
                    return PartialView(Repository.Sectors);
                }
                else
                {
                    ViewBag.ChooseStadiumId = FindFirst.Id;
                    ViewBag.ChooseStadium = FindFirst.Name;
                    return PartialView(Repository.Sectors.Where(p => p.StadiumId == FindFirst.Id));
                }
            }
            else
            {
                ViewBag.ChooseStadiumId = sid;
                ViewBag.ChooseStadium = Repository.Stadiums.Where(s => s.Id == sid).Distinct().Select(k => k.Name).Max();
                return PartialView(Repository.Sectors.Where(p => p.StadiumId == sid));
            }

        }


        public ActionResult Edit(int id)
        {

            Sector sector = Repository.Sectors.FirstOrDefault(p => p.Id == id);

            ViewBag.StadiumId = sector.StadiumId;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == sector.StadiumId).Select(p => p.Name).Max();

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

                ViewBag.StadiumId = sector.StadiumId;
                ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == sector.StadiumId).Select(p => p.Name).Max();

                return View(sector);

            }
        }


        [HttpGet]
        public ActionResult Create(int id)
        {

            ViewBag.StadiumId = id;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == id).Select(p => p.Name).Max();

            return PartialView(new Sector());
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

                ViewBag.StadiumId = sector.StadiumId;
                ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == sector.StadiumId).Select(p => p.Name).Max();

                return View(sector);

            }
        }


        public ActionResult Delete(int id)
        {
            Sector sector = Repository.Sectors
              .FirstOrDefault(p => p.Id == id);

            ViewBag.StadiumId = sector.StadiumId;
            ViewBag.StadiumName = Repository.Stadiums.Where(p => p.Id == sector.StadiumId).Select(p => p.Name).Max();

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
