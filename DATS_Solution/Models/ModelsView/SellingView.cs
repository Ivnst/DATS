using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

namespace DATS
{
  /// <summary>
  /// Вьюшка для окна подтверждения продажи или возврата билетов
  /// </summary>
  public class SellingView
  {
    /// <summary>
    /// Информация, переданная от страницы продажи
    /// </summary>
    public string Data {get; set;}

    /// <summary>
    /// Код сектора
    /// </summary>
    [DisplayName("Код сектора")]
    public int SectorId { get; set; }
    /// <summary>
    /// Наименование сектора
    /// </summary>
    [DisplayName("Сектор")]
    public string SectorName { get; set; }

    /// <summary>
    /// Наименование стадиона
    /// </summary>
    [DisplayName("Стадион")]
    public string StadiumName { get; set; }

    /// <summary>
    /// Код мероприятия
    /// </summary>
    [DisplayName("Код мероприятия")]
    public int MatchId { get; set; }
    /// <summary>
    /// Наименование мероприятия
    /// </summary>
    [DisplayName("Мероприятие")]
    public string MatchName { get; set; }

    /// <summary>
    /// Количество мест
    /// </summary>
    [DisplayName("Количество")]
    public int Count { get; set; }
    /// <summary>
    /// Цена билета
    /// </summary>
    [DisplayName("Цена")]
    public decimal Price { get; set; }
    /// <summary>
    /// Итоговая сумма
    /// </summary>
    [DisplayName("Сумма")]
    public decimal Summ { get; set; }
    /// <summary>
    /// Места
    /// </summary>
    [DisplayName("Места")]
    public string PlacesList { get; set; }
  }
}