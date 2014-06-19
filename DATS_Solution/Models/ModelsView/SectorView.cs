using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATS
{
  public class SectorView
  {
    public int SectorId { get; set; }
    public string Name { get; set; }
    public int TotalPlaces { get; set; }
    public int FreePlaces { get; set; }
    public int SoldPlaces { get; set; }
    public int ReservedPlaces { get; set; }
  }
}