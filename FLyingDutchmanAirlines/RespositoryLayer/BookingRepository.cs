using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class BookingRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    /// <summary>
    /// Create a new booking repository.
    /// </summary>
    /// <param name="context">The dastabase context to use for the bookings.</param>
    /// <exception cref="CouldNotAddBookingToDatabaseException">Unable to add booking to database.</exception>
    public BookingRepository(FlyingDutchmanAirlinesContext context) => _context = context;

    public async Task CreateBooking(int customerId, int flightNumber)
    {
        if (customerId < 0)
        {
            Console.WriteLine($"{nameof(customerId)} out of range in {nameof(CreateBooking)}: {customerId}");
            throw new ArgumentOutOfRangeException(nameof(customerId), "Must be non-negtive.");
        }
        if (flightNumber < 0)
        {
            Console.WriteLine($"{nameof(flightNumber)} out of range in {nameof(CreateBooking)}: {flightNumber}");
            throw new ArgumentOutOfRangeException(nameof(flightNumber), "Must be non-negative.");
        }

        Booking booking = new() { CustomerId = customerId, FlightNumber = flightNumber };

        try
        {
            _ = _context.Bookings.Add(booking);
            _ = await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception during database write: {exception.Message}");
            throw new CouldNotAddBookingToDatabaseException();
        }
    }
}
