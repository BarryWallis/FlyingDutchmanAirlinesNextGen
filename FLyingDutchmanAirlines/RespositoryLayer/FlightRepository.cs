using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class FlightRepository
{
    private readonly FlyingDutchmanAirlinesContext? _context;

    /// <summary>
    /// Create a new flight repository without injecting a context. This should only be used during testing.
    /// </summary>
    /// <exception cref="InvalidOperationException">This constructor was called from the production assembly.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public FlightRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new InvalidOperationException("This constructor should only be used for testing.");
        }
    }

    /// <summary>
    /// Create a new flight repository.
    /// </summary>
    /// <param name="context">The database context to use for the repository.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> cannot be null.</exception>
    public FlightRepository(FlyingDutchmanAirlinesContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Get flight information from the repository.
    /// </summary>
    /// <param name="flightNumber">The flight number of the flight.</param>
    /// <returns>The flight.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="flightNumber"/> is out of range. Must be non-negative.</exception>
    /// <exception cref="FlightNotFoundException"><paramref name="flightNumber"/> was not found in database.</exception>
    public virtual async Task<Flight> GetFlightByFlightNumberAsync(int flightNumber)
    {
        if (flightNumber.IsNegative())
        {
            Console.WriteLine($"{nameof(flightNumber)} out of range in {nameof(GetFlightByFlightNumberAsync)}: {flightNumber}");
            throw new ArgumentOutOfRangeException(nameof(flightNumber));
        }

        Debug.Assert(_context is not null);
        return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ?? throw new FlightNotFoundException();
    }
}