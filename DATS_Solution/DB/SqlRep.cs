using System.Data.Entity;

namespace DATS
{
  public partial class SqlRep : DbContext, IRepository
  {
    /// <summary>
    /// Конструктор (здесь DATS - название базы данных)
    /// </summary>
    public SqlRep() : base("DefaultConnection") {} 


    /// <summary>
    /// Сохранение изменений в базе данных
    /// </summary>
    /// <returns></returns>
    public int SaveChanges()
    {
      return base.SaveChanges();
    }
       
  }
}