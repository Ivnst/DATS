using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATS
{
    /// <summary>
    /// Состояния мест в секторе
    /// </summary>
    public enum PlaceState 
    {
      /// <summary>
      /// Место свободно
      /// </summary>
      Free = 0,
      /// <summary>
      /// Место продано
      /// </summary>
      Sold = 1,
      /// <summary>
      /// Место забронировано
      /// </summary>
      Reserved = 2,
    }
}
