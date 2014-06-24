﻿using System.Data.Entity;
using System.Linq;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая со стадионами
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список стадионов
    /// </summary>
    public DbSet<Stadium> Stadiums { get; set; }

  }
}