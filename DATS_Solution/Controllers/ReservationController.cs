using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Text;

namespace DATS.Controllers
{
    public class ReservationController : BaseController
    {
        //
        // GET: /Reservation/

        public ActionResult Index(string s)
        {
          if (CurrentMatch == null) return RedirectToAction("Error", "Home", new { e = 2 });
          FillViewBag(CurrentStadium, CurrentMatch);

          List<ReservationView> list = new List<ReservationView>();
          if (s == null)
          {
            list = Repository.GetReservationsList(CurrentMatch);
          }
          else
          {
            list = Repository.GetReservationsList(s);
            ViewBag.SearchString = s;
          }

          return View(list);
        }


        public ActionResult Create(int sid, int mid, string data)
        {
          ClientView client = new ClientView();
          client.MatchId = mid;
          client.SectorId = sid;
          client.Data = data;

          return PartialView(client);
        }


        [HttpPost]
        public ActionResult Create(ClientView client)
        {
          if (client.Contact == null) client.Contact = "";

          if (ModelState.IsValid)
          {
            //определяем места (в client.Data хранится ключ, по которому из кэша достаём json-данные, переданные туда заранее с помощью javscript)
            List<PlaceView> places = new List<PlaceView>();
            try
            {
              string jsonData = GetDataFromCache(client.Data);
              places = JsonConvert.DeserializeObject<List<PlaceView>>(jsonData);
            }
            catch (System.Exception ex)
            {
              logger.Error("Переданы ошибочные данные!", ex);
              throw new InvalidOperationException("Переданы ошибочные данные!");
            }

            //обрабатываем бронирование
            int clientId = Repository.ProcessTicketsReservation(client, places);

            //перенаправляем снова на страницу продажи
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId, cid = clientId });
          }
          else
          {
            return View(client);
          }
        }


        public ActionResult Edit(int id)
        {
          ReservationView reservationView = Repository.GetReservationInfo(id);
          if (reservationView == null)
          {
            logger.Warn("/Reservation/Edit : Указанный код брони не найден. id = " + id.ToString());
            return RedirectToAction("MessageBox", "Utils", new { header = "Внимание", message = "Информация по текущей брони не найдена!" });
          }

          reservationView.PlacesList = GetPlacesStringForReservation(reservationView.Id);

          return PartialView(reservationView);
        }

        [HttpPost]
        public ActionResult SellReservation(ReservationView reservation)
        {
          if (ModelState.IsValid)
          {
            reservation = Repository.GetReservationInfo(reservation.Id);
            Repository.SellAllReservation(reservation);
            return RedirectToAction("Index");
          }
          else
          {
            return View("Edit", reservation);
          }
        }


        [HttpPost]
        public ActionResult ReleaseReservation(ReservationView reservation)
        {
          if (ModelState.IsValid)
          {
            reservation = Repository.GetReservationInfo(reservation.Id);
            Repository.ReleaseAllReservation(reservation);
            return RedirectToAction("Index");
          }
          else
          {
            return View("Edit", reservation);
          }
        }


        #region <Private Methods>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        private string GetPlacesStringForReservation(int reservationId)
        {
          //находим все забронированные места по указаному коду брони
          List<Place> places = Repository.GetPlacesByReservationId(reservationId, true);

          List<int> rows = new List<int>();
          Dictionary<int, List<int>> placesInRow = new Dictionary<int, List<int>>();
          foreach (Place place in places)
          {
            if (!rows.Contains(place.Row))
              rows.Add(place.Row);
            if (!placesInRow.ContainsKey(place.Row))
              placesInRow.Add(place.Row, new List<int>());
            placesInRow[place.Row].Add(place.Column);
          }
          rows.Sort();

          //составляем строку
          StringBuilder result = new StringBuilder();
          foreach (int row in rows)
          {
            if (result.Length != 0)
              result.Append("\n");

            result.Append(string.Format("Ряд {0}: ", row));
            List<int> placesList = placesInRow[row];
            placesList.Sort();
            foreach (int col in placesList)
            {
              result.Append(col.ToString());
              result.Append(" ");
            }
          }

          return result.ToString();
        }
        #endregion

    }
}
