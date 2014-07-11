using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATS;

namespace DATS.Controllers
{
    public class PriceSettingController : BaseController
    {


        [ChildActionOnly]
        public ActionResult Index(int? sid, int? mid)
        {
            // переменные для сбора данных в ветвлениях
            int SID, MID;
            Stadium STADIUM;
            Match MATCH;

            ViewBag.Stadiumes = Repository.GetAllStadiums();

            if (sid == null)
            {
                // если sid нет, выбераем первый встретившийся стадион
                var FindFirstStadium = Repository.Stadiums.FirstOrDefault<Stadium>(z => z.Id == z.Id);

                if (FindFirstStadium == null)
                {
                    throw new ArgumentNullException("Ошибка! Отсутсвуют стадионы в справочнике стадионов.");
                }
                else
                {

                    // начало секции для mid

                    if (mid == null)
                    {
                        // если sid нет, выбераем первый встретившийся стадион
                        var FindFirstMatch = Repository.Matches.FirstOrDefault<Match>(z => z.Id == z.Id);

                        if (FindFirstMatch == null)
                        {
                            throw new ArgumentNullException("Ошибка! Отсутсвуют стадионы в справочнике стадионов.");
                        }
                        else
                        {
                            STADIUM = FindFirstStadium;
                            MATCH = FindFirstMatch;
                            SID = FindFirstStadium.Id;
                            MID = FindFirstMatch.Id;
                        }
                    }
                    else
                    {
                        STADIUM = FindFirstStadium;
                        MATCH = Repository.FindMatch((int)mid);
                        SID = FindFirstStadium.Id;
                        MID = (int)mid;
                    }

                    // конец сексии для mid

                }
            }
            else
            {

                // начало секции для mid

                if (mid == null)
                {
                    // если sid нет, выбераем первый встретившийся стадион
                    var FindFirstMatch = Repository.Matches.FirstOrDefault<Match>(z => z.Id == z.Id && z.StadiumId == (int)sid);

                    if (FindFirstMatch == null)
                    {
                        throw new ArgumentNullException("Ошибка! Отсутсвуют стадионы в справочнике стадионов.");
                    }
                    else
                    {
                        STADIUM = Repository.FindStadium((int)sid);
                        MATCH = FindFirstMatch;
                        SID = (int)sid;
                        MID = FindFirstMatch.Id;
                    }
                }
                else
                {
                    STADIUM = Repository.FindStadium((int)sid);
                    MATCH = Repository.FindMatch((int)mid);
                    SID = (int)sid;
                    MID = (int)mid;
                }

                // конец сексии для mid

            }


// Применяем собраные значения переменных SID и MID (как фильтры), STADIUM и MATCH (как выбор в фильтрах)

            ViewBag.Matchess = Repository.Matches.Where<Match>(m => m.StadiumId == SID).ToList<Match>();

            ViewBag.Stadium = STADIUM;
            ViewBag.Match = MATCH;

            var JoinSectorPrice = (from sk in Repository.Sectors
                                   where sk.StadiumId == SID
                                   select new PriceView()
                                   {
                                       StadiumId = sk.StadiumId,
                                       MatchId = (from pr in Repository.Prices
                                                  where (sk.Id == pr.SectorId) && (pr.MatchId == MID)
                                                  select pr.MatchId).Max(),
                                       Name = sk.Name,
                                       PriceValue = (from pr in Repository.Prices
                                                     where (sk.Id == pr.SectorId) && (pr.MatchId == MID)
                                                     select pr.PriceValue).Max()
                                   }).ToList<PriceView>();


            return PartialView(JoinSectorPrice);


        }

    }
}
