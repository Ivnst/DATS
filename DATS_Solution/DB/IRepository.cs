﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace DATS
{
  public interface IRepository
  {
    DbSet<Stadium> Stadiums { get; set; }
    DbSet<Sector> Sectors { get; set; }
    DbSet<Client> Clients { get; set; }
    DbSet<Match> Matches { get; set; }
    DbSet<Place> Places { get; set; }
    DbSet<Price> Prices { get; set; }
    DbSet<SoldPlace> SoldPlaces { get; set; }

    int SaveChanges();
         
    //Stadiums
    List<Stadium> GetAllStadiums();

    //Matches
    List<Match> GetMatchesByStadium(Stadium stadium);
    List<SectorView> GetSectorsStatistics(Match match);
  }
}
