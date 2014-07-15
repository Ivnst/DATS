using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

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

      string sql = @"SELECT c.Id, c.Name, c.Contact, sp.SectorId, s.Name as SectorName, 
                  COUNT(sp.Id) as [Count], sp.Summ as Price, (COUNT(sp.Id) * sp.Summ) as Summ, c.Date as ReservationDate
                    FROM [SoldPlaces] as sp
                    inner join [Clients] as c on c.Id = sp.ClientId
                    inner join [Sectors] as s on s.Id = sp.SectorId
                    where sp.IsReservation = 1
                      and sp.MatchId = @p0
                    group by c.Id, c.Name, c.Contact, sp.SectorId, s.Name, sp.Summ,c.Date";

      return this.Database.SqlQuery<ReservationView>(sql, match.Id).ToList();
    }


    /// <summary>
    /// Возвращает информацию по бронированию, по указанному номеру брони
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    public ReservationView GetReservationInfo(int Id)
    {
      string sql = @"SELECT c.Id, c.Name, c.Contact, sp.SectorId, s.Name as SectorName, 
                  COUNT(sp.Id) as [Count], sp.Summ as Price, (COUNT(sp.Id) * sp.Summ) as Summ, c.Date as ReservationDate
                    FROM [SoldPlaces] as sp
                    inner join [Clients] as c on c.Id = sp.ClientId
                    inner join [Sectors] as s on s.Id = sp.SectorId
                    where sp.IsReservation = 1
                      and c.Id = @p0
                    group by c.Id, c.Name, c.Contact, sp.SectorId, s.Name, sp.Summ,c.Date";

      return this.Database.SqlQuery<ReservationView>(sql, Id).FirstOrDefault();
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