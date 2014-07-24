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
    /// Логгер
    /// </summary>
    protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Сохранение изменений в базе данных
    /// </summary>
    /// <returns></returns>
    public int SaveChanges()
    {
      return base.SaveChanges();
    }
      
    /// <summary>
    /// Сохранение изменений в элементе базы данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entry"></param>
    public void SaveEntry<T>(T entry) where T: class
    {
      this.Entry<T>(entry).State = EntityState.Modified;
      SaveChanges();
    }


    /// <summary>
    /// Удаление элемента из базы данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entry"></param>
    public void DeleteEntry<T>(T entry) where T : class
    {
      this.Entry<T>(entry).State = EntityState.Deleted;
      SaveChanges();
    }
  }
}