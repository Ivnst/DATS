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


    /// <summary>
    /// Возвращает список всех стадионов
    /// </summary>
    /// <returns></returns>
    public List<Stadium> GetAllStadiums()
    {
      return Stadiums.ToList<Stadium>();
    }


    /// <summary>
    /// Возвращает стадион с указанным кодом или null, если такой стадион не найден.
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public Stadium FindStadium(int sid)
    {
      return Stadiums.FirstOrDefault<Stadium>(s => s.Id == sid);
    }
  }
}