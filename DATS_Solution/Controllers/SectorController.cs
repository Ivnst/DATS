using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
