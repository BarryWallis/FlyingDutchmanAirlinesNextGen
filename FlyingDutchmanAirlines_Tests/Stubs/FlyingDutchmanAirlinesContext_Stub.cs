using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs;

internal class FlyingDutchmanAirlinesContext_Stub : FlyingDutchmanAirlinesContext
{
    /// <summary>
    /// Create a new stub.
    /// </summary>
    /// <param name="options">The database options.</param>
    public FlyingDutchmanAirlinesContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        => base.Database.EnsureDeleted();

    /// <inheritdoc/>
    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry>? pendingChanges = ChangeTracker.Entries()
                                                                .Where(e => e.State == EntityState.Added);
        IEnumerable<Booking>? bookings = pendingChanges.Select(e => e.Entity)
                                                       .OfType<Booking>();
#pragma warning disable CA2201 // Do not raise reserved exception types
        if (bookings.Any(b => b.CustomerId != 1))
        {
            throw new Exception("Database Error!");
        }

        IEnumerable<Airport> airports = pendingChanges.Select(e=>e.Entity).OfType<Airport>();
        return airports.Any(a =>a.AirportId == 10) ? throw new Exception("Database Error!") : await base.SaveChangesAsync(cancellationToken);
#pragma warning restore CA2201 // Do not raise reserved exception types
    }
}
