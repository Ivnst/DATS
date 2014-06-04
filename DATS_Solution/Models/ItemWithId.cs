using System.ComponentModel.DataAnnotations;

namespace DATS
{
  public class ItemWithId
  {
    [Key]
    public int Id { get; set; }
  }
}