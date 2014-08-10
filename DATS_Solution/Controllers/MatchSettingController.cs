using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace DATS.Controllers
{
    public class MatchSettingController : BaseController
    {

      public ActionResult Index(int? sid)
      {
          FillUserDetail();
          ViewBag.Tab = 3;
          ViewBag.Stadiumes = Repository.GetAllStadiums();
        
          //проверяем входящий параметр (код стадиона)
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
        
          //ищем указанный стадион
          ViewBag.Stadium = Repository.FindStadium((int)sid);
          if (ViewBag.Stadium == null)
          {
            logger.Warn("/MatchSetting/Index : Не найден указанный стадион. sid = " + sid.ToString());
            string msgKey = PrepareMessageBox("Не найден указанный стадион!", "Внимание!", true);
            return RedirectToAction("Index", "MatchSetting", new { notify = msgKey });
          }

          return View(Repository.Matches.Where(p => p.StadiumId == (int)sid).OrderBy(p => p.BeginsAt));
      }


        #region <Edit>
        
        public ActionResult Edit(int id)
        {

            Match match = Repository.FindMatch(id);

            if (match == null)
            {
                logger.Warn("/MatchSetting/Edit : Указанное мероприятие не найдено. id = " + id.ToString());
                string msgKey = PrepareMessageBox("Указанное мероприятие не найдено!", "Внимание!", true);
                return RedirectToAction("Index", "MatchSetting", new { notify = msgKey });
            }

            ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

            return PartialView(match);
        }

        [HttpPost]
        public ActionResult Edit(Match match)
        {
            match.Name = Utils.DeleteSpecialCharacters(match.Name);
          
            if (ModelState.IsValid)
            {
                try
                {
                    ((DbContext)Repository).Entry<Match>(match).State = EntityState.Modified;
                    Repository.SaveChanges();

                    string msg = string.Format(@"Мероприятие ""{0}"" успешно сохранено.", match.Name);
                    TempData["message"] = msg;
                    logger.Info(msg);
                    return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
                }
                catch (System.Exception ex)
                {
                    logger.Error("/MatchSetting/Edit : Ошибка", ex);
                    string msgKey = PrepareMessageBox("При сохранении мероприятия возникла ошибка!", "Внимание!", true);
                    return RedirectToAction("Index", "MatchSetting", new { notify = msgKey, sid = match.StadiumId });
                }
            }
            else
            {

                ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

                return View(match);

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

            Match match = new Match();
            match.StadiumId = id;

            return PartialView(match);
        }

        [HttpPost]
        public ActionResult Create(Match match)
        {
            match.Name = Utils.DeleteSpecialCharacters(match.Name);
          
            if (ModelState.IsValid)
            {
                try
                {
                    ((DbContext)Repository).Entry<Match>(match).State = EntityState.Added;
                    Repository.SaveChanges();

                    string msg = string.Format(@"Мероприятие ""{0}"" успешно создано.", match.Name);
                    TempData["message"] = msg;
                    logger.Info(msg);
                    return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
                }
                catch (System.Exception ex)
                {
                    logger.Error("/MatchSetting/Create : Ошибка", ex);
                    string msgKey = PrepareMessageBox("При создании мероприятия возникла ошибка!", "Внимание!", true);
                    return RedirectToAction("Index", "MatchSetting", new { notify = msgKey, sid = match.StadiumId });
                }
            }
            else
            {
                ViewBag.Stadium = Repository.FindStadium(match.StadiumId);

                return View(match);
            }
        }


        #endregion

        #region <Delete>
        

        public ActionResult Delete(int id)
        {
            Match match = Repository.FindMatch(id);

            if (match == null)
            {
                logger.Warn("/MatchSetting/Delete : Указанное мероприятие не найдено. id = " + id.ToString());
                string msgKey = PrepareMessageBox("Указанное мероприятие не найдено!", "Внимание!", true);
                return RedirectToAction("Index", "MatchSetting", new { notify = msgKey });
            }

            return PartialView(match);
        }



        [HttpPost]
        public ActionResult Delete(Match match)
        {
            try
            {
                ((DbContext)Repository).Entry<Match>(match).State = EntityState.Deleted;
                Repository.SaveChanges();

                string msg = string.Format(@"Мероприятие было удалено.", match.Name);
                TempData["message"] = msg;
                logger.Info(msg);
                return RedirectToAction("Index", "MatchSetting", new { sid = match.StadiumId });
            }
            catch (System.Exception ex)
            {
                logger.Error("/MatchSetting/Delete : Ошибка", ex);
                string msgKey = PrepareMessageBox("При удалении мероприятия возникла ошибка!", "Внимание!", true);
                return RedirectToAction("Index", "MatchSetting", new { notify = msgKey, sid = match.StadiumId });
            }
        }


        #endregion

    }
}
