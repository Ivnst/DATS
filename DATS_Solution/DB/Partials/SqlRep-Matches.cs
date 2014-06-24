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
    /// Сохранение матча. Если Id матча равно 0, то создаётся новый матч, иначе - обновляется существующий.
    /// </summary>
    /// <param name="match"></param>
    public void SaveMatch(Match match)
    {


        if (match.Id == 0)
        {
            Matches.Add(match);
        }
        else
        {
            Match dbEntry = Matches.Find(match.Id);
            if (dbEntry != null)
            {

                dbEntry.StadiumId = match.StadiumId;
                dbEntry.Name = match.Name;
                dbEntry.BeginsAt = match.BeginsAt;
                dbEntry.Duration = match.Duration;
            }
        }
        SaveChanges();
    }

    /// <summary>
    /// Удаление указанного матча. Если Id матча равен 0 - ничего не происходит.
    /// </summary>
    /// <param name="match"></param>
    public void DeleteMatch(Match match)
    {

        if (match.Id == 0)
        {
            // нечего удалять
        }
        else
        {
            Match dbEntry = Matches.Find(match.Id);
            if (dbEntry != null)
            {
                Matches.Remove(dbEntry);
                SaveChanges();
            }
        }

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