using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using System.Data.Entity.Core.Objects;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с мероприятиями
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список мероприятий
    /// </summary>
    public DbSet<Match> Matches { get; set; }


    /// <summary>
    /// Возвращает мероприятие с указанным кодом или null, если такое мероприятие не найдено.
    /// </summary>
    /// <param name="mid"></param>
    /// <returns></returns>
    public Match FindMatch(int mid)
    {
      return Matches.FirstOrDefault<Match>(m => m.Id == mid);
    }


    /// <summary>
    /// Возвращает список мероприятий у указанного стадиона
    /// </summary>
    /// <param name="stadium"></param>
    /// <returns></returns>
    public List<Match> GetMatchesByStadium(Stadium stadium)
    {
      return Matches.Where<Match>(m => m.StadiumId == stadium.Id && DbFunctions.AddMinutes(m.BeginsAt, m.Duration) > DateTime.Now).ToList<Match>();
    }

    /// <summary>
    /// Статистика по секторам на указанном мероприятии
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public List<SectorView> GetSectorsStatistics(Match match)
    {
      List<SectorView> result = new List<SectorView>();
      if (match == null) return result;

      foreach (Sector sector in GetSectorsByStadium(match.StadiumId))
      {
        SectorView item = new SectorView();
        item.SectorId = sector.Id;
        item.Name = sector.Name;
        item.TotalPlaces = Places.Count<Place>(p => p.SectorId == sector.Id);
        item.SoldPlaces = GetCountOfSoldPlaces(match, sector, false);
        item.ReservedPlaces = GetCountOfSoldPlaces(match, sector, true);
        item.FreePlaces = item.TotalPlaces - item.SoldPlaces - item.ReservedPlaces;
       
        result.Add(item);
      }

      return result;
    }

  }
}