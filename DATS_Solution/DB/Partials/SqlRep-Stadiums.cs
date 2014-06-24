using System.Data.Entity;
using System.Collections.Generic;
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
    IQueryable<Stadium> IRepository.Stadiumes { get { return Stadiums; } }


    /// <summary>
    /// Возвращает список всех стадионов
    /// </summary>
    /// <returns></returns>
    public List<Stadium> GetAllStadiums()
    {
      return Stadiums.ToList<Stadium>();
    }
  }
}