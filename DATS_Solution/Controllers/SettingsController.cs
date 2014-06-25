using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS.Controllers
{
    public class SettingsController : BaseController
    {
        //
        // GET: /Settings/

        public ActionResult Index()
        {
            return RedirectToAction("Stadiums");
        }

        public ActionResult Stadiums()
        {
          return View();
        }

        public ActionResult Matches()
        {
          return View();
        }

        public ActionResult Sectors()
        {
          return View();
        }

        public ActionResult Prices()
        {
          return View();
        }
    }
}
