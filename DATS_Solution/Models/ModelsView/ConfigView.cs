using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace DATS
{
  /// <summary>
  /// 
  /// </summary>
  public class ConfigView
  {
      // значение параметра по умолчанию
      public ConfigView()
      {
          RemoveReservationPeriod = 30;
      }

    /// <summary>
    /// Имя стадиона
    /// </summary>
    public int StadiumId { get; set; }
    /// <summary>
    /// Время, за которое до начала мероприятия все забронированные места попадают в продажу (в минутах)
    /// </summary>
    [DisplayName("Период времени до мероприятия, за который возвращать забронированные билеты в свободную продажу")]
    public int RemoveReservationPeriod { get; set; }

    #region <Static fields>
    
    /// <summary>
    /// Название параметра в базе данных для поля RemoveReservationPeriod
    /// </summary>
    public static string RemoveReservationPeriod_ConfigName = "rrperiod";

    #endregion
  }
}