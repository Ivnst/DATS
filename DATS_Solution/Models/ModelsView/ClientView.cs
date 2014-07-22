using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

namespace DATS
{
  /// <summary>
  /// 
  /// </summary>
  public class ClientView
  {
    /// <summary>
    /// Имя клиента
    /// </summary>
    [DisplayName("Имя клиента")]
    [Required(ErrorMessage = "Укажите, пожалуйста, имя клиента!")]
    public string Name { get; set; }
    /// <summary>
    /// Контактные данные клиента
    /// </summary>
    [DisplayName("Контактные данные (например, номер телефона)")]
    public string Contact { get; set; }
    /// <summary>
    /// Информация о выбранных местах
    /// </summary>
    public string Data { get; set; }
    /// <summary>
    /// Код мероприятия
    /// </summary>
    public int MatchId { get; set; }
    /// <summary>
    /// Код сектора
    /// </summary>
    public int SectorId { get; set; }
  }
}