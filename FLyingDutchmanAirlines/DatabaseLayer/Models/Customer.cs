using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    public partial class Customer
    {
        public Customer() => Bookings = new HashSet<Booking>();

        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
