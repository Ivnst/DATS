using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DATS.Controllers
{
    public class SectorSettingController : BaseController
    {
        //
        // GET: /SectorSetting/

        [ChildActionOnly]
        public ActionResult Index(int? sid)
        {
            ViewBag.Stadiumes = Repository.GetAllStadiums();

            if (sid == null)
            {
                int CookieId = ReadIntValueFromCookie("CurrSadiumForSector");
                if (CookieId != -1)
                {
                    sid = CookieId;
                }
            }

            if (sid == null)
            {
                ViewBag.ChooseStadium = "Фильтровать по стадионам";
                return PartialView(Repository.Sectors);
            }
            else
            {
                ViewBag.ChooseStadium = Repository.Stadiums.Where(s => s.Id == sid).Distinct().Select(k => k.Name).Max();
                WriteIntValueIntoCookie("CurrSadiumForSector", (int)sid);
                return PartialView(Repository.Sectors.Where(p => p.StadiumId == sid));
            }

        }


    }
}
