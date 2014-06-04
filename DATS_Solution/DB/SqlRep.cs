using System.Data.Entity;

namespace DATS
{
  public class SqlRep : DbContext, IRepository
  {
    public SqlRep() : base("DATS") { }

    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<SoldPlace> SoldPlaces { get; set; }
  }
}