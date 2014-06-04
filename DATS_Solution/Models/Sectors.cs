using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace DATS
{
  /// <summary>
  /// Таблица со списком секторов у конкретного стадиона
  /// </summary>
  [Table("Sectors")]
  public class Sector : ItemWithId
  {
    public Sector()
    {
      Places = new List<Place>();
    }
    
    /// <summary>
    /// Код стадиона
    /// </summary>
    [Required]
    public int StadiumId { get; set; }
    /// <summary>
    /// Название сектора. (Например, 1, 2, 3, VIP, Для инвалидов и тд)
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// Цвет сектора
    /// </summary>
    public int Color { get; set; }

    //------------------------------------------------------------------------------
    /// <summary>
    /// Стадион, к которому относится текущий сектор
    /// </summary>
    public Stadium Stadium { get; set; }
    /// <summary>
    /// Места в текущем секторе
    /// </summary>
    public List<Place> Places { get; set; }

  }
}