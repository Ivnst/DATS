using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace DATS
{
  /// <summary>
  /// Таблица со списком клиентов, которые забронировали места
  /// </summary>
  [Table("Clients")]
  public class Client : ItemWithId
  {
    /// <summary>
    /// Код матча
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// Код матча
    /// </summary>
    [Required]
    public string Contact { get; set; }
    /// <summary>
    /// Дата создания клиента
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
    /// <summary>
    /// Если true - бронь по текущему клиенту активна.
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
  }
}