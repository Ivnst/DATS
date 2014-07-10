using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

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
    [Required(ErrorMessage = "Пожалуйста, укажите имя клиента!")]
    [DisplayName("Имя клиента")]
    public string Name { get; set; }
    /// <summary>
    /// Код матча
    /// </summary>
    [Required(ErrorMessage = "Пожалуйста, укажите контактные данные клиента!")]
    [DisplayName("Контактные данные")]
    public string Contact { get; set; }
    /// <summary>
    /// Дата создания клиента
    /// </summary>
    [Required]
    [DisplayName("Дата бронирования")]
    public DateTime Date { get; set; }
    /// <summary>
    /// Если true - бронь по текущему клиенту активна.
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
  }
}