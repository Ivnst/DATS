using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Text;

namespace DATS.Controllers
{
    public class StadiumSettingController : BaseController
    {
        //
        // GET: /StadiumSetting/
        public ActionResult Index()
        {
            FillUserDetail();
            ViewBag.Tab = 1;
            return View(Repository.Stadiums.OrderBy(s => s.Name));
        }

        #region <Create>

        public ActionResult Create()
        {
          return PartialView(new Stadium());
        }

        [HttpPost]
        public ActionResult Create(Stadium stadium)
        {
          stadium.Name = Utils.DeleteSpecialCharacters(stadium.Name);
          stadium.Address = Utils.DeleteSpecialCharacters(stadium.Address);

          if (!ModelState.IsValid)
            return View(stadium);

          try
          {
            //создаём новый стадион
            Repository.Stadiums.Add(stadium);
            Repository.SaveChanges();
          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Create : Ошибка", ex);
            string msgKey = PrepareMessageBox("При создании стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          try
          {
            //Сохранение изображения стадиона
            string ticksAsName = DateTime.UtcNow.Ticks.ToString();
            string tempName = SaveFile(ticksAsName);

            if (tempName != null)
            {
              //сохраняем его картинку
              FileInfo fi = new FileInfo(tempName);
              string imageName = string.Format("stadium{0:000}{1}", stadium.Id, fi.Extension);
              RenameFile(tempName, imageName);

              //присваиваем созданному стадиону его картинку
              stadium.SchemePath = "/Content/StadiumsImages/" + imageName;
              Repository.SaveEntry<Stadium>(stadium);
              Repository.SaveChanges();
            }

          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Create : Ошибка при сохранении изображения", ex);
            string msgKey = PrepareMessageBox("При создании стадиона возникла ошибка! Стадион сохранён без изображения!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          string msg = string.Format(@"Стадион ""{0}"" успешно создан.", stadium.Name);
          TempData["message"] = msg;
          logger.Info(msg);
          return RedirectToAction("Index", "StadiumSetting");
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
          stadium.Name = Utils.DeleteSpecialCharacters(stadium.Name);
          stadium.Address = Utils.DeleteSpecialCharacters(stadium.Address);

          if (!ModelState.IsValid)
            return View(stadium);

          //Сохранение стадиона
          try
          {
            Repository.SaveEntry<Stadium>(stadium);
          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Edit : Ошибка", ex);
            string msgKey = PrepareMessageBox("При сохранении стадиона возникла ошибка!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          //Сохранение изображения стадиона
          try
          {
            string ticksAsName = DateTime.UtcNow.Ticks.ToString();
            string tempName = SaveFile(ticksAsName);

            if (tempName != null)
            {
              //сохраняем его картинку
              FileInfo fi = new FileInfo(tempName);
              string imageName = string.Format("stadium{0:000}{1}", stadium.Id, fi.Extension);
              RenameFile(tempName, imageName);

              //присваиваем созданному стадиону его картинку
              stadium.SchemePath = "/Content/StadiumsImages/" + imageName;
              Repository.SaveEntry<Stadium>(stadium);
              Repository.SaveChanges();
            }

          }
          catch (System.Exception ex)
          {
            logger.Error("/StadiumSetting/Create : Ошибка при сохранении изображения", ex);
            string msgKey = PrepareMessageBox("При создании стадиона возникла ошибка! Стадион сохранён без изображения!", "Внимание!", true);
            return RedirectToAction("Index", "StadiumSetting", new { notify = msgKey });
          }

          string msg = string.Format(@"Стадион ""{0}"" успешно сохранен.", stadium.Name); ;
          TempData["message"] = msg;
          logger.Info(msg);
          return RedirectToAction("Index", "StadiumSetting");
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
            string imagePath = stadium.SchemePath;
            Repository.DeleteEntry<Stadium>(stadium);

            FileInfo fi = new FileInfo(Server.MapPath(imagePath));
            if(fi.Exists)
            {
              System.IO.File.Delete(fi.FullName);
            }

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

        #region <Private Functions>
        /// <summary>
        /// Сохранение файла, переданного в запрос
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string SaveFile(string name)
        {
          string resultFileName = null;

          if (HttpContext.Request.Files["StadiumImage"] != null)
          {
            HttpPostedFileBase MyFile = HttpContext.Request.Files["StadiumImage"];
            if (MyFile.ContentLength == 0) return null;

            FileInfo fi = new FileInfo(MyFile.FileName);
            string ext = fi.Extension.ToLower();
            if(ext != ".jpeg" && ext != ".jpg" && ext != ".png" )
            {
              return null;
            }

            //get correct name
            string TargetLocation = Server.MapPath("~/Content/StadiumsImages");
            name = name + ext;
            resultFileName = Path.Combine(TargetLocation, name);

            if (MyFile.ContentLength > 0)
            {
              //загружаем содержимое файла
              int fileSize = MyFile.ContentLength;
              byte[] FileByteArray = new byte[fileSize];
              MyFile.InputStream.Read(FileByteArray, 0, fileSize);

              //сохраняем файл
              using (FileStream fs = new FileStream(resultFileName, FileMode.Create))
              {
                fs.Write(FileByteArray, 0, fileSize);
              }
            }
            return resultFileName;
          }
          return null;
        }

        /// <summary>
        /// Переименование сохранённого файла
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        private void RenameFile(string oldName, string newName)
        {
          if (string.IsNullOrEmpty(oldName)) throw new ArgumentNullException("oldName");
          if (string.IsNullOrEmpty(newName)) throw new ArgumentNullException("newName");

          string TargetLocation = Server.MapPath("~/Content/StadiumsImages");
          string oldPath = Path.Combine(TargetLocation, oldName);
          string newPath = Path.Combine(TargetLocation, newName);

          //если конечный файл уже существует, то удаляем
          if (System.IO.File.Exists(newPath))
          {
            System.IO.File.Delete(newPath);
          }

          //переименовываем существующий файл
          System.IO.File.Move(oldPath, newPath);
        }
        #endregion

    }
}
