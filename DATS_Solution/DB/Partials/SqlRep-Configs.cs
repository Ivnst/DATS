using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с настройками стадионов
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Настройки
    /// </summary>
    public DbSet<Config> Configs { get; set; }

  }
}