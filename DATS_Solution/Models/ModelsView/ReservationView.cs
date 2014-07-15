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
    public int Id { get; set; }    
    /// <summary>
    /// Фио клиента
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Контактные данные клиента
    /// </summary>
    public string Contact { get; set; }
    /// <summary>
    /// Код сектора
    /// </summary>
    public int SectorId { get; set; }
    /// <summary>
    /// Наименование сектора
    /// </summary>
    public string SectorName { get; set; }
    /// <summary>
    /// Количество забронированных мест
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    /// Цена забронированных мест
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Итоговая сумма брони
    /// </summary>
    public decimal Summ { get; set; }
    /// <summary>
    /// Дата бронирования
    /// </summary>
    public DateTime ReservationDate { get; set; }
    /// <summary>
    /// Статус
    /// </summary>
    public string StatusName { get; set; }
  }
}