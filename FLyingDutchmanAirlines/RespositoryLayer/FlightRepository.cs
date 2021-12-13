using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class FlightRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

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
    /// <param name="originAirportId">The origin airport ID of the flight.</param>
    /// <param name="destinationAirportId">The destination airport ID of the flight.</param>
    /// <returns>The flight.</returns>
    public async Task<Flight> GetFlightByFlightNumberAsync(int flightNumber, int originAirportId, int destinationAirportId)
    {
        if (flightNumber.IsNegative())
        {
            Console.WriteLine($"{nameof(flightNumber)} out of range in {nameof(GetFlightByFlightNumberAsync)}: {flightNumber}");
            throw new ArgumentOutOfRangeException(nameof(flightNumber));
        }
        if (originAirportId.IsNegative())
        {
            Console.WriteLine($"{nameof(originAirportId)} out of range in {nameof(GetFlightByFlightNumberAsync)}: {originAirportId}");
            throw new ArgumentOutOfRangeException(nameof(originAirportId));
        }
        if (destinationAirportId.IsNegative())
        {
            Console.WriteLine($"{nameof(destinationAirportId)} out of range in {nameof(GetFlightByFlightNumberAsync)}: " +
                $"{destinationAirportId}");
            throw new ArgumentOutOfRangeException(nameof(destinationAirportId));
        }

        return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ?? throw new FlightNotFoundException();
    }
}