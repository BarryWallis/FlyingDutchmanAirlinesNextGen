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

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class BookingRepository
{
    private readonly FlyingDutchmanAirlinesContext? _context;

    /// <summary>
    /// Create a new booking repository without injecting a context. This should only be used during testing.
    /// </summary>
    /// <exception cref="InvalidOperationException">This constructor was called from the production assembly.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public BookingRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new InvalidOperationException("This constructor should only be used for testing.");
        }
    }

    /// <summary>
    /// Create a new booking repository.
    /// </summary>
    /// <param name="context">The dastabase context to use for the bookings.</param>
    public BookingRepository(FlyingDutchmanAirlinesContext context) => _context = context;

    /// <summary>
    /// Create a new booking.
    /// </summary>
    /// <param name="customerId">The customer id for the booking.</param>
    /// <param name="flightNumber">The flight number for the booking.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Custimer id or flight number are negative.</exception>
    /// <exception cref="CouldNotAddBookingToDatabaseException">Unable to add booking to database.</exception>
    public virtual async Task CreateBookingAsync(int customerId, int flightNumber)
    {
        if (customerId.IsNegative())
        {
            Console.WriteLine($"{nameof(customerId)} out of range in {nameof(CreateBookingAsync)}: {customerId}");
            throw new ArgumentOutOfRangeException(nameof(customerId), "Must be non-negtive.");
        }
        if (flightNumber.IsNegative())
        {
            Console.WriteLine($"{nameof(flightNumber)} out of range in {nameof(CreateBookingAsync)}: {flightNumber}");
            throw new ArgumentOutOfRangeException(nameof(flightNumber), "Must be non-negative.");
        }

        Booking booking = new() { CustomerId = customerId, FlightNumber = flightNumber };

        try
        {
            Debug.Assert(_context is not null);
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
