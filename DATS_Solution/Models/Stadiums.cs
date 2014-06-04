using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
  /// <summary>
  /// Таблица со списком стадионов
  /// </summary>
  [Table("Stadiums")]
  public class Stadium : ItemWithId
  {
    public Stadium()
    {
      Sectors = new List<Sector>();
      Matches = new List<Match>();
    }

    /// <summary>
    /// Наименование стадиона
    /// </summary>
    [Required]
    [MinLength(10)]
    public string Name { get; set; }
    /// <summary>
    /// Адрес стадиона
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// Путь к изображению со схемой
    /// </summary>
    public string SchemePath { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Секторы в текущем стадионе
    /// </summary>
    public List<Sector> Sectors { get; set; }
    /// <summary>
    /// Матчи в текущем стадионе
    /// </summary>
    public List<Match> Matches { get; set; }
  }
}