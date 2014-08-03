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

          if (!ModelState.IsValid)
            return View(client);

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
            string msgKey = PrepareMessageBox("Переданы ошибочные данные!", "Внимание!", true);
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId, notify = msgKey });
          }

          try
          {
            //обрабатываем бронирование
            int clientId = Repository.ProcessTicketsReservation(client, places);

            //Создаём сообщение, отображающее код брони пользователю
            string messageKey = PrepareMessageBox(string.Format("Сообщите клиенту номер брони: {0} !", clientId), "Внимание!");

            //перенаправляем снова на страницу продажи
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId, notify = messageKey });
          }
          catch (System.Exception ex)
          {
            logger.Error("Ошибка при бронировнии!", ex);
            string msgKey = PrepareMessageBox("При бронировании возникла ошибка!\n" + ex.Message, "Внимание!", true);
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId, notify = msgKey });
          }
        }


        public ActionResult Edit(int id)
        {
          ReservationView reservationView = Repository.GetReservationInfo(id);
          if (reservationView == null)
          {
            logger.Warn("/Reservation/Edit : Указанный код брони не найден. id = " + id.ToString());

            string msgKey = PrepareMessageBox("Информация по текущей брони не найдена!", "Внимание!", true);
            return RedirectToAction("Index", "Reservation", new { notify = msgKey });
          }

          List<Place> placesList = Repository.GetPlacesByReservationId(reservationView.Id, true);
          reservationView.PlacesList = Repository.GetPlacesString(placesList);

          return PartialView(reservationView);
        }


        [HttpPost]
        public ActionResult SellReservation(ReservationView reservation)
        {
          if (ModelState.IsValid)
          {
            reservation = Repository.GetReservationInfo(reservation.Id);
            if(reservation == null)
            {
              string msgKey = PrepareMessageBox("Информация о данном бронировании отсутствует!", "Внимание!", true);
              return RedirectToAction("Index", "Reservation", new { notify = msgKey });
            }
            Repository.SellAllReservation(reservation);

            string messageKey = PrepareMessageBox("Забронированные билеты успешно проданы!", "Готово!", false);
            return RedirectToAction("Index", "Reservation", new { notify = messageKey });
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
            if (reservation == null)
            {
              string msgKey = PrepareMessageBox("Информация о данном бронировании отсутствует!", "Внимание!", true);
              return RedirectToAction("Index", "Reservation", new { notify = msgKey });
            }
            Repository.ReleaseAllReservation(reservation);
            
            string messageKey = PrepareMessageBox("Забронированные билеты успешно возвращены в свободную продажу!", "Готово!", false);
            return RedirectToAction("Index", "Reservation", new { notify = messageKey });
          }
          else
          {
            return View("Edit", reservation);
          }
        }
    }
}
