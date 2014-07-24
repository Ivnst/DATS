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
        public ActionResult Index()
        {
            ViewBag.Tab = 1;
            return View(Repository.Stadiums);
        }

        #region <Create>

        public ActionResult Create()
        {
          return PartialView(new Stadium());
        }

        [HttpPost]
        public ActionResult Create(Stadium stadium)
        {
          if (!ModelState.IsValid)
            return View(stadium);

          try
          {
            Repository.Stadiums.Add(stadium);
            Repository.SaveChanges();

            string msg = string.Format(@"Стадион ""{0}"" успешно создан.", stadium.Name);
            TempData["message"] = msg;
            logger.Info(msg);
            return RedirectToAction("Index", "StadiumSetting");
          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Create : Ошибка", ex);
            string msgKey = PrepareMessageBox("При создании стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }
        }

        #endregion

        #region <Edit>

        public ActionResult Edit(int id)
        {
          Stadium stadium = Repository.FindStadium(id);

          if (stadium == null)
          {
            logger.Warn("/StadiumSetting/Edit : Указанный стадион не найден. id = " + id.ToString());
            string msgKey = PrepareMessageBox("Указанный стадион не найден!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          return PartialView(stadium);
        }

        [HttpPost]
        public ActionResult Edit(Stadium stadium)
        {
          if (!ModelState.IsValid)
            return View(stadium);

          try
          {
            Repository.SaveEntry<Stadium>(stadium);

            string msg = string.Format(@"Стадион ""{0}"" успешно сохранен.", stadium.Name); ;
            TempData["message"] = msg;
            logger.Info(msg);
            return RedirectToAction("Index", "StadiumSetting");
          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Edit : Ошибка", ex);
            string msgKey = PrepareMessageBox("При сохранении стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

        }
        #endregion

        #region <Delete>

        /// <summary>
        /// Удаление стадиона
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
          Stadium stadium = Repository.FindStadium(id);

          if (stadium == null)
          {
            logger.Warn("/StadiumSetting/Delete : Указанный стадион не найден. id = " + id.ToString());
            string msgKey = PrepareMessageBox("Указанный стадион не найден!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }


          return PartialView(stadium);
        }

        [HttpPost]
        public ActionResult Delete(Stadium stadium)
        {
          int id = stadium.Id;
          stadium = Repository.FindStadium(id);
          if (stadium == null)
          {
            logger.Error("/StadiumSetting/Delete : Удаление удалённого стадиона! sid=" + id.ToString());
            string msgKey = PrepareMessageBox("При удалении стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          try
          {
            Repository.DeleteEntry<Stadium>(stadium);

            string msg = string.Format(@"Стадион ""{0}"" был удалён.", stadium.Name);
            TempData["message"] = msg;
            logger.Info(msg);
            return RedirectToAction("Index", "StadiumSetting");
          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Delete : Ошибка", ex);
            string msgKey = PrepareMessageBox("При удалении стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }
        }

        #endregion

    }
}
