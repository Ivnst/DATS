using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с клиентами, которые забронировали билеты
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Клиенты
    /// </summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>
    /// Пример метода, добавляющего нового клиента в базу данных
    /// </summary>
    /// <param name="client"></param>
    public void AddClient(Client client)
    {
      Clients.Add(client);
      base.SaveChanges();
    }

    /// <summary>
    /// Возвращает список бронирований по указанному мероприятию
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public List<ReservationView> GetReservationsList(Match match)
    {
//       var list = from sp in SoldPlaces
//                  join c in Clients on sp.ClientId equals c.Id
//                  join s in Sectors on sp.SectorId equals s.Id
//                  where sp.MatchId == match.Id
//                  where sp.IsReservation == true
//                  select new ReservationView()
//                  {
//                    Id = c.Id,
//                    Name = c.Name,
//                    Contact = c.Contact,
//                    SectorId = s.Id,
//                    SectorName= s.Name,
//                    Count = g.Average(order => order.TotalDue)
//                  };

      string sql = @"SELECT c.Id, c.Name, c.Contact, 
                  t.Id as StadiumId, t.Name as StadiumName, 
                  m.Id as MatchId,   m.Name as MatchName, 
                  sp.SectorId,       s.Name as SectorName, 
                  COUNT(sp.Id) as [Count], sp.Summ as Price, (COUNT(sp.Id) * sp.Summ) as Summ, c.Date as ReservationDate
                    FROM [SoldPlaces] as sp
                    inner join [Clients] as c on c.Id = sp.ClientId
                    inner join [Sectors] as s on s.Id = sp.SectorId
                    inner join [Stadiums] as t on t.Id = s.StadiumId
                    inner join [Matches]  as m on m.Id = sp.MatchId
                    where sp.IsReservation = 1
                      and sp.MatchId = @p0
                    group by c.Id, c.Name, c.Contact, t.Id, t.Name, m.Id, m.Name, sp.SectorId, s.Name, sp.Summ,c.Date";

      List<ReservationView> result = this.Database.SqlQuery<ReservationView>(sql, match.Id).ToList();

      //сдвигаем дату на текущий часовой пояс
      foreach (ReservationView rv in result)
      {
        rv.ReservationDate = Utils.AdjustDate(rv.ReservationDate);
      }

      return result;
    }


    /// <summary>
    /// Возвращает список бронирований, удовлетворяющих заданной строке поиска
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public List<ReservationView> GetReservationsList(string searchString)
    {
      int clientId;
      bool isNumeric = Int32.TryParse(searchString, out clientId);

      string sql = @"SELECT c.Id, c.Name, c.Contact,
                  t.Id as StadiumId, t.Name as StadiumName, 
                  m.Id as MatchId,   m.Name as MatchName, 
                  sp.SectorId,       s.Name as SectorName, 
                  COUNT(sp.Id) as [Count], sp.Summ as Price, (COUNT(sp.Id) * sp.Summ) as Summ, c.Date as ReservationDate
                    FROM [SoldPlaces] as sp
                    inner join [Clients]  as c on c.Id = sp.ClientId
                    inner join [Sectors]  as s on s.Id = sp.SectorId
                    inner join [Stadiums] as t on t.Id = s.StadiumId
                    inner join [Matches]  as m on m.Id = sp.MatchId
                    where sp.IsReservation = 1
                      and ( " + ((isNumeric) ? "c.Id = @p0 or " : "")
                          + @"c.Name like '%' + @p0 + '%' 
                          or c.Contact like '%' + @p0 + '%' )
                      and m.BeginsAt > @p1
                    group by c.Id, c.Name, c.Contact, t.Id, t.Name, m.Id, m.Name, sp.SectorId, s.Name, sp.Summ,c.Date";

      DateTime now = Utils.GetNow();
      List<ReservationView> result = this.Database.SqlQuery<ReservationView>(sql, searchString, now).ToList();

      //сдвигаем дату на текущий часовой пояс
      foreach (ReservationView rv in result)
      {
        rv.ReservationDate = Utils.AdjustDate(rv.ReservationDate);
      }

      return result;
    }


    /// <summary>
    /// Возвращает информацию по бронированию, по указанному номеру брони
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public ReservationView GetReservationInfo(int Id)
    {
      string sql = @"SELECT c.Id, c.Name, c.Contact, 
                  t.Id as StadiumId, t.Name as StadiumName, 
                  m.Id as MatchId,   m.Name as MatchName, 
                  sp.SectorId,       s.Name as SectorName, 
                  COUNT(sp.Id) as [Count], sp.Summ as Price, (COUNT(sp.Id) * sp.Summ) as Summ, c.Date as ReservationDate
                    FROM [SoldPlaces] as sp
                    inner join [Clients]  as c on c.Id = sp.ClientId
                    inner join [Sectors]  as s on s.Id = sp.SectorId
                    inner join [Stadiums] as t on t.Id = s.StadiumId
                    inner join [Matches]  as m on m.Id = sp.MatchId
                    where sp.IsReservation = 1
                      and c.Id = @p0
                    group by c.Id, c.Name, c.Contact, t.Id, t.Name, m.Id, m.Name, sp.SectorId, s.Name, sp.Summ,c.Date";

      ReservationView result = this.Database.SqlQuery<ReservationView>(sql, Id).FirstOrDefault();
      
      if (result != null)
        result.ReservationDate = Utils.AdjustDate(result.ReservationDate);

      return result;
    }


    /// <summary>
    /// Продать все места в брони
    /// </summary>
    /// <param name="reservation"></param>
    public void SellAllReservation(ReservationView reservation)
    {
      List<SoldPlace> soldPlaces = GetSoldPlacesByReservationId(reservation.Id, true);

      foreach (SoldPlace sp in soldPlaces)
      {
        sp.IsReservation = false;
      }

      SaveChanges();
    }


    /// <summary>
    /// Снять бронь со всех мест в указанной брони
    /// </summary>
    /// <param name="reservation"></param>
    public void ReleaseAllReservation(ReservationView reservation)
    {
      List<SoldPlace> soldPlaces = GetSoldPlacesByReservationId(reservation.Id, true);

      foreach (SoldPlace sp in soldPlaces)
      {
        Entry<SoldPlace>(sp).State = EntityState.Deleted;
      }

      SaveChanges();
    }
  }
}