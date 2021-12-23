using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService
{
    private readonly BookingRepository _bookingRepository;
    private readonly FlightRepository _flightRepository;
    private readonly CustomerRepository _customerRepository;

    /// <summary>
    /// Create a new booking service.
    /// </summary>
    /// <param name="bookingRepository">The booking repository to use for bookings.</param>
    /// <param name="customerRepository">The customr repository to use for bookings.</param>
    /// <exception cref="ArgumentNullException"><paramref name="bookingRepository"/> or <paramref name="customerRepository"/> 
    /// is not null.</exception>
    public BookingService(BookingRepository bookingRepository, FlightRepository flightRepository, CustomerRepository customerRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    /// <summary>
    /// Create a new booking. Will add customer if needed.
    /// </summary>
    /// <param name="customerName">The name of the customer making the booking. It will be added if there is no customer record.</param>
    /// <param name="flightNumber">Th flight number of the booking.</param>
    /// <returns><see langword="null"/> on success; otherwise returns the exception.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="customerName"/> must not be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="customerName"/> is required. </exception>
    /// <exception cref="ArgumentOutOfRangeException"><param</exception>
    public async Task<Exception?> CreateBookingAsync(string customerName, int flightNumber)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            return customerName is null ? new ArgumentNullException(nameof(customerName))
                                        : new ArgumentException("Is required.", nameof(customerName));
        }

        if (flightNumber.IsNegative())
        {
            return new ArgumentOutOfRangeException(nameof(flightNumber), "Must not be negative.");
        }

        try
        {
            Customer customer = await GetCustomerFromDatabaseAsync(customerName) ?? await AddCustomerToDatabaseAsync(customerName);
            if (!await FlightExistsInDatabaseAsync(flightNumber))
            {
                throw new CouldNotAddBookingToDatabaseException();
            }

            await _bookingRepository.CreateBookingAsync(customer.CustomerId, flightNumber);
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private async Task<bool> FlightExistsInDatabaseAsync(int flightNumber)
    {
        try
        {
            return await _flightRepository.GetFlightByFlightNumberAsync(flightNumber) is not null;
        }
        catch (FlightNotFoundException)
        {
            return false;
        }
    }

    private async Task<Customer?> GetCustomerFromDatabaseAsync(string name)
    {
        try
        {
            return await _customerRepository.GetCustomerByNameAsync(name);
        }
        catch (CustomerNotFoundException)
        {
            return null;
        }
        catch (Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException ?? exception).Throw();
            return null;
        }
    }

    private async Task<Customer> AddCustomerToDatabaseAsync(string name)
    {
        bool isSuccess = await _customerRepository.CreateCustomerAsync(name);
        Debug.Assert(isSuccess);
        return await _customerRepository.GetCustomerByNameAsync(name);
    }
}
