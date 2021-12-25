using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer;

[TestClass]
public class FlightServiceTests
{
    private const int BogusFlightNumber = -1;
    private const int FlightNumber = 148;
    private const int OriginAirportId = 31;
    private const int DestinationAirportId = 92;
    private const string OriginCity = "Mexico City";
    private const string OriginCode = "MEX";
    private const string DestinationCity = "Ulaanbaataar";
    private const string DestinationCode = "UBN";

    private readonly Flight _flightInDatabase = new()
    {
        FlightNumber = FlightNumber,
        Origin = OriginAirportId,
        Destination = DestinationAirportId
    };


    private Mock<AirportRepository>? _mockAirportRepository;
    private Mock<FlightRepository>? _mockFlightRepository;
    private FlightService? _flightService;

    [TestInitialize]
    public void TestInitializeAsync()
    {
        _mockAirportRepository = new();
        _mockFlightRepository = new();
        _flightService = new(_mockFlightRepository.Object, _mockAirportRepository.Object);

        _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(FlightNumber))
                             .Returns(Task.FromResult(_flightInDatabase));
        Debug.Assert(_mockAirportRepository is not null);
        _ = _mockAirportRepository.Setup(ar => ar.GetAirportByIdAsync(OriginAirportId))
                              .ReturnsAsync(new Airport { AirportId = OriginAirportId, City = OriginCity, Iata = OriginCode });
        _ = _mockAirportRepository.Setup(ar => ar.GetAirportByIdAsync(DestinationAirportId))
                              .ReturnsAsync(new Airport { AirportId = DestinationAirportId, City = DestinationCity, Iata = DestinationCode });

        Queue<Flight> mockReturn = new(1);
        mockReturn.Enqueue(_flightInDatabase);
        _ = _mockFlightRepository.Setup(fr => fr.GetFlightsAsync()).ReturnsAsync(mockReturn);
        _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(FlightNumber))
            .Returns(Task.FromResult(_flightInDatabase));
    }

    [TestMethod]
    public async Task GetFlights_SuccessAsync()
    {
        Debug.Assert(_flightService is not null);
        await foreach (FlightView flightView in _flightService.GetFlightsAsync())
        {
            Assert.AreEqual(flightView.FlightNumber, FlightNumber.ToString(CultureInfo.CurrentCulture));
            Assert.AreEqual(flightView.Origin.City, OriginCity);
            Assert.AreEqual(flightView.Origin.Code, OriginCode);
            Assert.AreEqual(flightView.Destination.City, DestinationCity);
            Assert.AreEqual(flightView.Destination.Code, DestinationCode);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlights_Failure_RepositgoryExceptionAsync()
    {
        Debug.Assert(_mockAirportRepository is not null);
        _ = _mockAirportRepository.Setup(repository => repository.GetAirportByIdAsync(OriginAirportId))
                              .ThrowsAsync(new FlightNotFoundException());

        Debug.Assert(_flightService is not null);
        await foreach (FlightView _ in _flightService.GetFlightsAsync())
        {
            ;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlightBFlightNumber_Failure_RepositoryException_FlightNotFoundExceptionAsync()
    {
        Debug.Assert(_mockFlightRepository is not null);
        _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(-BogusFlightNumber))
                                 .Throws(new FlightNotFoundException());

        Debug.Assert(_flightService is not null);
        _ = await _flightService.GetFlightByFlightNumberAsync(-BogusFlightNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetFlightBFlightNumber_Failure_RepositoryException_ExceptionAsync()
    {
        Debug.Assert(_mockFlightRepository is not null);
        _ = _mockFlightRepository.Setup(fr => fr.GetFlightByFlightNumberAsync(BogusFlightNumber))
                                 .Throws(new OverflowException());

        Debug.Assert(_flightService is not null);
        _ = await _flightService.GetFlightByFlightNumberAsync(BogusFlightNumber);
    }
}
