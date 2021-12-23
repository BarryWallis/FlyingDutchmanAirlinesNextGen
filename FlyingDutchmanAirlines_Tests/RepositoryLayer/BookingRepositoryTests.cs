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
public class BookingRepositoryTests : IDisposable
{
    private FlyingDutchmanAirlinesContext? _context;
    private BookingRepository? _repository;
    private bool _disposedValue;

    [TestInitialize]
    public void TestInitialize()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions 
            = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

        _repository = new(_context);
        Assert.IsNotNull(_repository);
    }

    [TestMethod]
    public async Task CreateBooking_SuccessAsync()
    {
        Debug.Assert(_repository is not null);
        await _repository.CreateBookingAsync(1, 0);

        Debug.Assert(_context is not null);
        Booking booking = _context.Bookings.First();

        Assert.IsNotNull(booking);
        Assert.AreEqual(1, booking.CustomerId);
        Assert.AreEqual(0, booking.FlightNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task CreateBooking_Failure_InvalidCustomerIdAsync()
    {
        StringWriter output = new();
        try
        {
            Console.SetOut(output);
            Debug.Assert(_repository is not null);
            await _repository.CreateBookingAsync(-1, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.IsTrue(output.ToString().Contains("customerId out of range in CreateBookingAsync: -1"));
            throw;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task CreateBooking_Failure_InvalidFlightNumberAsync()
    {
        StringWriter output = new();
        try
        {
            Console.SetOut(output);
            Debug.Assert(_repository is not null);
            await _repository.CreateBookingAsync(0, -1);
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.IsTrue(output.ToString().Contains("flightNumber out of range in CreateBookingAsync: -1"));
            throw;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(CouldNotAddBookingToDatabaseException))]
    public async Task CreateBooking_Failure_DatabaseErrorAsync()
    {
        Debug.Assert(_repository is not null);
        await _repository.CreateBookingAsync(0, 1);
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
