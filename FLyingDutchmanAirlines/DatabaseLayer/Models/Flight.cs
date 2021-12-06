using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public partial class Flight
{
    public Flight() => Bookings = new HashSet<Booking>();

    public int FlightNumber { get; set; }
    public int Origin { get; set; }
    public int Destination { get; set; }

    public virtual Airport DestinationNavigation { get; set; } = null!;
    public virtual Airport OriginNavigation { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; }
}
