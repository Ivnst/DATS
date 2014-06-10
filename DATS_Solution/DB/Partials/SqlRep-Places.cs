using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с местами в секторах
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список мест в каждом секторе
    /// </summary>
    public DbSet<Place> Places { get; set; }

  }
}