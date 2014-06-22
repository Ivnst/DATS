using System.Data.Entity;

namespace DATS
{
  /// <summary>
  /// Часть класса SqlRep, работающая с мероприятиями
  /// </summary>
  public partial class SqlRep
  {
    /// <summary>
    /// Список мероприятий
    /// </summary>
    public DbSet<Match> Matches { get; set; }



    public void SaveMatch(Match match)
    {


        if (match.Id == 0)
        {
            Matches.Add(match);
        }
        else
        {
            Match dbEntry = Matches.Find(match.Id);
            if (dbEntry != null)
            {

                dbEntry.StadiumId = match.StadiumId;
                dbEntry.Name = match.Name;
                dbEntry.BeginsAt = match.BeginsAt;
                dbEntry.Duration = match.Duration;
            }
        }
        SaveChanges();
    }


    public void DeleteMatch(Match match)
    {

        if (match.Id == 0)
        {
            // нечего удалять
        }
        else
        {
            Match dbEntry = Matches.Find(match.Id);
            if (dbEntry != null)
            {
                Matches.Remove(dbEntry);
                SaveChanges();
            }
        }

    }



  }
}