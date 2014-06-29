using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System;

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
      if (sector == null) throw new ArgumentNullException("sector");
      if (Sectors.Count<Sector>(s => s.Id == sector.Id) == 0) throw new ArgumentException("Sector not exists");

      return Places.Where<Place>(p => p.SectorId == sector.Id).ToList<Place>();
    }

    /// <summary>
    /// Сохранение мест для сектора
    /// </summary>
    /// <param name="sector"></param>
    /// <param name="places"></param>
    public bool SavePlacesBySector(Sector sector, List<PlaceView> places)
    {
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (Sectors.Count<Sector>(s => s.Id == sector.Id) == 0) throw new ArgumentException("Sector not exists");

      if (!CanEditSector(sector)) return false;

      //Далее мы составляем матрицу мест, чтобы распределить номера мест (не путать с положением)
      //а также чтобы избежать некоторых других ошибок (например наложение мест друг на друга)

      //для начала определим какая размерность нашего сектора
      int maxCol = 0;
      int maxRow = 0;
      foreach (PlaceView pv in places)
      {
        if(pv.Row > maxRow) maxRow = pv.Row;
        if(pv.Col > maxCol) maxCol = pv.Col;
      }
      maxCol++;
      maxRow++;
      
      //создаём пустую матрицу с размерами нашего сектора
      PlaceView[][] placeMatrix = new PlaceView[maxRow][];
      for (int i = 0; i < maxRow; i++)
      {
        placeMatrix[i] = new PlaceView[maxCol];
      }

      //распределяем места по матрице
      foreach (PlaceView pv in places)
      {
        if (placeMatrix[pv.Row][pv.Col] != null) continue; //если уже есть такое место, то пропускаем
        placeMatrix[pv.Row][pv.Col] = pv;
      }

      //Нумеруем места
      int currentPlace = 0;
      for (int i = 0; i < maxRow; i++)
      {
        currentPlace = 1;
        for (int j = 0; j < maxCol; j++)
        {
          if(placeMatrix[i][j] != null)
          {
            placeMatrix[i][j].Num = currentPlace;
            currentPlace++;
          }
        }
      }

      //удаление или обновление старых мест
      List<Place> existPlaces = GetPlacesBySector(sector);
      foreach (Place place in existPlaces)
      {
        if(places.Count != 0)
        {
          //переиспользование старых мест.
          PlaceView currPlaceView = places[0];
          places.RemoveAt(0);
          place.Row = currPlaceView.Row;
          place.Location = currPlaceView.Col;
          place.Position = currPlaceView.Num;
          Entry<Place>(place).State = EntityState.Modified;
        }
        else
        {
          Entry<Place>(place).State = EntityState.Deleted;
        }
        
      }

      //добавление новых мест
      //смотрим на places, а не на placeMatrix, т.к. это одни и те же объекты, но в PlaceMatrix необходимо два цикла.
      foreach (PlaceView placeView in places)
      {
        Place newPlace = new Place();
        newPlace.Row = placeView.Row;
        newPlace.Location = placeView.Col;
        newPlace.Position = placeView.Num;
        newPlace.SectorId = sector.Id;
        Places.Add(newPlace);
      }

      //сохранение изменений
      SaveChanges();

      return true;
    }

    /// <summary>
    /// Проверяет, возможно ли редактирование мест сектора. Если в секторе не было продаж, значит можно. 
    /// </summary>
    /// <param name="sector"></param>
    /// <returns></returns>
    public bool CanEditSector(Sector sector)
    {
      return (GetCountOfSoldPlacesInSector(sector) == 0);
    }
  }
}