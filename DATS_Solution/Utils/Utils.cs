using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATS
{
  public static class Utils
  {
    /// <summary>
    /// Возвращает текущее наше время
    /// </summary>
    /// <returns></returns>
    public static DateTime GetNow()
    {
      return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "FLE Standard Time");
    }


    /// <summary>
    /// Конвертирует указанное время в текущее наше время (добавляет необходимое количество часов)
    /// </summary>
    /// <returns></returns>
    public static DateTime GetNow(DateTime date)
    {
      return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, "FLE Standard Time");
    }


    /// <summary>
    /// Корректирует дату на три часа вперед
    /// </summary>
    /// <returns></returns>
    public static DateTime AdjustDate(DateTime date)
    {
      return date.AddHours(3);
    }
  }
}