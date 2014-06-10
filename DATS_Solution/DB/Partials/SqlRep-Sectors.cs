using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с секторами стадионов
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список секторов в каждом стадионе
    /// </summary>
    public DbSet<Sector> Sectors { get; set; }

  }
}