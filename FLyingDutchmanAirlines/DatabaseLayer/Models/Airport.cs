using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public partial class Airport
{
    public Airport()
    {
        FlightDestinationNavigations = new HashSet<Flight>();
        FlightOriginNavigations = new HashSet<Flight>();
    }

    public int AirportId { get; set; }
    public string City { get; set; } = null!;
    public string Iata { get; set; } = null!;

    public virtual ICollection<Flight> FlightDestinationNavigations { get; set; }
    public virtual ICollection<Flight> FlightOriginNavigations { get; set; }
}
