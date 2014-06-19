using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

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

    /// <summary>
    /// Возвращает список секторов у указанного стадиона
    /// </summary>
    /// <param name="stadium"></param>
    /// <returns></returns>
    public List<Sector> GetSectorsByStadium(Stadium stadium)
    {
      return GetSectorsByStadium(stadium.Id);
    }

    /// <summary>
    /// Возвращает список секторов у указанного стадиона
    /// </summary>
    /// <param name="stadium"></param>
    /// <returns></returns>
    public List<Sector> GetSectorsByStadium(int stadiumId)
    {
      return Sectors.Where<Sector>(s => s.StadiumId == stadiumId).ToList<Sector>();
    }
  }
}