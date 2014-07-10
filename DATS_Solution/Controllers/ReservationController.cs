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


        public ActionResult Create()
        {
          return PartialView(new Client());
        }


        [HttpPost]
        public ActionResult Create(Client client)
        {
          if (ModelState.IsValid)
          {
            return RedirectToAction("Index", "Home");
          }
          else
          {
            return View(client);
          }
        }
    }
}
