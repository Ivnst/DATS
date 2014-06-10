using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с мероприятиями
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список мероприятий
    /// </summary>
    public DbSet<Match> Matches { get; set; }

  }
}