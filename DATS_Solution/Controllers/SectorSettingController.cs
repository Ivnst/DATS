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
            FillUserDetail();
            ViewBag.Tab = 2;
            ViewBag.Stadiumes = Repository.GetAllStadiums();

            if (sid == null)
            {
                // если sid нет, выбираем первый встретившийся стадион
                var FindFirstStadium = Repository.Stadiums.FirstOrDefault<Stadium>();

                if (FindFirstStadium == null)
                {
                  throw new ArgumentNullException("Ошибка! Отсутствуют стадионы в справочнике стадионов.");
                }
                sid = FindFirstStadium.Id;
            }

            ViewBag.Stadium = Repository.FindStadium((int)sid);
            if (ViewBag.Stadium == null)
            {
              logger.Warn("/SectorSetting/Index : Не найден указанный стадион. sid = " + sid.ToString());
              string msgKey = PrepareMessageBox("Не найден указанный стадион!", "Внимание!", true);
              return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
            }

            return View(Repository.Sectors.Where(p => p.StadiumId == (int)sid).OrderBy(p => p.Name));
        }

        #region <Edit>

        public ActionResult Edit(int id)
        {

            Sector sector = Repository.FindSector(id);

            if (sector == null)
            {
                logger.Warn("/SectorSetting/Edit : Указанный сектор не найден. id = " + id.ToString());
                string msgKey = PrepareMessageBox("Указанный сектор не найден!", "Внимание!", true);
                return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
            }

            ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

            return PartialView(sector);
        }

        [HttpPost]
        public ActionResult Edit(Sector sector)
        {
            sector.Name = Utils.DeleteSpecialCharacters(sector.Name);

            if (ModelState.IsValid)
            {
                try
                {
                    ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Modified;
                    Repository.SaveChanges();
                    int TDStadiumId = sector.StadiumId;
                    string msg = string.Format(@"Сектор ""{0}"" успешно сохранен.", sector.Name);
                    TempData["message"] = msg;
                    logger.Info(msg);
                    return RedirectToAction("Index", "SectorSetting", new { sid = TDStadiumId });
                }
                catch (System.Exception ex)
                {
                    logger.Error("/SectorSetting/Edit : Ошибка", ex);
                    string msgKey = PrepareMessageBox("При сохранении сектора возникла ошибка!", "Внимание!", true);
                    return RedirectToAction("Index", "SectorSetting", new { notify = msgKey, sid = sector.StadiumId });
                }
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

                return View(sector);

            }
        }
        
        #endregion

        #region <Create>

        [HttpGet]
        public ActionResult Create(int id)
        {

            Stadium stadium = Repository.FindStadium(id);

            if (stadium == null)
            {
                logger.Warn("/SectorSetting/Edit : Указанный стадион не найден. id = " + id.ToString());
                string msgKey = PrepareMessageBox("Указанный стадион не найден!", "Внимание!", true);
                return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
            }

            ViewBag.Stadium = stadium;

            Sector sector = new Sector();
            sector.StadiumId = id;

            return PartialView(sector);
        }

        [HttpPost]
        public ActionResult Create(Sector sector)
        {
            sector.Name = Utils.DeleteSpecialCharacters(sector.Name);
            if (ModelState.IsValid)
            {
                try
                {
                    ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Added;
                    Repository.SaveChanges();
                    int TDStadiumId = sector.StadiumId;
                    string msg = string.Format(@"Сектор ""{0}"" успешно создан.", sector.Name); ;
                    TempData["message"] = msg;
                    logger.Info(msg);
                    return RedirectToAction("Index", "SectorSetting", new { sid = TDStadiumId });
                }
                catch (System.Exception ex)
                {
                    logger.Error("/SectorSetting/Create : Ошибка", ex);
                    string msgKey = PrepareMessageBox("При создании сектора возникла ошибка!", "Внимание!", true);
                    return RedirectToAction("Index", "SectorSetting", new { notify = msgKey, sid = sector.StadiumId });
                }
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(sector.StadiumId);

                return View(sector);

            }
        }

        #endregion

        #region <Delete>


        public ActionResult Delete(int id)
        {
            Sector sector = Repository.FindSector(id);

            if (sector == null)
            {
                logger.Warn("/SectorSetting/Delete : Указанный сектор не найден. id = " + id.ToString());
                string msgKey = PrepareMessageBox("Указанный сектор не найден!", "Внимание!", true);
                return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
            }

            return PartialView(sector);
        }



        [HttpPost]
        public ActionResult Delete(Sector sector)
        {
            if(Repository.GetCountOfSoldPlacesInSector(sector) != 0)
            {
              string msgKey = PrepareMessageBox("По этому сектору проводились продажи! Удаление запрещено!", "Внимание!", true);
              return RedirectToAction("Index", "SectorSetting", new { notify = msgKey });
            }

            try
            {
                ((DbContext)Repository).Entry<Sector>(sector).State = EntityState.Deleted;
                Repository.SaveChanges();

                string msg = string.Format(@"Сектор был удален.", sector.Name);
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "SectorSetting", new { sid = sector.StadiumId });
            }
            catch (System.Exception ex)
            {
                logger.Error("/SectorSetting/Delete : Ошибка", ex);
                string msgKey = PrepareMessageBox("При удалении сектора возникла ошибка!", "Внимание!", true);
                return RedirectToAction("Index", "SectorSetting", new { notify = msgKey, sid = sector.StadiumId });
            }
        }


        #endregion

        #region <Copy>

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

        #endregion

    }
}
