using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с настройками стадионов
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Настройки
    /// </summary>
    public DbSet<Config> Configs { get; set; }

    /// <summary>
    /// Получение параметров стадиона
    /// </summary>
    /// <param name="stadium"></param>
    /// <returns></returns>
    public ConfigView GetConfigView(Stadium stadium)
    {
      if(stadium == null) throw new ArgumentNullException("stadium");

      //достаём все параметры, относящиеся к указанному стадиону
      List<Config> configsList = Configs.Where<Config>(c => c.StadiumId == stadium.Id).ToList<Config>();

      ConfigView result = new ConfigView();
      result.StadiumId = stadium.Id;

      //ищем необходимые параметры
      int Tag = 0;
      foreach (Config config in configsList)
      {
          Tag = 1;
        if (config.Name == ConfigView.RemoveReservationPeriod_ConfigName)
          result.RemoveReservationPeriod = Convert.ToInt32(config.Val);

        //если добавится новый параметр - добавить сюда
      }

      if (Tag == 0) result.RemoveReservationPeriod = 30;

      return result;
    }


    /// <summary>
    /// Сохранение параметров стадиона
    /// </summary>
    /// <param name="configView"></param>
    public void SetConfigView(ConfigView configView)
    {
      if (configView == null) throw new ArgumentNullException("configView");


      //достаём все параметры, относящиеся к указанному стадиону
      List<Config> configsList = Configs.Where<Config>(c => c.StadiumId == configView.StadiumId).ToList<Config>();


      //сохраняем параметр RemoveReservationPeriod
      Config reservationRemovePeriodConfig = configsList.Find(c => c.Name == ConfigView.RemoveReservationPeriod_ConfigName);
      if(reservationRemovePeriodConfig == null)
      {
        Config newConfig = new Config();
        newConfig.StadiumId = configView.StadiumId;
        newConfig.Name = ConfigView.RemoveReservationPeriod_ConfigName;
        newConfig.Val = configView.RemoveReservationPeriod.ToString();
        Configs.Add(newConfig);
      }
      else
      {
        reservationRemovePeriodConfig.Val = configView.RemoveReservationPeriod.ToString();
        this.Entry<Config>(reservationRemovePeriodConfig).State = EntityState.Modified;
      }

      //если добавится новый параметр - добавить сюда

      SaveChanges();
    }
  }
}