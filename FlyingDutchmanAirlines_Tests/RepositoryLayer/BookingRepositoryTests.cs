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

        _repository = new BookingRepository(_context);
        Assert.IsNotNull(_repository);
    }

    [TestMethod]
    public async Task CreateBooking_Success()
    {
        Debug.Assert(_repository is not null);
        await _repository.CreateBooking(1, 0);

        Debug.Assert(_context is not null);
        Booking booking = _context.Bookings.First();

        Assert.IsNotNull(booking);
        Assert.AreEqual(1, booking.CustomerId);
        Assert.AreEqual(0, booking.FlightNumber);
    }

    [TestMethod]
    [DataRow(-1, 0)]
    [DataRow(0, -1)]
    [DataRow(-1, -1)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task CreateBooking_Failure_InvalidInputs(int customerId, int flightNumber)
    {
        Debug.Assert(_repository is not null);
        await _repository.CreateBooking(customerId, flightNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(CouldNotAddBookingToDatabaseException))]
    public async Task CreateBooking_Failure_DatabaseError()
    {
        Debug.Assert(_repository is not null);
        await _repository.CreateBooking(0, 1);
    }

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
}
