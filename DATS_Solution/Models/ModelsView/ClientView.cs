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
    /// Код матча
    /// </summary>
    [DisplayName("Имя клиента")]
    public string Name { get; set; }
    /// <summary>
    /// Код матча
    /// </summary>
    [DisplayName("Контактные данные")]
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