using System.Web.Mvc;
using Ninject;
using System;
using System.Web;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections;
using Newtonsoft.Json;

namespace DATS.Controllers
{
    public class BaseController : Controller
    {
      [Inject]
      public IRepository Repository { get; set; }

      #region <Fields>
      /// <summary>
      /// Логгер
      /// </summary>
      protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

      /// <summary>
      /// Различная информация (кэш)
      /// </summary>
      ConcurrentDictionary<string, string> cachedData = new ConcurrentDictionary<string, string>();

      #endregion

      #region <Properties>
      /// <summary>
      /// Текущий стадион
      /// </summary>
      public Stadium CurrentStadium
      {
        get
        {
          //view cookie
          int stadiumId = ReadIntValueFromCookie("ss");//SS - selected stadium

          //search stadium (check cookie value)
          Stadium stadium = Repository.FindStadium(stadiumId);
          if (stadium == null)
          {
            //Если не нашли, берем первый стадион
            stadium = Repository.Stadiums.FirstOrDefault<Stadium>();
            //и сохраняем его
            CurrentStadium = stadium;
          }

          return stadium;
        }
        set
        {
          if (value == null) return;

          //create cookie
          WriteIntValueIntoCookie("ss", value.Id);
        }
      }

      /// <summary>
      /// Текущее мероприятие
      /// </summary>
      public Match CurrentMatch
      {
        get
        {
          Stadium stadium = this.CurrentStadium;
          if (stadium == null) return null;

          //view cookie
          int matchId = ReadIntValueFromCookie("sm");//SM - selected match

          Match match = Repository.Matches.FirstOrDefault<Match>(m => m.Id == matchId);
          if (match == null || match.StadiumId != stadium.Id)
          {
            //Если не нашли, берем первое мероприятие
            match = Repository.GetMatchesByStadium(stadium).FirstOrDefault<Match>();
            //и сохраняем его
            CurrentMatch = match;
          }

          return match;
        }
        set
        {
          if (value == null) return;

          //create cookie
          WriteIntValueIntoCookie("sm", value.Id);
        }
      }

      #endregion

      #region <Functions>
      
      /// <summary>
      /// Считывает целочисленное значение из куки
      /// </summary>
      /// <param name="cookieName"></param>
      /// <returns></returns>
      protected int ReadIntValueFromCookie(string cookieName)
      {
        if (string.IsNullOrEmpty(cookieName)) throw new ArgumentNullException("cookieName");
        var cookie = Request.Cookies[cookieName]; //SS - selected stadium
        return Convert.ToInt32(cookie == null ? "-1" : cookie.Value);
      }

      /// <summary>
      /// Записывает целочисленное значение в куку
      /// </summary>
      /// <param name="cookieName"></param>
      /// <param name="value"></param>
      protected void WriteIntValueIntoCookie(string cookieName, int value)
      {
        if (string.IsNullOrEmpty(cookieName)) throw new ArgumentNullException("cookieName");
        var cookie = new HttpCookie(cookieName, value.ToString());
        Response.SetCookie(cookie);
      }

      /// <summary>
      /// заполняем данные для View
      /// </summary>
      /// <param name="currentStadium"></param>
      /// <param name="currentMatch"></param>
      protected void FillViewBag(Stadium currentStadium, Match currentMatch)
      {
        if (currentStadium == null)
        {
          currentStadium = new Stadium();
          currentStadium.Name = "Стадионы не найдены";
        }

        //заполняем данные для View
        ViewBag.Stadiums = Repository.GetAllStadiums();
        ViewBag.CurrentStadium = currentStadium;
        ViewBag.CurrentMatch = currentMatch;
        ViewBag.Matches = Repository.GetMatchesByStadium(currentStadium);
      }


      /// <summary>
      /// Сохраняет значение в кэш и возвращает ключ, по которому это значение можно достать.
      /// </summary>
      /// <param name="data"></param>
      /// <returns></returns>
      protected string StoreDataInCache(string data)
      {
        long t = DateTime.UtcNow.Ticks;
        string key = t.ToString();

        while (System.Web.HttpContext.Current.Cache[key] != null)
        {
          t++;
          key = t.ToString();
        }
        System.Web.HttpContext.Current.Cache[key] = data;

        return key;
      }


      /// <summary>
      /// Достаёт значение из кэша по ключу и удаляет значение из кэша.
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      protected string GetDataFromCache(string key)
      {
        string data = System.Web.HttpContext.Current.Cache[key] as string;
        System.Web.HttpContext.Current.Cache.Remove(key);
        return data;
      }


      /// <summary>
      /// Подготавливает данные для вывода окна с указанным сообщением и заголовком.
      /// Использование: передать возвращённый данным методом ключ на страницу с помощью параметра notify
      /// Пример: Home/Index?notify=234234234234234
      /// </summary>
      /// <param name="_message"></param>
      /// <param name="_header"></param>
      /// <param name="_error"></param>
      /// <returns></returns>
      protected string PrepareMessageBox(string _message, string _header, bool _error = false)
      {
        //Создаём сообщение, отображающее код брони пользователю
        var messageObject = new { header = _header, message = _message, error = _error };
        string messageJson = JsonConvert.SerializeObject(messageObject);
        return StoreDataInCache(messageJson);
      }

      #endregion
    }
}
