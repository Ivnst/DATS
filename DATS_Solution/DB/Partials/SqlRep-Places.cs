using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

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


    /// <summary>
    /// Возвращает список мест в указанном секторе
    /// </summary>
    /// <param name="sector"></param>
    /// <returns></returns>
    public List<Place> GetPlacesBySector(Sector sector)
    {
      return Places.Where<Place>(p => p.SectorId == sector.Id).ToList<Place>();
    }
  }
}