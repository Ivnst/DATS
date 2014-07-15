using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.Entity;

namespace DATS.Controllers
{
    public class ReservationController : BaseController
    {
        //
        // GET: /Reservation/

        public ActionResult Index()
        {
          if (CurrentMatch == null) return RedirectToAction("Error", "Home", new { e = 2 });
          FillViewBag(CurrentStadium, CurrentMatch);

          List<ReservationView> list = Repository.GetReservationsList(CurrentMatch);

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
            Repository.ProcessTicketsReservation(client, places);

            //перенаправляем снова на страницу продажи
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId });
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

    }
}
