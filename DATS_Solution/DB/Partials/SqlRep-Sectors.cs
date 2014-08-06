using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using System.Transactions;

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
      return Sectors.Where<Sector>(s => s.StadiumId == stadiumId).OrderBy(s => s.Name).ToList<Sector>();
    }


    /// <summary>
    /// Возвращает сектор с указанным кодом или null, если такой сектор не найден.
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public Sector FindSector(int sid)
    {
      return Sectors.FirstOrDefault<Sector>(s => s.Id == sid);
    }

    /// <summary>
    /// Создание копии указанного сектора
    /// </summary>
    /// <param name="sector"></param>
    public Sector CopySector(Sector sector)
    {
      if (sector == null) throw new ArgumentNullException("sector");

      using (TransactionScope scope = new TransactionScope())
      {
        Sector newSector = new Sector();
        newSector.Name = "Копия " + sector.Name;
        newSector.StadiumId = sector.StadiumId;

        Sectors.Add(newSector);
        SaveChanges();

        List<Place> places = this.GetPlacesBySector(sector);

        foreach (Place place in places)
        {
          place.SectorId = newSector.Id;
          this.Entry<Place>(place).State = EntityState.Added;
        }
        SaveChanges();

        scope.Complete();
        return newSector;
      }
    }

  }
}