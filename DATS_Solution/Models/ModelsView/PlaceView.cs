using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Облегчённый вариант модели Places для передачи в JavaScript
  /// </summary>
  public class PlaceView
  {
    public PlaceView()
    {
    }

    public PlaceView(Place place)
    {
      this.Row = place.Row;
      this.Col = place.Column;
      this.RowPos = place.RowPos;
      this.ColPos = place.ColumnPos;
      this.Price = 0;
    }
    
    public PlaceView(int row, int col, int rowPos, int colPos)
    {
      this.Row = row;
      this.Col = col;
      this.RowPos = rowPos;
      this.ColPos = colPos;
    }

    /// <summary>
    /// Код места
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ряд
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Номер места
    /// </summary>
    public int Col { get; set; }

    /// <summary>
    /// Позиция ряда
    /// </summary>
    public int RowPos { get; set; }

    /// <summary>
    /// Позиция места
    /// </summary>
    public int ColPos { get; set; }

    /// <summary>
    /// Состояние (для редактирования сектора - не используется. Для продажи - см. перечисление PlaceState).
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// Цена билета
    /// </summary>
    public decimal Price { get; set; }

    #region <Methods>

    public Place ToPlace()
    {
      Place result = new Place();
      result.Column = this.Col;
      result.ColumnPos = this.ColPos;
      result.Row = this.Row;
      result.RowPos = this.RowPos;
      return result;
    }

    #endregion
  }
}