using System.Linq;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

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
    /// Возвращает список проданных мест на указанное мероприятие в указанном секторе. 
    /// </summary>
    /// <param name="reservationId">Код клиента. Он же код брони</param>
    /// <param name="onlyReserved">Если истина - возвращаются только НЕ ПРОДАННЫЕ билеты. Если ложь - то и забронированные и проданные</param>
    /// <returns></returns>
    public List<SoldPlace> GetSoldPlacesByReservationId(int reservationId, bool onlyReserved)
    {
      if (onlyReserved)
      {
        return (from sp in SoldPlaces
                join p in Places on sp.PlaceId equals p.Id
                where sp.ClientId == reservationId
                where sp.IsReservation == true
                select sp).ToList<SoldPlace>();

      }
      else
      {
        return (from sp in SoldPlaces
                join p in Places on sp.PlaceId equals p.Id
                where sp.ClientId == reservationId
                select sp).ToList<SoldPlace>();
      }
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
      Dictionary<int, Place> sectorPlaces = GetPlacesDictionary(sector);

      //достаём проданные билеты в этом секторе на это мероприятие
      Dictionary<int, SoldPlace> soldPlaces = GetSoldPlacesDictionary(match, sector);

      List<SoldPlace> newItems = new List<SoldPlace>();

      for(int i =0; i < places.Count; i++)
      {
        PlaceView pv = places[i];
        int hash = getPlaceHash(pv.Row, pv.Col);

        //проверка существования таких мест в секторе
        if (!sectorPlaces.ContainsKey(hash))
          return false;

        //проверяем, не были ли эти билеты уже проданы
        if (soldPlaces.ContainsKey(hash))
        {
          SoldPlace soldPlace = soldPlaces[hash];
          if (!soldPlace.IsReservation) return false;

          //снимаем бронь
          soldPlace.IsReservation = false;

          //добавляем в список купленных 
          newItems.Add(soldPlaces[hash]);

          //удаляем из списка
          places.RemoveAt(i); i--;
        }
      }

      //сохранение новых данных
      foreach (PlaceView pv in places)
      {
        SoldPlace sp = new SoldPlace();
        sp.ClientId = null;
        sp.Date = DateTime.UtcNow;
        sp.IsReservation = false;
        sp.MatchId = match.Id;
        sp.PlaceId = sectorPlaces[getPlaceHash(pv.Row, pv.Col)].Id;
        sp.SectorId = sector.Id;
        sp.Summ = pv.Price;
        newItems.Add(sp);
      }

      //обработка изменений
      foreach (SoldPlace soldPlace in newItems)
      {
        if (soldPlace.Id > 0)
        {
          Entry<SoldPlace>(soldPlace).State = EntityState.Modified;
        }
        else
        {
          SoldPlaces.Add(soldPlace);
        }
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
      Dictionary<int, SoldPlace> soldPlaces = GetSoldPlacesDictionary(match, sector);
      List<SoldPlace> toRemove = new List<SoldPlace>();

      //ищем указанные билеты в проданных
      foreach (PlaceView pv in places)
      {
        if (!soldPlaces.ContainsKey(getPlaceHash(pv.Row, pv.Col)))
          return false;

        toRemove.Add(soldPlaces[getPlaceHash(pv.Row, pv.Col)]);
      }

      //удаление лишних проданных мест
      foreach (SoldPlace soldplace in toRemove)
      {
        Entry<SoldPlace>(soldplace).State = EntityState.Deleted;
      }
      SaveChanges();

      return true;
    }


    /// <summary>
    /// Осуществление бронирования билетов
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public int ProcessTicketsReservation(ClientView clientView, List<PlaceView> places)
    {
      if (clientView == null) throw new ArgumentNullException("clientView");
      if (clientView.Data == null) throw new ArgumentNullException("clientView.Data");

      //создаём клиента
      Client client = new Client();
      client.Name = clientView.Name;
      client.IsActive = true;
      client.Date = DateTime.UtcNow;
      client.Contact = clientView.Contact;

      //ищем сектор
      Sector sector = this.FindSector(clientView.SectorId);
      if (sector == null) throw new InvalidOperationException("Указанный сектор не найден!");

      //ищем мероприятие
      Match match = this.FindMatch(clientView.MatchId);
      if (match == null) throw new InvalidOperationException("Указанное мероприятие не найдено!");

      //достаём места сектора
      Dictionary<int, Place> sectorPlaces = GetPlacesDictionary(sector);

      //достаём проданные билеты в этом секторе на это мероприятие
      Dictionary<int, SoldPlace> soldPlaces = GetSoldPlacesDictionary(match, sector);
      
      foreach (PlaceView pv in places)
      {
        //проверка существования таких мест в секторе
        if (!sectorPlaces.ContainsKey(getPlaceHash(pv.Row, pv.Col)))
        {
          throw new InvalidOperationException("Указанные места отсутствуют в секторе!");
        }

        //проверяем, не были ли эти билеты уже проданы
        if (soldPlaces.ContainsKey(getPlaceHash(pv.Row, pv.Col)))
        {
          throw new InvalidOperationException("Указанные места уже проданы!");
        }
      }

      Clients.Add(client);
      SaveChanges();

      //сохранение новых данных
      foreach (PlaceView pv in places)
      {
        SoldPlace sp = new SoldPlace();
        sp.ClientId = client.Id;
        sp.Date = DateTime.UtcNow;
        sp.IsReservation = true;
        sp.MatchId = match.Id;
        sp.PlaceId = sectorPlaces[getPlaceHash(pv.Row, pv.Col)].Id;
        sp.SectorId = sector.Id;
        sp.Summ = pv.Price;
        SoldPlaces.Add(sp);
      }
      SaveChanges();

      return client.Id;
    }


    #region <Private methods>

    /// <summary>
    /// Возвращает словарь [int, Place], где int высчитывается по формуле Row*1000 + Column. 
    /// Необходимо для более быстрого поиска
    /// </summary>
    /// <param name="sector"></param>
    /// <returns></returns>
    private Dictionary<int, Place> GetPlacesDictionary(Sector sector)
    {
      List<Place> sectorPlaces = GetPlacesBySector(sector);
      Dictionary<int, Place> result = new Dictionary<int, Place>();
      
      foreach (Place place in sectorPlaces)
      {
        int hash = getPlaceHash(place.Row, place.Column);
        if (!result.ContainsKey(hash))
          result.Add(hash, place);
      }

      return result;
    }


    /// <summary>
    /// Возвращает словарь [int, SoldPlace], где int высчитывается по формуле Row*1000 + Column. 
    /// Необходимо для более быстрого поиска
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    private Dictionary<int, SoldPlace> GetSoldPlacesDictionary(Match match, Sector sector)
    {
      //достаём места в секторе и проданные места
      List<SoldPlace> soldPlaces = GetSoldPlaces(match, sector);
      Dictionary<int, Place> placesDict =  GetPlacesBySector(sector).ToDictionary(x => x.Id);

      Dictionary<int, SoldPlace> result = new Dictionary<int, SoldPlace>();
      foreach (SoldPlace soldPlace in soldPlaces)
      {
        Place place = placesDict[soldPlace.PlaceId];
        int hash = getPlaceHash(place.Row, place.Column);
        if (!result.ContainsKey(hash))
          result.Add(hash, soldPlace);
      }

      return result;
    }


    /// <summary>
    /// Возвращает хэш места, равный row * 1000 + column. Для создания хэш словарей для быстрого поиска.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private int getPlaceHash(int row, int column)
    {
      return row * 1000 + column;
    }

    #endregion
  }
}