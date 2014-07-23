using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using System.Text;

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
    public void SavePlacesBySector(Sector sector, List<PlaceView> places)
    {
      if (sector == null) throw new ArgumentNullException("sector");
      if (places == null) throw new ArgumentNullException("places");
      if (Sectors.Count<Sector>(s => s.Id == sector.Id) == 0) throw new ArgumentException("Указанный сектор не существует!");

      if (!CanEditSector(sector)) throw new InvalidOperationException("По данному сектору уже проводились продажи! Редактирование запрещено!");

      //Далее мы составляем матрицу мест, чтобы распределить номера мест (не путать с положением)
      //а также чтобы избежать некоторых других ошибок (например наложение мест друг на друга)

      //для начала определим какая размерность нашего сектора
      int maxCol = 0;
      int maxRow = 0;
      foreach (PlaceView pv in places)
      {
        if (pv.RowPos > maxRow) maxRow = pv.RowPos;
        if (pv.ColPos > maxCol) maxCol = pv.ColPos;
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
        if (placeMatrix[pv.RowPos][pv.ColPos] != null) continue; //если уже есть такое место, то пропускаем
        placeMatrix[pv.RowPos][pv.ColPos] = pv;
      }

      //Нумеруем места
      int currentRow = 1;
      int currentPlace = 0;
      bool existRow = false;

      for (int i = maxRow - 1; i >= 0; i--) //ряды нумеруются снизу вверх
      {
        currentPlace = 1;
        existRow = false;
        for (int j = 0; j < maxCol; j++)
        {
          if (placeMatrix[i][j] != null)
          {
            placeMatrix[i][j].Col = currentPlace;
            placeMatrix[i][j].Row = currentRow;
            currentPlace++;
            existRow = true;
          }
        }
        if (existRow) currentRow++;
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
          place.Column = currPlaceView.Col;
          place.RowPos = currPlaceView.RowPos;
          place.ColumnPos = currPlaceView.ColPos;
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
        newPlace.Column = placeView.Col;
        newPlace.RowPos = placeView.RowPos;
        newPlace.ColumnPos = placeView.ColPos;
        newPlace.SectorId = sector.Id;
        Places.Add(newPlace);
      }

      //сохранение изменений
      SaveChanges();
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


    /// <summary>
    /// Возвращает список мест в текстовом представлении.
    /// Например: Ряд5: 4 5 6 7 8
    /// </summary>
    /// <param name="reservationId"></param>
    /// <returns></returns>
    public string GetPlacesString(List<Place> places)
    {
      //находим все забронированные места по указаному коду брони
      List<int> rows = new List<int>();
      Dictionary<int, List<int>> placesInRow = new Dictionary<int, List<int>>();
      foreach (Place place in places)
      {
        if (!rows.Contains(place.Row))
          rows.Add(place.Row);
        if (!placesInRow.ContainsKey(place.Row))
          placesInRow.Add(place.Row, new List<int>());
        placesInRow[place.Row].Add(place.Column);
      }
      rows.Sort();

      //составляем строку
      StringBuilder result = new StringBuilder();
      foreach (int row in rows)
      {
        if (result.Length != 0)
          result.Append("\n");

        result.Append(string.Format("Ряд {0}: ", row));
        List<int> placesList = placesInRow[row];
        placesList.Sort();
        foreach (int col in placesList)
        {
          result.Append(col.ToString());
          result.Append(" ");
        }
      }

      return result.ToString();
    }
  }
}