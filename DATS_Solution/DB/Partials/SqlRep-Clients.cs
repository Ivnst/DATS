using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с клиентами, которые забронировали билеты
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Клиенты
    /// </summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>
    /// Пример метода, добавляющего нового клиента в базу данных
    /// </summary>
    /// <param name="client"></param>
    public void AddClient(Client client)
    {
      Clients.Add(client);
      base.SaveChanges();
    }
  }
}