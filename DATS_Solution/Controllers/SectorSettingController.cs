﻿using System;
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
                var FindFirstStadium = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == z.Id);

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
            return RedirectToAction("Index", "SectorSetting");
          }

          try
          {
            Sector newSector = Repository.CopySector(sector);
            string msg = string.Format(@"Сектор ""{0}"" успешно скопирован в новый сектор ""{1}"".", sector.Name, newSector.Name);
            TempData["message"] = msg;
            logger.Info(msg);
          }
          catch (System.Exception ex)
          {
            string msg = "При копировании сектора произошла ошибка! " + ex.Message;
            TempData["message"] = msg;
            logger.Error(msg, ex);
          }

          return RedirectToAction("Index", "SectorSetting", new { sid = sector.StadiumId });
        }
    }
}
