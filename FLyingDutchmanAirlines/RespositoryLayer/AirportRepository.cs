using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class AirportRepository
{
    private readonly FlyingDutchmanAirlinesContext? _context;

    /// <summary>
    /// Create a new airport repository without injecting a context. This should only be used during testing.
    /// </summary>
    /// <exception cref="InvalidOperationException">This constructor was called from the production assembly.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public AirportRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new InvalidOperationException("This constructor should only be used for testing.");
        }
    }

    /// <summary>
    /// Create a new airport repository.
    /// </summary>
    /// <param name="context">The dastabase context to use for the airports.</param>
    public AirportRepository(FlyingDutchmanAirlinesContext context) => _context = context;

    /// <summary>
    /// Return the airport with the given id.
    /// </summary>
    /// <param name="airportId">The airport id to return.</param>
    /// <returns>The airport with the given id.</returns>
    /// <exception cref="AirportNotFoundException">The <paramref name="airportId"/> is invalid or not found.</exception>
    public virtual async Task<Airport> GetAirportByIdAsync(int airportId)
    {
        if (airportId.IsNegative())
        {
            Console.WriteLine($"Argument out of range exception: {nameof(airportId)} = {airportId}");
            throw new ArgumentOutOfRangeException(nameof(airportId));
        }

        Debug.Assert(_context is not null);
        Airport? airport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId);
        if (airport is null)
        {
            Console.WriteLine($"Airport not found exception: {nameof(airportId)} = {airportId}");
            throw new AirportNotFoundException();
        }

        return airport;
    }
}
