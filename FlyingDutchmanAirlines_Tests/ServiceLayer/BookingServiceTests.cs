using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;

using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class BookingServiceTests 
    {
        private const string Name = "Leo Tolstoy";

        [TestMethod]
        public async Task CreateBooking_SuccessAsync()
        {
            Mock<BookingRepository> mockBookingRepository = new();
            Mock<CustomerRepository> mockCustomerRepository = new();

            _ = mockBookingRepository.Setup(br => br.CreateBookingAsync(0, 0)).Returns(Task.CompletedTask);
            _ = mockCustomerRepository.Setup(cr => cr.GetCustomerByNameAsync(Name))
                                      .Returns(Task.FromResult(new Customer(Name)));

            BookingRepository bookingRepository = mockBookingRepository.Object;
            CustomerRepository customerRepository = mockCustomerRepository.Object;
            BookingService bookingService = new(bookingRepository, customerRepository);
            Exception? exception = await bookingService.CreateBookingAsync(Name, 0);
            Assert.IsNull(exception);
        }
    }
}
