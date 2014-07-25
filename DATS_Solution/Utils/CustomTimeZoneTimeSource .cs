using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog.Time;
using System.ComponentModel.DataAnnotations;

namespace DATS
{
  /// <summary>
  /// Класс, необходимый для корректировки времени логгера на нужную нам зону
  /// </summary>
  public class CustomTimeZoneTimeSource : TimeSource
  {
    string ZoneName;
    TimeZoneInfo ZoneInfo;

    [Required]
    public string Zone
    {
      get { return ZoneName; }
      set
      {
        ZoneName = value;
        ZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(value);
      }
    }

    /// <summary>
    /// Текущее время
    /// </summary>
    public override DateTime Time
    {
      get
      {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZoneInfo);
      }
    }
  }
}