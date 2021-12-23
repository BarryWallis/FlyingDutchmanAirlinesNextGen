using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
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
        private const string LeoTolstoy = "Leo Tolstoy";
        private const string GalileoGalilei = "Galileo Galilei";
        //private const string EiseEisinga = "Eise Eisinga";
        private const string MauritsEscher = "Maurits Escher";
        private const string KonradZuse = "Konrad Zuse";
        private const string BillGates = "Bill Gates";

        private Mock<BookingRepository>? _mockBookingRepository;
        private Mock<FlightRepository>? _mockFlightRepository;
        private Mock<CustomerRepository>? _mockCustomerRepository;
        private BookingService? _bookingService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingRepository = new();
            _mockFlightRepository = new();
            _mockCustomerRepository = new();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockFlightRepository.Object,
                                                 _mockCustomerRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BookingServiceConstructor_ArgumentNullException_BookingRepository()
        {
            Debug.Assert(_mockCustomerRepository is not null);
            Debug.Assert(_mockFlightRepository is not null);
            _ = new BookingService(null!, _mockFlightRepository.Object, _mockCustomerRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BookingServiceConstructor_ArgumentNullException_CustomerRepository()
        {
            Debug.Assert(_mockBookingRepository is not null);
            Debug.Assert(_mockFlightRepository is not null);
            _ = new BookingService(_mockBookingRepository.Object, _mockFlightRepository.Object, null!);
        }

        [TestMethod]
        public async Task CreateBooking_Success_CustomrInDatabaseAsync()
        {
            Debug.Assert(_mockBookingRepository is not null);
            _ = _mockBookingRepository.Setup(br => br.CreateBookingAsync(0, 0))
                                      .Returns(Task.CompletedTask);
            Debug.Assert(_mockFlightRepository is not null);
            _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(0)).ReturnsAsync(new Flight());
            Debug.Assert(_mockCustomerRepository is not null);
            _ = _mockCustomerRepository.Setup(cr => cr.GetCustomerByNameAsync(LeoTolstoy))
                                       .Returns(Task.FromResult(new Customer(LeoTolstoy)));
            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(LeoTolstoy, 0);
            Assert.IsNull(exception);
        }

        [TestMethod]
        public async Task CreateBooking_Success_CustomerNotInDatabaseAsync()
        {
            Debug.Assert(_mockBookingRepository is not null);
            _ = _mockBookingRepository.Setup(br => br.CreateBookingAsync(0, 0)).Returns(Task.CompletedTask);
            Debug.Assert(_mockCustomerRepository is not null);
            _ = _mockCustomerRepository.Setup(cr => cr.GetCustomerByNameAsync(KonradZuse)).Throws(new CustomerNotFoundException());
            _ = _mockCustomerRepository.Setup(cr => cr.CreateCustomerAsync(KonradZuse)).ReturnsAsync(true);

            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(KonradZuse, 0);

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CustomerNotFoundException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_EmptyCustomerNameAsync()
        {
            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(" ", 0);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_NullCustomerNameAsync()
        {
            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(null!, 0);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_NegativeFlightNumberAsync()
        {
            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(LeoTolstoy, -1);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_RepositoryExceptionAsync()
        {
            Debug.Assert(_mockBookingRepository is not null);
            _ = _mockBookingRepository.Setup(br => br.CreateBookingAsync(0, 1)).Throws(new ArgumentOutOfRangeException());
            _ = _mockBookingRepository.Setup(br => br.CreateBookingAsync(1, 2)).Throws(new CouldNotAddBookingToDatabaseException());

            Debug.Assert(_mockCustomerRepository is not null);
            _ = _mockCustomerRepository.Setup(cr=>cr.GetCustomerByNameAsync(GalileoGalilei)).Returns(Task.FromResult(new Customer(GalileoGalilei) { CustomerId=0}));

            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(GalileoGalilei, 1);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof (CouldNotAddBookingToDatabaseException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_FlightNotInDatabaseAsync()
        {
            Debug.Assert(_mockFlightRepository is not null);
            _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(-1))
                                     .Throws(new FlightNotFoundException());

            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(MauritsEscher, -1);

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_CustomerNotInDatabase_RepositoryFailureAsync()
        {
            Debug.Assert(_mockBookingRepository is not null);
            _ = _mockBookingRepository.Setup(br => br.CreateBookingAsync(0, 0))
                                      .Throws(new CouldNotAddBookingToDatabaseException());
            Debug.Assert(_mockFlightRepository is not null);
            _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(0))
                                     .ReturnsAsync(new Flight());
            Debug.Assert(_mockCustomerRepository is not null);
            _ = _mockCustomerRepository.Setup(cr => cr.GetCustomerByNameAsync(BillGates))
                                       .Returns(Task.FromResult(new Customer(BillGates)));

            Debug.Assert(_bookingService is not null);
            Exception? exception = await _bookingService.CreateBookingAsync(BillGates, 0);

            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
        }
    }
}
