using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;

namespace DATS
{
  /// <summary>
  /// Таблица со списком настроек в разрезе стадионов
  /// </summary>
  [Table("Configs")]
  public class Config : ItemWithId
  {
    /// <summary>
    /// Код стадиона
    /// </summary>
    public int? StadiumId { get; set; }
    /// <summary>
    /// Название параметра
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    /// <summary>
    /// Значение параметра
    /// </summary>
    [Required]
    public string Val { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Стадион, к которому относится текущая настройка
    /// </summary>
    public Stadium Stadium { get; set; }
  }
}