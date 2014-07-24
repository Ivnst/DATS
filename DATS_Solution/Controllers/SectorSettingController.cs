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

        public ActionResult Index(int? sid)
        {
            ViewBag.Tab = 2;
            ViewBag.Stadiumes = Repository.GetAllStadiums();

            if (sid == null)
            {
                // если sid нет, выбераем первый встретившийся стадион
                var FindFirstStadium = Repository.Stadiums.FirstOrDefault<Stadium>();

                if (FindFirstStadium == null)
                {
                    throw new ArgumentNullException("Ошибка! Отсутсвуют стадионы в справочнике стадионов.");
                }
                else
                {
                    ViewBag.Stadium = FindFirstStadium;
                    return View(Repository.Sectors.Where(p => p.StadiumId == FindFirstStadium.Id));
                }
            }
            else
            {
                ViewBag.Stadium = Repository.FindStadium((int)sid);
                if (ViewBag.Stadium == null)
                {
                  logger.Warn("/SectorSetting/Index : Не найден указанный стадион. sid = " + sid.ToString());
                  string msgKey = PrepareMessageBox("Не найден указанный стадион!", "Внимание!", true);
                  return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
                }


                return View(Repository.Sectors.Where(p => p.StadiumId == (int)sid));
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
                string msg = string.Format(@"Сектор ""{0}"" успешно сохранен.", sector.Name);
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "SectorSetting", new { sid = TDStadiumId });
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
                string msg = string.Format(@"Сектор ""{0}"" успешно создан.", sector.Name); ;
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "SectorSetting", new { sid = TDStadiumId });
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
            ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Deleted;
            Repository.SaveChanges();

            string msg = string.Format(@"Сектор был удален.", sector.Name);
            TempData["message"] = msg;
            logger.Info(msg);
            return RedirectToAction("Index", "SectorSetting", new { sid = sector.StadiumId });
        }


        [HttpPost]
        public ActionResult Copy(int id)
        {
          Sector sector =  Repository.FindSector(id);
          if(sector == null)
          {
            string msgKey = PrepareMessageBox("Не найден указанный сектор!", "Внимание!", true);
            return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
          }

          try
          {
            Sector newSector = Repository.CopySector(sector);
            string msg = string.Format(@"Сектор ""{0}"" успешно скопирован в новый сектор ""{1}"".", sector.Name, newSector.Name);
            TempData["message"] = msg;
            logger.Info(msg);
            
            string messageKey = PrepareMessageBox(string.Format("Создана копия сектора с названием '{0}'!", newSector.Name), "Внимание!", false);
            return RedirectToAction("Index", "SectorSetting", new { sid = sector.StadiumId, notify = messageKey });
          }
          catch (System.Exception ex)
          {
            string msg = "При копировании сектора произошла ошибка!\n " + ex.Message;
            logger.Error(msg, ex);
            string messageKey = PrepareMessageBox(msg, "Внимание!", true);
            return RedirectToAction("Index", "SectorSetting", new { sid = sector.StadiumId, notify = messageKey });
          }

        }
    }
}
