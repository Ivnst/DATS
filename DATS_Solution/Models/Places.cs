using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Таблица со списком мест у конкретного сектора
  /// </summary>
  [Table("Places")]
  public class Place : ItemWithId
  {
    /// <summary>
    /// Код сектора, к которому относится место
    /// </summary>
    [Required]
    public int SectorId { get; set; }
    /// <summary>
    /// Ряд. Начинается с единицы, нумерация направлена ВВЕРХ.
    /// </summary>
    [Required]
    public int Row { get; set; }
    /// <summary>
    /// Номер места. Начинается с единицы, нумерация направлена вправо.
    /// </summary>
    [Required]
    public int Column { get; set; }

    /// <summary>
    /// Позиция ряда (для отображения). Начинается с нуля, нумерация направлена вниз.
    /// </summary>
    [Required]
    public int RowPos { get; set; }

    /// <summary>
    /// Позиция места (для отображения). Начинается с нуля, нумерация направлена вправо.
    /// </summary>
    [Required]
    public int ColumnPos { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Сектор, к которому относится текущее место
    /// </summary>
    public Sector Sector { get; set; }
  }
}