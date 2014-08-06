using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATS;

namespace System.Web.Mvc
{
  public static class HtmlHelpers
  {
    /// <summary>
    /// Укорачивает длину строки, вставляя в центре строки "..."
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="input">Входящая строка</param>
    /// <param name="requiredLength">Рекомендуемая длина</param>
    /// <param name="dots">Обозначение удалённой части строки</param>
    /// <returns></returns>
    public static MvcHtmlString ShortenString(this HtmlHelper htmlHelper, string input, int requiredLength, string dots = "...")
    {
      return new MvcHtmlString(Utils.ShortenString(input, requiredLength, dots));
    }

    /// <summary>
    /// Разбивает строку на части указанной длины, разделяя их указанной строкой.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="input">Входящая строка</param>
    /// <param name="requiredLength">Рекомендуемая длина</param>
    /// <param name="divider">Разделитель частей</param>
    /// <returns></returns>
    public static MvcHtmlString SplitString(this HtmlHelper htmlHelper, string input, int requiredLength, char divider = ' ')
    {
      return new MvcHtmlString(Utils.SplitString(input, requiredLength, divider));
    }

    /// <summary>
    /// Разбивает строку на части указанной длины, разделяя их указанной строкой.
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="input">Входящая строка</param>
    /// <param name="requiredLength">Рекомендуемая длина</param>
    /// <param name="divider">Разделитель частей</param>
    /// <returns></returns>
    public static MvcHtmlString SplitString(this HtmlHelper htmlHelper, MvcHtmlString input, int requiredLength, char divider = ' ')
    {
      return new MvcHtmlString(Utils.SplitString(HttpUtility.HtmlDecode(input.ToString()), requiredLength, divider));
    }
  }
}