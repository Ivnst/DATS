using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Таблица со списком цен в каждом секторе на конкретном матче
  /// </summary>
  [Table("Prices")]
  public class Price : ItemWithId
  {
    /// <summary>
    /// Код матча
    /// </summary>
    [Required]
    public int MatchId { get; set; }
    /// <summary>
    /// Код сектора на стадионе
    /// </summary>
    [Required]
    public int SectorId { get; set; }
    /// <summary>
    /// Цена
    /// </summary>
    [Required]
    public decimal PriceValue { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Матч, к которому относится данная цена билетов
    /// </summary>
    public Match Match { get; set; }
    /// <summary>
    /// Сектор, к которому относится данная цена билетов
    /// </summary>
    public Sector Sector { get; set; }
  }
}