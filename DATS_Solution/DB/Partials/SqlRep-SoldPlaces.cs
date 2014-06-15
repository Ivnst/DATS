using System.Linq;
using System.Data.Entity;

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
  }
}