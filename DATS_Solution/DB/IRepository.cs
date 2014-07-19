using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace DATS
{
  public interface IRepository
  {
    DbSet<SoldPlace> SoldPlaces { get; set; }
    DbSet<Stadium> Stadiums { get; set; }
    DbSet<Sector> Sectors { get; set; }
    DbSet<Client> Clients { get; set; }
    DbSet<Config> Configs { get; set; }
    DbSet<Match> Matches { get; set; }
    DbSet<Place> Places { get; set; }
    DbSet<Price> Prices { get; set; }

    int SaveChanges();
         
    //Stadiums
    List<Stadium> GetAllStadiums();
    Stadium FindStadium(int sid);

    //Matches
    Match FindMatch(int mid);
    List<Match> GetMatchesByStadium(Stadium stadium);
    List<SectorView> GetSectorsStatistics(Match match);

    //Places
    List<Place> GetPlacesBySector(Sector sector);
    void SavePlacesBySector(Sector sector, List<PlaceView> places);

    //SoldPlaces
    List<SoldPlace> GetSoldPlaces(Match match, Sector sector);
    List<SoldPlace> GetSoldPlacesByReservationId(int reservationId, bool onlyReserved);
    List<Place> GetPlacesByReservationId(int reservationId, bool onlyReserved);
    void ProcessTicketsSelling(Match match, Sector sector, List<PlaceView> places);
    void ProcessTicketsReturning(Match match, Sector sector, List<PlaceView> places);
    int ProcessTicketsReservation(ClientView clientView, List<PlaceView> places);

    //Sectors
    Sector FindSector(int sid);
    Sector CopySector(Sector sector);

    //Clients
    List<ReservationView> GetReservationsList(Match match);
    List<ReservationView> GetReservationsList(string searchString);
    ReservationView GetReservationInfo(int Id);
    void SellAllReservation(ReservationView reservation);
    void ReleaseAllReservation(ReservationView reservation);

    //Prices
    decimal GetPrice(int sectorId, int matchId);
  }
}
