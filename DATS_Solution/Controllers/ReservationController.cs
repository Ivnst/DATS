using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS.Controllers
{
    public class ReservationController : BaseController
    {
        //
        // GET: /Reservation/

        public ActionResult Index()
        {
          FillViewBag(CurrentStadium, CurrentMatch);
          
          return View();
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
            Repository.ProcessTicketsReservation(client);
            return RedirectToAction("Edit", "Sector", new { sid = client.SectorId, mid = client.MatchId });
          }
          else
          {
            return View(client);
          }
        }
    }
}
