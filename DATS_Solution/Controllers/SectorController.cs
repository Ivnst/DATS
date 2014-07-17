﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DATS.Controllers
{
    public class SectorController : BaseController
    {
        //
        // GET: /Sector/
        
        #region <Selling tickets>
        /// <summary>
        /// Страница для продажи мест
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        public ActionResult Edit(int sid, int mid)
        {
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if (sector == null)
          {
            logger.Warn("/Sector/Edit : Не найден указанный сектор. sid = " + sid.ToString());
            return RedirectToAction("Index", "Home");
          }

          Match match = Repository.Matches.FirstOrDefault<Match>(m => m.Id == mid);
          if (match == null)
          {
            logger.Warn("/Sector/Edit : Не найдено указанное мероприятие. mid = " + mid.ToString());
            return RedirectToAction("Index", "Home");
          }

          FillViewBag(CurrentStadium, CurrentMatch);

          ViewBag.CurrentSector = sector;
          ViewBag.CurrentMatch = match;

          return View();
        }


        /// <summary>
        /// Возвращает информацию о проданных местах в указанном секторе
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult SectorSoldInfo(int sid, int mid)
        {
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if (sector == null)
          {
            logger.Warn("/Sector/SectorSoldInfo : Не найден указанный сектор. sid = " + sid.ToString());
            return Json(null, JsonRequestBehavior.AllowGet);
          }

          Match match = Repository.Matches.FirstOrDefault<Match>(m => m.Id == mid);
          if (match == null)
          {
            logger.Warn("/Sector/SectorSoldInfo : Не найдено указанное мероприятие. mid = " + mid.ToString());
            return Json(null, JsonRequestBehavior.AllowGet);
          }

          //достаём места необходимого сектора
          List<PlaceView> places = GetSoldPlacesInfo(match, sector);

          return Json(places, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Выполнение операции над билетами (продажа, возврат, бронь). ПЕРЕДЕЛАТЬ! (Временное решение)
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StoreSectorSoldInfo(int sid, int mid, string data)
        {
            //check sector id
            Sector sector = Repository.FindSector(sid);
            if (sector == null)
            {
              logger.Warn("/Sector/StoreSectorSoldInfo : Не найден указанный сектор. sid = " + sid.ToString());
              return Content("Указанный сектор не существует!");
            }

            //check match id
            Match match = Repository.FindMatch(mid);
            if (sector == null)
            {
              logger.Warn("/Sector/StoreSectorSoldInfo : Не найдено указанное мероприятие. mid = " + mid.ToString());
              return Content("Указанное мероприятие не существует!!");
            }

          //json to List
          List<PlaceView> places;
          try
          {
            
            places = JsonConvert.DeserializeObject<List<PlaceView>>(data);
            if (places.Count == 0)
            {
              logger.Warn("/Sector/StoreSectorSoldInfo : Не выбраны места для осуществления операции. mid = " + mid.ToString());
              return Content("Не выбраны места для осуществления операции!");
            }
          }
          catch (System.Exception ex)
          {
            logger.Error(ex);
            return Content("Получены некорректные данные!");
          }

          //проверка состояний переданных билетов
          int state = places[0].State;
          foreach (PlaceView pv in places)
          {
            if (pv.State != state)
            {
              logger.Warn("Выбраны билеты с разным статусом! Попробуйте ещё раз!");
              return Content("Выбраны билеты с разным статусом! Попробуйте ещё раз.");
            }
          }

          //выполнение операции
          try
          {
            if (state == (int)PlaceState.Sold)
            {
              Repository.ProcessTicketsSelling(match, sector, places);
            }

            if (state == (int)PlaceState.Free)
            {
              Repository.ProcessTicketsReturning(match, sector, places);
            }
          }
          catch (System.Exception ex)
          {
            logger.Error(data, ex);
            return Content("При сохранении данных возникла ошибка!\n" + ex.Message);
          }

          return Content("Операция выполнена успешно!");
        }
        #endregion

        #region <Configuring sector>
        /// <summary>
        /// Страница для редактирования мест в стадионе
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Configure(int sid)
        {
          Sector sector = Repository.FindSector(sid);
          if (sector == null)
          {
            logger.Warn("/Sector/Configure : Не найден указанный сектор. sid = " + sid.ToString());
            return RedirectToAction("Sectors", "Settings");
          }

          Stadium stadium = Repository.FindStadium(sector.StadiumId);

          ViewBag.CurrentSector = sector;
          ViewBag.CurrentStadium = stadium;

          return View();
        }


        /// <summary>
        /// Возвращает информацию о расположении мест в указанном секторе
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult SectorInfo(int sid)
        {
          Sector sector = Repository.FindSector(sid);
          if (sector == null)
          {
            logger.Warn("/Sector/SectorInfo : Не найден указанный сектор. sid = " + sid.ToString());
            return Json(null, JsonRequestBehavior.AllowGet);
          }

          //достаём места необходимого сектора
          List<Place> places = Repository.GetPlacesBySector(sector);

          //Переводим список объектов класса Place в список объектов класса PlaceView
          List<PlaceView> result = new List<PlaceView>(places.Count);
          foreach (Place place in places)
          {
            result.Add(new PlaceView(place));
          }

          return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Сохраняет новое расположение мест в указанном секторе.
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StoreSectorInfo(int sid, string data)
        {
          //проверка кода сектора
          Sector sector = Repository.FindSector(sid);
          if (sector == null)
          {
            logger.Warn("/Sector/SectorInfo : Не найден указанный сектор. sid = " + sid.ToString());
            return Content("Текущий редактируемый сектор не существует!");
          }


          //десериализация полученных данных
          List<PlaceView> places = new List<PlaceView>();
          try
          {
            places = JsonConvert.DeserializeObject<List<PlaceView>>(data);
          }
          catch (System.Exception ex)
          {
            logger.Error(data, ex);
            return Content("Получены некорректные данные! Ошибка десериализации!");
          }


          //сохранение расположения мест
          try
          {
            Repository.SavePlacesBySector(sector, places);
          }
          catch (System.Exception ex)
          {
            logger.Error(data, ex);
            return Content("Возникла ошибка при сохранении данных!\n" + ex.Message);
          }

          //успешное завершение
          logger.Debug(string.Format("Обновление расположение мест в секторе '{0}' ({1})", sector.Name, sector.Id));
          return Content("Данные успешно сохранены");
        }
        #endregion

        #region <Methods>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <param name="sector"></param>
        /// <returns></returns>
        private List<PlaceView> GetSoldPlacesInfo(Match match, Sector sector)
        {
          if (match == null) throw new ArgumentNullException("match");
          if (sector == null) throw new ArgumentNullException("sector");

          decimal defaultPrice = Repository.GetPrice(sector.Id, match.Id);

          //размерность сектора
          int maxRow = 0;
          int minRow = int.MaxValue;
          int maxCol = 0;
          int minCol = int.MaxValue;

          //достаём места текущего сектора
          List<Place> places = Repository.GetPlacesBySector(sector);
          if (places.Count == 0) return new List<PlaceView>();

          Dictionary<int, Place> placesDict = new Dictionary<int, Place>(); //id - place
          foreach (Place place in places)
          {
            if (place.Row < minRow) minRow = place.Row;
            if (place.Row > maxRow) maxRow = place.Row;
            if (place.Column < minCol) minCol = place.Column;
            if (place.Column > maxCol) maxCol = place.Column;
            placesDict.Add(place.Id, place);
          }

          //определяем ширину и высоту сектора
          int width = maxCol - minCol + 1;
          int height = maxRow - minRow + 1;

          //составляем матрицу мест
          PlaceView[][] placeMatrix = new PlaceView[height][];
          for (int i = 0; i < height; i++)
            placeMatrix[i] = new PlaceView[width];

          //Заполняем матрицу мест
          foreach (Place place in places)
          {
            PlaceView pv = new PlaceView(place);
            pv.State = (int)PlaceState.Free;
            pv.Price = defaultPrice;
            placeMatrix[place.Row - minRow][place.Column - minCol] = pv;
          }

          //Достаём информацию о проданных или забронированных местах
          List<SoldPlace> soldPlaces = Repository.GetSoldPlaces(match, sector);
          foreach (SoldPlace soldPlace in soldPlaces)
          {
            if (!placesDict.ContainsKey(soldPlace.PlaceId))
            {
              logger.Warn("GetSoldPlacesInfo : Продано место, отсуствющее в секторе!");
              continue;
            }

            Place place = placesDict[soldPlace.PlaceId];
            PlaceView pv = placeMatrix[place.Row - minRow][place.Column - minCol];
            pv.Price = soldPlace.Summ;
            pv.State = soldPlace.IsReservation ? (int)PlaceState.Reserved : (int)PlaceState.Sold;
          }

          //составляем результат
          List<PlaceView> result = new List<PlaceView>();
          for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
              if (placeMatrix[i][j] != null)
                result.Add(placeMatrix[i][j]);
            }

          return result;
        }
        #endregion
    }
}
