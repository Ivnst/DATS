using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Облегчённый вариант модели Places для передачи в JavaScript
  /// </summary>
  public class PlaceView
  {
    public PlaceView(int row, int col, int label)
    {
      this.Row = row;
      this.Num = label;
      this.Col = col;
    }

    /// <summary>
    /// Аналог Place.Row. Позиция места по вертикали
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Аналог Place.Location. Позиция места по горизонтали
    /// </summary>
    public int Col { get; set; }

    /// <summary>
    /// Аналог Place.Position. Порядковый номер места
    /// </summary>
    public int Num { get; set; }
  }
}