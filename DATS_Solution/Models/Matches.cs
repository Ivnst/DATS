using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Таблица со списком матчей у конкретного стадиона
  /// </summary>
  [Table("Matches")]
  public class Match : ItemWithId
  {
    /// <summary>
    /// Код стадиона
    /// </summary>
    [Required]
    public int StadiumId { get; set; }
    /// <summary>
    /// Название матча (или мероприятия)
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// Дата и время начала
    /// </summary>
    [Required]
    public DateTime BeginsAt { get; set; }
    /// <summary>
    /// Продолжительность в минутах
    /// </summary>
    [Required]
    public int Duration { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Стадион, на котором происходит текущий матч
    /// </summary>
    public Stadium Stadium { get; set; }
  }
}