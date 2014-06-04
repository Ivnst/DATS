using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace DATS
{
  /// <summary>
  /// Таблица со списком мест, которые были забронированы или проданы
  /// </summary>
  [Table("SoldPlaces")]
  public class SoldPlace : ItemWithId
  {
    /// <summary>
    /// Код матча
    /// </summary>
    [Required]
    public int MatchId { get; set; }
    /// <summary>
    /// Код места
    /// </summary>
    [Required]
    public int PlaceId { get; set; }
    /// <summary>
    /// Стоимость билета
    /// </summary>
    [Required]
    public decimal Summ { get; set; }
    /// <summary>
    /// Дата продажи или бронирования
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
    /// <summary>
    /// Место забронировано
    /// </summary>
    [Required]
    public bool IsReservation { get; set; }
    /// <summary>
    /// Ссылка на человека, который забронировал место
    /// </summary>
    public int ClientId { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Матч, к которому относится текущий проданный билет
    /// </summary>
    public Match Match { get; set; }
    /// <summary>
    /// Место, к которому относится текущий проданный билет
    /// </summary>
    public Place Place { get; set; }
    /// <summary>
    /// Клиент (если есть), который забронировал текущий билет
    /// </summary>
    public Client Client { get; set; }

  }
}