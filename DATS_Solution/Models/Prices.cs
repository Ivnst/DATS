using System.ComponentModel;
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
    [DisplayName("Ид. матча")]
    [Required(ErrorMessage = "Пожалуйста выбирите ид. матча.")]
    public int MatchId { get; set; }
    /// <summary>
    /// Код сектора на стадионе
    /// </summary>
    [DisplayName("Ид. сектора")]
    [Required(ErrorMessage = "Пожалуйста выбирите ид. сектора.")]
    public int SectorId { get; set; }
    /// <summary>
    /// Цена
    /// </summary>
    [DisplayName("Цена")]
    [Range(0, 1000000000, ErrorMessage = @"Поле ""Продолжительность в минутах"" должно содержать целое число большее нуля.")]
    [Required(ErrorMessage = "Пожалуйста введите продолжительность в минутах.")]
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