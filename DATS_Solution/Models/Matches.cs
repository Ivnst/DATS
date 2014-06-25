using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
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
    [DisplayName("Ид. стадиона")]
    [Required(ErrorMessage = "Пожалуйста выбирите стадион.")]
    public int StadiumId { get; set; }

    /// <summary>
    /// Название матча (или мероприятия)
    /// </summary>
    [DisplayName("Название мероприятия")]
    [DataType(DataType.MultilineText)]
    [Required(ErrorMessage = "Пожалуйста введите название матча (или мероприятия).")]
    public string Name { get; set; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    [DisplayName("Дата и время начала")]
    [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Range(typeof(DateTime), "01.01.2000", "31.12.2099")]
    [Required(ErrorMessage = "Пожалуйста введите дату и время начала (dd.MM.yyyy HH:mm:ss).")]
    public DateTime BeginsAt { get; set; }

    /// <summary>
    /// Продолжительность в минутах
    /// </summary>
    [DisplayName("Продолжительность в минутах")]
    [Range(1, int.MaxValue, ErrorMessage = @"Поле ""Продолжительность в минутах"" должно содержать целое число большее нуля.")]
    [Required(ErrorMessage = "Пожалуйста введите продолжительность в минутах.")]
    public int Duration { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Стадион, на котором происходит текущий матч
    /// </summary>
    public Stadium Stadium { get; set; }

  }
}