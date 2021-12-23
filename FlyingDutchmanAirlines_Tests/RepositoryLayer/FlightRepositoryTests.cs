using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class FlightRepositoryTests : IDisposable
{
    private const int FlightNumber = 1;
    private const int OriginAirportId = 1;
    private const int DestinationAirportId = 2;

    private FlyingDutchmanAirlinesContext? _context;
    private FlightRepository? _repository;
    private bool _disposedValue;

    [TestInitialize]
    public async Task TestInitializeAsync()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions
            = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

        Flight flight = new() { FlightNumber = FlightNumber, Origin = OriginAirportId, Destination = DestinationAirportId };
        _ = _context.Flights.Add(flight);
        int numberOfChanges = await _context.SaveChangesAsync();
        Assert.AreEqual(1, numberOfChanges);

        _repository = new(_context);
        Assert.IsNotNull(_repository);
    }

    [TestMethod]
    public async Task GetFlightByFlightNumber_SuccessAsync()
    {
        Debug.Assert(_repository is not null);
        Flight flight = await _repository.GetFlightByFlightNumberAsync(FlightNumber);
        Assert.IsNotNull(flight);

        Debug.Assert(_context is not null);
        Flight? databaseFlight = _context.Flights.FirstOrDefault(f=> f.FlightNumber == FlightNumber);
        Assert.IsNotNull(databaseFlight);

        Assert.AreEqual(databaseFlight.FlightNumber, flight.FlightNumber);
        Assert.AreEqual(databaseFlight.Origin, flight.Origin);
        Assert.AreEqual(databaseFlight.Destination, flight.Destination);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task GetFlightByFlightNumber_Failure_InvalidFlightNumberAsync()
    {
        StringWriter output = new();
        try
        {
            Console.SetOut(output);
            Debug.Assert(_repository is not null);
            _ = await _repository.GetFlightByFlightNumberAsync(-1);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.IsTrue(output.ToString().Contains("flightNumber out of range in GetFlightByFlightNumberAsync: -1"));
            throw;
        }
    }

    //[TestMethod]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public async Task GetFlightByFlightNumber_Failure_InvalidOriginaAirportIdAsync()
    //{
    //    StringWriter output = new();
    //    try
    //    {
    //        Console.SetOut(output);
    //        Debug.Assert(_repository is not null);
    //        _ = await _repository.GetFlightByFlightNumberAsync(0);
    //    }
    //    catch (ArgumentOutOfRangeException)
    //    {
    //        Assert.IsTrue(output.ToString().Contains("originAirportId out of range in GetFlightByFlightNumberAsync: -1"));
    //        throw;
    //    }
    //}

    //[TestMethod]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    //public async Task GetFlightByFlightNumber_Failure_InvalidDestinationAirportIdAsync()
    //{
    //    StringWriter output = new();
    //    try
    //    {
    //        Console.SetOut(output);
    //        Debug.Assert(_repository is not null);
    //        _ = await _repository.GetFlightByFlightNumberAsync(0);
    //    }
    //    catch (ArgumentOutOfRangeException)
    //    {
    //        Assert.IsTrue(output.ToString().Contains("destinationAirportId out of range in GetFlightByFlightNumberAsync: -1"));
    //        throw;
    //    }
    //}

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlightByFlightNumber_Failure_FlightNotFoundExceptionAsync()
    {
        Debug.Assert(_repository is not null);
        _ = await _repository.GetFlightByFlightNumberAsync(2);
    }

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Debug.Assert(_context is not null);
            _context.Dispose();
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BookingRepositoryTests()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
