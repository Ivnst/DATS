using System;
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
          PlaceView[][] places = GetSoldPlacesInfo(match, sector);

          return Json(places, JsonRequestBehavior.AllowGet);
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
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if (sector == null)
          {
            logger.Warn("/Sector/Configure : Не найден указанный сектор. sid = " + sid.ToString());
            return RedirectToAction("Sectors", "Settings");
          }

          Stadium stadium = Repository.Stadiums.FirstOrDefault<Stadium>(s => s.Id == sector.StadiumId);

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
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if(sector == null)
          {
            logger.Warn("/Sector/SectorInfo : Не найден указанный сектор. sid = " + sid.ToString());
            return Json(null, JsonRequestBehavior.AllowGet);
          }

          //достаём места необходимого сектора
          List<Place> places = Repository.GetPlacesBySector(sector);

          //Переводим список объектов класса Place в список объектов класса PlaceView
          List<PlaceView> result = new List<PlaceView>();
          foreach (Place place in places)
          {
            result.Add(new PlaceView(place.Row, place.Location, place.Position));
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
          try
          {
            Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
            if (sector == null)
            {
              logger.Warn("/Sector/SectorInfo : Не найден указанный сектор. sid = " + sid.ToString());
              return Content("Текущий редактируемый сектор не существует!");
            }
            
            List<PlaceView> places = JsonConvert.DeserializeObject<List<PlaceView>>(data);
            bool res = Repository.SavePlacesBySector(sector, places);
            if (!res)
            {
              logger.Error(data, "Данные не были сохранены! Обратитесь к администратору!");
              return Content("Данные не были сохранены! Обратитесь к администратору!"); 
            }
            logger.Debug(string.Format("Обновление расположение мест в секторе '{0}' ({1})", sector.Name, sector.Id));
          }
          catch (System.Exception ex)
          {
            logger.Error(data, ex);
            return Content("Возникла ошибка при сохранении данных!");
          }

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
        private PlaceView[][] GetSoldPlacesInfo(Match match, Sector sector)
        {
          if (match == null) throw new ArgumentNullException("match");
          if (sector == null) throw new ArgumentNullException("sector");

          //размерность сектора
          int maxRow = 0;
          int minRow = int.MaxValue;
          int maxCol = 0;
          int minCol = int.MaxValue;

          //достаём места текущего сектора
          List<Place> places = Repository.GetPlacesBySector(sector);
          Dictionary<int, Place> placesDict = new Dictionary<int, Place>(); //id - place
          foreach (Place place in places)
          {
            if (place.Row < minRow) minRow = place.Row;
            if (place.Row > maxRow) maxRow = place.Row;
            if (place.Location < minCol) minCol = place.Location;
            if (place.Location > maxCol) maxCol = place.Location;
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
            PlaceView pv = new PlaceView(place.Row, place.Location, place.Position);
            pv.State = (int)PlaceState.Free;
            placeMatrix[place.Row - minRow][place.Location - minCol] = pv;
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
            PlaceView pv = placeMatrix[place.Row - minRow][place.Location - minCol];
            pv.State = soldPlace.IsReservation ? (int)PlaceState.Reserved : (int)PlaceState.Sold;
          }

          return placeMatrix;
        }
        #endregion
    }
}
