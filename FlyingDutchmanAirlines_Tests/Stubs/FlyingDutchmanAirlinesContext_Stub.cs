using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;

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
        IEnumerable<Booking>? bookings = ChangeTracker.Entries()
                                                      .Where(e => e.State == EntityState.Added)
                                                      .Select(e => e.Entity)
                                                      .OfType<Booking>();
#pragma warning disable CA2201 // Do not raise reserved exception types
        if (bookings.Any(b=> b.CustomerId != 1))
        {
            throw new Exception("Database Error!");
        }
#pragma warning restore CA2201 // Do not raise reserved exception types

        return await base.SaveChangesAsync(cancellationToken);
    }
}
