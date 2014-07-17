using System.Linq;
using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с ценами билетов
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список цен в каждом секторе на каждом мероприятии
    /// </summary>
    public DbSet<Price> Prices { get; set; }


    /// <summary>
    /// Возвращает цену в указанном секторе и на указанное мероприятие
    /// </summary>
    /// <param name="sectorId"></param>
    /// <param name="matchId"></param>
    /// <returns></returns>
    public decimal GetPrice(int sectorId, int matchId)
    {
      Price price = Prices.FirstOrDefault<Price>(p => (p.SectorId == sectorId) && (p.MatchId == matchId));
      if (price == null) return 0;
      return price.PriceValue;
    }
  }
}