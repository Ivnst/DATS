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
    /// Ряд
    /// </summary>
    [Required]
    public int Row { get; set; }
    /// <summary>
    /// Номер места
    /// </summary>
    [Required]
    public int Position { get; set; }
    /// <summary>
    /// Позиция места (для отображения)
    /// </summary>
    [Required]
    public int Location { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Сектор, к которому относится текущее место
    /// </summary>
    public Sector Sector { get; set; }
  }
}