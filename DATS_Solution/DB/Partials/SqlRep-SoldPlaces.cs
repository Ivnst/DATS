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
    /// Возвращает список проданных мест, забронированных указанным клиентом
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
    /// Возвращает список мест, забронированных указанным клиентом
    /// </summary>
    /// <param name="reservationId">Код клиента. Он же код брони</param>
    /// <param name="onlyReserved">Если истина - возвращаются только НЕ ПРОДАННЫЕ билеты. Если ложь - то и забронированные и проданные</param>
    /// <returns></returns>
    public List<Place> GetPlacesByReservationId(int reservationId, bool onlyReserved)
    {
      if (onlyReserved)
      {
        return (from sp in SoldPlaces
                join p in Places on sp.PlaceId equals p.Id
                where sp.ClientId == reservationId
                where sp.IsReservation == true
                select p).ToList<Place>();

      }
      else
      {
        return (from sp in SoldPlaces
                join p in Places on sp.PlaceId equals p.Id
                where sp.ClientId == reservationId
                select p).ToList<Place>();
      }
    }
    
    
    /// <summary>
    /// Осуществление продажи билетов
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public void ProcessTicketsSelling(Match match, Sector sector, List<PlaceView> places)
    {
      if (match == null) throw new ArgumentNullException("match");
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (places.Count == 0) throw new Exception("Места не выбраны!");

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
        {
          logger.Error(string.Format("Некоторые (или все) из указанных мест не найдены в секторе! sid = {0}, mid={1}", sector.Id, match.Id));
          throw new Exception("Некоторые (или все) из указанных мест не найдены в секторе!");
        }

        //проверяем, не были ли эти билеты уже проданы
        if (soldPlaces.ContainsKey(hash))
        {
          SoldPlace soldPlace = soldPlaces[hash];
          if (!soldPlace.IsReservation)
          {
            logger.Error("Некоторые из выбранных билетов уже проданы! Обновите страницу!");
            throw new Exception("Некоторые из выбранных билетов уже проданы! Обновите страницу!");
          }

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
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="match"></param>
    /// <param name="sector"></param>
    /// <returns></returns>
    public void ProcessTicketsReturning(Match match, Sector sector, List<PlaceView> places)
    {
      if (match == null) throw new ArgumentNullException("match");
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (places.Count == 0) throw new Exception("Места не выбраны!");

      //достаём проданные билеты в этом секторе на это мероприятие
      Dictionary<int, SoldPlace> soldPlaces = GetSoldPlacesDictionary(match, sector);
      List<SoldPlace> toRemove = new List<SoldPlace>();

      //ищем указанные билеты в проданных
      foreach (PlaceView pv in places)
      {
        if (!soldPlaces.ContainsKey(getPlaceHash(pv.Row, pv.Col)))
        {
          logger.Error("Некоторые из указанных билетов не являются проданными! Обновите страницу!");
          throw new Exception("Некоторые из указанных билетов не являются проданными! Обновите страницу!");
        }

        toRemove.Add(soldPlaces[getPlaceHash(pv.Row, pv.Col)]);
      }

      //удаление лишних проданных мест
      foreach (SoldPlace soldplace in toRemove)
      {
        Entry<SoldPlace>(soldplace).State = EntityState.Deleted;
      }
      SaveChanges();
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

      //ищем стадион
      Stadium stadium = this.FindStadium(sector.StadiumId);
      if (stadium == null) throw new InvalidOperationException("Указанный стадион не найден!");

      //достаём настройки стадиона
      ConfigView config = GetConfigView(stadium);

      //проверяем, можно ли бронировать
      if(Utils.GetNow() > match.BeginsAt.AddMinutes(-config.RemoveReservationPeriod))
      {
        throw new Exception(string.Format("Бронирование запрещено настройками (за {0} минут до начала матча)", config.RemoveReservationPeriod));
      }

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


    /// <summary>
    /// Возвращение всех забронированных билетов в свободную продажу по всем стадионам
    /// </summary>
    public void RemoveReservationsByTimeout()
    {
      try
      {
        //достаём все стадионы
        List<Stadium> stadiums = this.GetAllStadiums();

        //цикл по всем стадионам
        foreach (Stadium stadium in stadiums)
        {
          //достаём настройки по стадиону
          ConfigView configs = GetConfigView(stadium);

          //а также все мероприятия стадиона
          List<Match> matches = GetMatchesByStadium(stadium);

          //цикл по мероприятиям стадиона
          foreach (Match match in matches)
          {
            DateTime currentTime = Utils.GetNow();

            //если мероприятие уже закончилось - пропускаем
            if (match.BeginsAt < currentTime) continue;

            //определяем период времени до начала матча
            TimeSpan ts = TimeSpan.FromTicks(match.BeginsAt.Ticks - currentTime.Ticks);

            if (ts.TotalMinutes < configs.RemoveReservationPeriod)
            {
              //достаём информацию о брони по текущему мероприятию
              List<ReservationView> reservations = GetReservationsList(match);

              //цикл по клиентам, сделавших бронь
              foreach (ReservationView rv in reservations)
              {
                ReleaseAllReservation(rv);
              }
            }

          }

        }
      }
      catch (System.Exception ex)
      {
        logger.Error("Ошибка при автоматическом возвращении брони в свободную продажу!", ex);
      }

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