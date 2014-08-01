using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DATS;

namespace DATS.Controllers
{
    public class ConfigSettingController : BaseController
    {


        public ActionResult Index(int? sid)
        {
            FillUserDetail();
            ViewBag.Tab = 5;
            ViewBag.Stadiumes = Repository.GetAllStadiums();

            // переменные для сбора данных в ветвлениях
            Stadium STADIUM;

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
                    STADIUM = FindFirstStadium;
                }
            }
            else
            {
                STADIUM = Repository.FindStadium((int)sid);
                if(STADIUM == null)
                {
                    logger.Warn("/ConfigSetting/Index : Не найден указанный стадион. sid = " + sid.ToString());
                    string msgKey = PrepareMessageBox("Не найден указанный стадион!", "Внимание!", true);
                    return RedirectToAction("Index", "ConfigSetting", new { notify = msgKey });
                }
            }

            // Применяем собраные значения переменных SID (как фильтры) и STADIUM (как выбор в фильтре)

            ViewBag.Stadium = STADIUM;

            ConfigView confView = Repository.GetConfigView(STADIUM);

            List<ConfigView> listConfig = new List<ConfigView>();
            listConfig.Add(confView);

            return View(listConfig);

        }


        [HttpPost]
        public ActionResult Save(FormCollection formCollection)
        {
            int? SID = null;
            int RRPeriod;
            string valueStadiumId = null, valueTime = null;
 
            try
            { 

            if (Request.IsAjaxRequest())
            {

                foreach (var key in formCollection.AllKeys)
                {
                    if ((string)key == "item.StadiumId")
                    {
                        valueStadiumId = formCollection[key];
                        SID = Int32.Parse(valueStadiumId);
                    }
                    if ((string)key == "item.RemoveReservationPeriod")
                    {
                        valueTime = formCollection[key];
                    }
                }

                if (SID != null)
                {
                    // создаём, модифицируем или удаляем данные


                        if (valueTime.Length > 0)
                        {
                            RRPeriod = Convert.ToInt32(valueTime);
                            Repository.SetConfigView(new ConfigView { StadiumId = (int)SID, RemoveReservationPeriod = RRPeriod });
                         }
                        else if (valueTime.Length == 0)
                        {
                            Repository.SetConfigView(new ConfigView { StadiumId = (int)SID, RemoveReservationPeriod = 0 });
                        }


                    // перенаправление на Index после завершения
                    TempData["message"] = "Данные успешно сохранены.";
                    logger.Info("Данные успешно сохранены.");
                    return Json("Success");
                }
                else
                {
                    TempData["message"] = "Ошибка извлечения данных!";
                    logger.Info("Ошибка извлечения данных!");
                    return Json("An Error Has occoured");
                }
            }

            }
            catch (System.Exception ex)
            {
                logger.Error("/ConfigSetting/Delete : Ошибка", ex);
                string msgKey = PrepareMessageBox("При сохранении параметров возникла ошибка!", "Внимание!", true);
                return RedirectToAction("Index", "ConfigSetting", new { notify = msgKey });
            }

            return RedirectToAction("Index");
        }










           
    }
}
