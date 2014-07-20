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
            ViewBag.Tab = 5;
            ViewBag.Stadiumes = Repository.GetAllStadiums();

            // переменные для сбора данных в ветвлениях
            int? SID;
            Stadium STADIUM;

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
                    STADIUM = FindFirstStadium;
                    SID = FindFirstStadium.Id;
                }
            }
            else
            {
                STADIUM = Repository.FindStadium((int)sid);
                SID = sid;
            }

            // Применяем собраные значения переменных SID (как фильтры) и STADIUM (как выбор в фильтре)

            ViewBag.Stadium = STADIUM;

            var rrperiodCount = Repository.Configs.Where(cf => cf.StadiumId == SID && cf.Name == "rrperiod").Count();

            if (rrperiodCount == 0)
            {
                Config config = new Config()
                {
                    StadiumId = SID,
                    Name = "rrperiod",
                    Val = "30"
                };
                Repository.Configs.Add(config);
                Repository.SaveChanges();
            }


            var confView = (from cf in Repository.Configs.AsEnumerable()
                                   where cf.StadiumId == SID && cf.Name == "rrperiod"
                                   select new ConfigView()
                                   {
                                       StadiumId = (int)SID,
                                       RemoveReservationPeriod = int.Parse(cf.Val)
                                   }).ToList<ConfigView>();


            return View(confView);

        }


        [HttpPost]
        public ActionResult Save(FormCollection formCollection)
        {

            int? SID = null;
            string[] valueStadiumId = null, valueTime = null;
            int LengthCount = 0;
            Config FindFirstTime = null;

            if (Request.IsAjaxRequest())
            {


                foreach (var key in formCollection.AllKeys)
                {
                    if ((string)key == "item.StadiumId")
                    {
                        valueStadiumId = formCollection[key].Split(new Char[] { ',' });
                        SID = Int32.Parse(valueStadiumId[0]);
                        LengthCount = valueStadiumId.Length;
                    }
                    if ((string)key == "item.RemoveReservationPeriod")
                    {
                        valueTime = formCollection[key].Split(new Char[] { ',' });
                    }
                }

                if (SID != null)
                {
                    // создаём, модифицируем или удаляем данные
                    for (var i = 0; i < LengthCount; i++)
                    {

                        if (valueTime[i].Length > 0)
                        {
                             FindFirstTime = Repository.Configs.FirstOrDefault<Config>(z => (z.StadiumId == SID) && (z.Name == "rrperiod"));

                            if (FindFirstTime != null)
                            {
                                FindFirstTime.Val = valueTime[i];
                                ((DbContext)Repository).Entry<Config>(FindFirstTime).State = EntityState.Modified;
                                Repository.SaveChanges();
                            }
                         }
                        else if (valueTime[i].Length == 0)
                        {
                            FindFirstTime = Repository.Configs.FirstOrDefault<Config>(z => (z.StadiumId == SID) && (z.Name == "rrperiod"));

                            if (FindFirstTime != null)
                            {
                                FindFirstTime.Val = "0";
                                ((DbContext)Repository).Entry<Config>(FindFirstTime).State = EntityState.Modified;
                                Repository.SaveChanges();
                            }
                        }
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
            return RedirectToAction("Index");
        }










           
    }
}
