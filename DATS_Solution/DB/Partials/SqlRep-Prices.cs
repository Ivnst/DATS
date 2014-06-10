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

  }
}