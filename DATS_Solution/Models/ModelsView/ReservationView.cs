using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

namespace DATS
{
  /// <summary>
  /// 
  /// </summary>
  public class ReservationView
  {
    /// <summary>
    /// Код брони
    /// </summary>
    [DisplayName("Номер брони")]
    public int Id { get; set; }    
    /// <summary>
    /// Фио клиента
    /// </summary>
    [DisplayName("Имя клиента")]
    public string Name { get; set; }
    /// <summary>
    /// Контактные данные клиента
    /// </summary>
    [DisplayName("Контакт клиента")]
    public string Contact { get; set; }
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
    /// Количество забронированных мест
    /// </summary>
    [DisplayName("Количество")]
    public int Count { get; set; }
    /// <summary>
    /// Цена забронированных мест
    /// </summary>
    [DisplayName("Цена")]
    public decimal Price { get; set; }
    /// <summary>
    /// Итоговая сумма брони
    /// </summary>
    [DisplayName("Сумма")]
    public decimal Summ { get; set; }
    /// <summary>
    /// Дата бронирования
    /// </summary>
    [DisplayName("Дата")]
    public DateTime ReservationDate { get; set; }
    /// <summary>
    /// Места
    /// </summary>
    [DisplayName("Места")]
    public string PlacesList { get; set; }
  }
}