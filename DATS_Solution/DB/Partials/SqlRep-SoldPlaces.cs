using System.Linq;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с информацией о проданных билетах
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Проданные места
    /// </summary>
    public DbSet<SoldPlace> SoldPlaces { get; set; }

    /// <summary>
    /// Количество проданных мест. Если reserver == true, то количество забронированных мест
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <param name="reserved"></param>
    /// <returns></returns>
    public int GetCountOfSoldPlaces(Match match, Sector sector, bool reserved)
    {
      var list = from sp in SoldPlaces
             join p in Places on sp.PlaceId equals p.Id 
             where p.SectorId == sector.Id 
             where sp.MatchId == match.Id
             where sp.IsReservation == reserved
             select sp;
      return list.Count<SoldPlace>();
    }

    /// <summary>
    /// Количество проданных мест у указанного сектора. Необходимо для проверки, были ли вообще проданы
    /// какие-либо места у указанного сектора.
    /// </summary>
    /// <param name="sector"></param>
    /// <returns></returns>
    public int GetCountOfSoldPlacesInSector(Sector sector)
    {
      var list = from sp in SoldPlaces
                 join p in Places on sp.PlaceId equals p.Id
                 where p.SectorId == sector.Id
                 select sp;
      return list.Count<SoldPlace>();
    }


    /// <summary>
    /// Возвращает список проданных мест на указанное мероприятие в указанном секторе. 
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public List<SoldPlace> GetSoldPlaces(Match match, Sector sector)
    {
      if (match == null) throw new ArgumentNullException("match");
      if (sector == null) throw new ArgumentNullException("sector");

      return (from sp in SoldPlaces
                 join p in Places on sp.PlaceId equals p.Id
                 where p.SectorId == sector.Id
                 where sp.MatchId == match.Id
                 select sp).ToList<SoldPlace>();
    }


    /// <summary>
    /// Осуществление продажи билетов
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public bool ProcessTicketsSelling(Match match, Sector sector, List<PlaceView> places)
    {
      if (match == null) throw new ArgumentNullException("match");
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (places.Count == 0) return false;

      //достаём места сектора
      List<Place> sectorPlaces = GetPlacesBySector(sector);
      Dictionary<PlaceView, Place> placesDict = new Dictionary<PlaceView, Place>();

      //проверка существования таких мест в секторе
      foreach (PlaceView pv in places)
      {
        bool exists = false;
        foreach (Place place in sectorPlaces)
          if (place.Column == pv.Col && place.Row == pv.Row)
          {
            exists = true;
            placesDict.Add(pv, place);
            break;
          }

        if (!exists) return false;
      }

      //достаём проданные билеты в этом секторе на это мероприятие
      List<SoldPlace> soldPlaces = GetSoldPlaces(match, sector);

      //проверяем, не были ли эти билеты уже проданы
      foreach (SoldPlace soldplace in soldPlaces)
        foreach (PlaceView pv in places)
        {
          if (soldplace.IsReservation) continue;
          Place place = Places.FirstOrDefault<Place>(p => p.Id == soldplace.PlaceId);
          if (place == null) return false;

          if (place.Column == pv.Col && place.Row == pv.Row)
            return false;
        }

      //сохранение новых данных
      foreach (PlaceView pv in places)
      {
        SoldPlace sp = new SoldPlace();
        sp.ClientId = null;
        sp.Date = DateTime.UtcNow;
        sp.IsReservation = false;
        sp.MatchId = match.Id;
        sp.PlaceId = placesDict[pv].Id;
        sp.SectorId = sector.Id;
        sp.Summ = 0;
        SoldPlaces.Add(sp);
      }
      SaveChanges();

      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public bool ProcessTicketsReturning(Match match, Sector sector, List<PlaceView> places)
    {
      if (match == null) throw new ArgumentNullException("match");
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (places.Count == 0) return false;

      //достаём проданные билеты в этом секторе на это мероприятие
      List<SoldPlace> soldPlaces = GetSoldPlaces(match, sector);
      List<SoldPlace> toRemove = new List<SoldPlace>();

      //ищем указанные билеты в проданных
      foreach (PlaceView pv in places)
      {
        bool exists = false;
        foreach (SoldPlace soldplace in soldPlaces)
        {
          Place place = Places.FirstOrDefault<Place>(p => p.Id == soldplace.PlaceId);
          if (place == null) return false;
             
          if (place.Column == pv.Col && place.Row == pv.Row)
          {
            exists = true;
            toRemove.Add(soldplace);
            break;
          }
        }

        if (!exists) return false;
      }

      //удаление лишних проданных мест
      foreach (SoldPlace soldplace in toRemove)
      {
        Entry<SoldPlace>(soldplace).State = EntityState.Deleted;
      }
      SaveChanges();

      return true;
    }
  }
}