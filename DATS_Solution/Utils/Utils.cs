using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace DATS
{
  public static class Utils
  {
    private static string goodSymbols = null;
    /// <summary>
    /// Разрешенные символы
    /// </summary>
    public static string GoodSymbols
    {
      get
      {
        if (goodSymbols == null)
        {
          goodSymbols = string.Concat(
                "abcdefghijklmnopqrstuvwxyz",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                "абвгдеёжзийклмнопрстуфхцчшщъыьэюя",
                "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ",
                "1234567890",
                " `~!@#$%^&*()_-=+[]{};:'\\|,./?\"");
        }
        return goodSymbols;
      }
    }

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

    /// <summary>
    /// Укорачивает длину строки, вставляя в центре строки "..."
    /// </summary>
    /// <param name="input">Входящая строка</param>
    /// <param name="requiredLength">Рекомендуемая длина</param>
    /// <param name="dots">Обозначение удалённой части строки</param>
    /// <returns></returns>
    public static string ShortenString(string input, int requiredLength, string dots = "...")
    {
      if (string.IsNullOrEmpty(input)) return input;
      if (requiredLength > input.Length) return input;
      if (requiredLength <= 0) return ""; //если некорректная желаемая длина
      if (input.Length - requiredLength < dots.Length) return input; //если сокращение не имеет смысла 

      requiredLength = requiredLength - dots.Length;

      int first = requiredLength / 2;      //Длина первого куска текста
      int second = requiredLength - first; //Длина второго куска текста

      var firstPart = input.Substring(0, first);
      var secondPart = input.Substring(input.Length - second, second);

      //Результат
      var ouputtext = string.Concat(firstPart, dots, secondPart);

      return ouputtext;
    }


    /// <summary>
    /// Разбивает строку на куски определённой длины, разделяя их указанной строкой
    /// </summary>
    /// <param name="input">Входящая строка</param>
    /// <param name="requiredLength">Рекомендуемая длина</param>
    /// <param name="divider">Разделитель частей</param>
    /// <returns></returns>
    public static string SplitString(string input, int requiredLength, char divider = ' ')
    {
      if (string.IsNullOrEmpty(input)) return input;
      if (requiredLength > input.Length) return input;
      if (requiredLength <= 0) return ""; //если некорректная желаемая длина

      StringBuilder sb = new StringBuilder(input.Length);
      
      if (input.Contains(divider))
      {
        string[] parts = input.Split(divider);
        foreach (string part in parts)
        {
          if (string.IsNullOrEmpty(part)) continue;
          if(part.Length < requiredLength)
          {
            sb.Append(part);
          }
          else
          {
            sb.Append(SplitString(part, requiredLength, divider));
          }
          sb.Append(divider);
        }
      }
      else
      {
        for (int i = 0; i < input.Length; i += requiredLength)
        {
          int len = (i + requiredLength > input.Length) ? input.Length - i : requiredLength;
          string part = input.Substring(i, len);

          if (i != 0)
            sb.Append(divider);
          sb.Append(part);
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Удаление спец символов из строки
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string DeleteSpecialCharacters(string src)
    {
      if (string.IsNullOrEmpty(src)) return src;

      StringBuilder sb = new StringBuilder(src.Length);

      string allowed = GoodSymbols;

      foreach (char ch in src)
        if (allowed.Contains(ch))
            sb.Append(ch);

      return sb.ToString();
    }

  }
}