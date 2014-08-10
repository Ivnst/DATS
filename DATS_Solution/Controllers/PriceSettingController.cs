﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATS;
using System.Data.Entity;

namespace DATS.Controllers
{
  public class PriceSettingController : BaseController
  {


    public ActionResult Index(int? sid, int? mid)
    {
      FillUserDetail();
      ViewBag.Tab = 4;
      // переменные для сбора данных в ветвлениях
      int SID, MID;
      Stadium STADIUM;
      Match MATCH;

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

          // начало секции для mid

          if (mid == null)
          {
            // если mid нет, выбераем первое встретившееся мероприятие
            var FindFirstMatch = Repository.Matches.FirstOrDefault<Match>();

            if (FindFirstMatch == null)
            {
              // перехват ошибки
              ViewBag.Stadium = FindFirstStadium;
              ViewBag.Match = new Match();
              ViewBag.Matchess = Repository.Matches.Where<Match>(m => m.StadiumId == 0).OrderBy(m => m.BeginsAt).ToList<Match>();
              var JoinMoq1 = (from sk in Repository.Sectors
                              where sk.StadiumId == 0
                              select new PriceView()
                              {
                              }).ToList<PriceView>();
              return View(JoinMoq1);
            }
            else
            {
              STADIUM = FindFirstStadium;
              MATCH = FindFirstMatch;
              SID = FindFirstStadium.Id;
              MID = FindFirstMatch.Id;
            }
          }
          else
          {
            STADIUM = FindFirstStadium;
            MATCH = Repository.FindMatch((int)mid);
            SID = FindFirstStadium.Id;
            MID = (int)mid;
          }

          // конец сексии для mid

        }
      }
      else
      {

        // начало секции для mid

        if (mid == null)
        {
          // если mid нет, выбераем первое встретившееся мероприятие
          var FindFirstMatch = Repository.Matches.FirstOrDefault<Match>(z => z.Id == z.Id && z.StadiumId == (int)sid);

          if (FindFirstMatch == null)
          {
            // перехват ошибки
            ViewBag.Stadium = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == (int)sid);
            ViewBag.Match = new Match();
            ViewBag.Matchess = Repository.Matches.Where<Match>(m => m.StadiumId == 0).OrderBy(m => m.BeginsAt).ToList<Match>();
            var JoinMoq2 = (from sk in Repository.Sectors
                            where sk.StadiumId == 0
                            select new PriceView()
                            {
                            }).ToList<PriceView>();
            return View(JoinMoq2);
          }
          else
          {
            STADIUM = Repository.FindStadium((int)sid);
            MATCH = FindFirstMatch;
            SID = (int)sid;
            MID = FindFirstMatch.Id;
          }
        }
        else
        {
          STADIUM = Repository.FindStadium((int)sid);
          MATCH = Repository.FindMatch((int)mid);
          SID = (int)sid;
          MID = (int)mid;
        }

        // конец сексии для mid

      }


      // Применяем собраные значения переменных SID и MID (как фильтры), STADIUM и MATCH (как выбор в фильтрах)

      ViewBag.Matchess = Repository.Matches.Where<Match>(m => m.StadiumId == SID).OrderBy(m => m.BeginsAt).ToList<Match>();

      ViewBag.Stadium = STADIUM;
      ViewBag.Match = MATCH;

      var JoinSectorPrice = (from sk in Repository.Sectors
                             where sk.StadiumId == SID
                             select new PriceView()
                             {
                               StadiumId = sk.StadiumId,
                               MatchId = MID,
                               SectorId = sk.Id,
                               Name = sk.Name,
                               PriceValue = (from pr in Repository.Prices
                                             where (sk.Id == pr.SectorId) && (pr.MatchId == MID)
                                             select pr.PriceValue).Max()
                             }).ToList<PriceView>();


      return View(JoinSectorPrice);


    }


    #region <Save>

    [HttpPost]
    public ActionResult Save(FormCollection formCollection)
    {

      int? SID = null, MID = null;
      string[] valueStadiumId = null, valueMatchId = null, valueSectorId = null, valuePrice = null;
      int TempMatchId = 0, TempSectorId = 0, LengthMatchId = 0;
      decimal TempPriceValue = 0;
      Price FindFirstPrice = null;

      try
      {
        if (Request.IsAjaxRequest())
        {
          foreach (var key in formCollection.AllKeys)
          {
            if ((string)key == "item.StadiumId")
            {
              valueStadiumId = formCollection[key].Split(new Char[] { ',' });
              SID = Int32.Parse(valueStadiumId[0]);
            }
            if ((string)key == "item.MatchId")
            {
              valueMatchId = formCollection[key].Split(new Char[] { ',' });
              MID = Int32.Parse(valueMatchId[0]);
              LengthMatchId = valueMatchId.Length;
            }
            if ((string)key == "item.SectorId")
            {
              valueSectorId = formCollection[key].Split(new Char[] { ',' });
            }
            if ((string)key == "item.PriceValue")
            {
              valuePrice = formCollection[key].Split(new Char[] { ',' });
            }
          }

          if (SID != null && MID != null)
          {
            // создаём, модифицируем или удаляем данные
            for (var i = 0; i < LengthMatchId; i++)
            {

              if ((valueSectorId[i].Length > 0) && (valuePrice[i].Length > 0))
              {
                TempMatchId = (int)MID;
                TempSectorId = Int32.Parse(valueSectorId[i]);
                TempPriceValue = decimal.Parse(valuePrice[i].Replace(".", ","));
                FindFirstPrice = Repository.Prices.FirstOrDefault<Price>(z => (z.MatchId == TempMatchId) && (z.SectorId == TempSectorId));

                if (FindFirstPrice != null)
                {
                  FindFirstPrice.PriceValue = TempPriceValue;
                  ((DbContext)Repository).Entry<Price>(FindFirstPrice).State = EntityState.Modified;
                  Repository.SaveChanges();
                }
                else
                {
                  FindFirstPrice = new Price { MatchId = TempMatchId, SectorId = TempSectorId, PriceValue = TempPriceValue };
                  ((DbContext)Repository).Entry<Price>(FindFirstPrice).State = EntityState.Added;
                  Repository.SaveChanges();
                }
              }
              else if ((valueSectorId[i].Length > 0) && (valuePrice[i].Length == 0))
              {
                TempMatchId = (int)MID;
                TempSectorId = Int32.Parse(valueSectorId[i]);
                FindFirstPrice = Repository.Prices.FirstOrDefault<Price>(z => (z.MatchId == TempMatchId) && (z.SectorId == TempSectorId));

                if (FindFirstPrice != null)
                {
                  FindFirstPrice.PriceValue = 0;
                  ((DbContext)Repository).Entry<Price>(FindFirstPrice).State = EntityState.Modified;
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

      }
      catch (System.Exception ex)
      {
        logger.Error("/PriceSetting/Delete : Ошибка", ex);
        string msgKey = PrepareMessageBox("При сохранении цен возникла ошибка!", "Внимание!", true);
        return RedirectToAction("Index", "PriceSetting", new { notify = msgKey });
      }

      return RedirectToAction("Index");
    }

    #endregion

  }
}
