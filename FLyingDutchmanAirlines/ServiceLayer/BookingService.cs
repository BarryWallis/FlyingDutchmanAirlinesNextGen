using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RespositoryLayer;

namespace FlyingDutchmanAirlines.ServiceLayer
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly CustomerRepository _customerRepository;

        /// <summary>
        /// Create a new booking service.
        /// </summary>
        /// <param name="bookingRepository">The booking repository to use for bookings.</param>
        /// <param name="customerRepository">The customr repository to use for bookings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bookingRepository"/> or <paramref name="customerRepository"/> 
        /// is not null.</exception>
        public BookingService(BookingRepository bookingRepository, CustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        /// <summary>
        /// Create a new booking. Will add customer if needed.
        /// </summary>
        /// <param name="customerName">The name of the customer making the booking. It will be added if there is no customer record.</param>
        /// <param name="flightNumber">Th flight number of the booking.</param>
        /// <returns><see langword="null"/> on success; otherwise returns the exception.</returns>
        public async Task<Exception?> CreateBookingAsync(string customerName, int flightNumber)
        {
            try
            {
                Customer customer;
                try
                {
                    customer = await _customerRepository.GetCustomerByNameAsync(customerName);
                }
                catch
                {
                    bool isSuccess = await _customerRepository.CreateCustomerAsync(customerName);
                    Debug.Assert(isSuccess);
                    return await CreateBookingAsync(customerName, flightNumber);
                }

                await _bookingRepository.CreateBookingAsync(customer.CustomerId, flightNumber);
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
