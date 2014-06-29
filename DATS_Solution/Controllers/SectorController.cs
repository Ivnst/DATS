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
        
        /// <summary>
        /// Страница для продажи мест
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Edit(int sid)
        {
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if (sector == null)
          {
            return RedirectToAction("Sectors", "Settings");
          }
          ViewBag.CurrentSector = sector;

          FillViewBag(CurrentStadium, CurrentMatch);

          return View();
        }

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
            return RedirectToAction("Sectors", "Settings");
          }

          Stadium stadium = Repository.Stadiums.FirstOrDefault<Stadium>(s => s.Id == sector.StadiumId);

          ViewBag.CurrentSector = sector;
          ViewBag.CurrentStadium = stadium;

          return View();
        }


        /// <summary>
        /// Страница для редактирования мест в стадионе
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult SectorInfo(int sid)
        {
          Sector sector = Repository.Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
          if(sector == null)
          {
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
        /// 
        /// </summary>
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
              return Content("Данная форма содержит некорректные данные!");
            }
            
            List<PlaceView> places = JsonConvert.DeserializeObject<List<PlaceView>>(data);
            bool res = Repository.SavePlacesBySector(sector, places);
            if (!res)
            {
              logger.Error(data, "Данные не были сохранены! Обратитесь к администратору!");
              return Content("Данные не были сохранены! Обратитесь к администратору!"); 
            }
          }
          catch (System.Exception ex)
          {
            logger.Error(data, ex);
            return Content("Возникла ошибка при сохранении данных!");
          }

          return Content("Данные успешно сохранены");
        }
    }
}
