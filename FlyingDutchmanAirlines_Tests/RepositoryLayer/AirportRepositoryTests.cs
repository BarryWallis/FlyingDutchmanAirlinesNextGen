using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;

using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class AirportRepositoryTests : IDisposable
    {
        const int AirportId = 0;
        const string City = "Nuuk";
        const string Iata = "GOH";

        private FlyingDutchmanAirlinesContext? _context;
        private AirportRepository? _repository;
        private bool _disposedValue;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions
                = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

            SortedList<string, Airport> airports = new()
            {
                {"GOH", new Airport { AirportId = AirportId, City = City, Iata = Iata } },
                {"PHX", new Airport { AirportId = 1, City = "Phoenix", Iata = "PHX" } },
                {"DDH", new Airport { AirportId = 2, City = "Bennington", Iata = "DDH" } },
                {"RDU", new Airport { AirportId = 3, City = "Raleigh-Durham", Iata = "RDU" } }
            };

            _context.Airports.AddRange(airports.Values);
            int numberOfChanges = await _context.SaveChangesAsync();
            Assert.AreEqual(4, numberOfChanges);

            _repository = new(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task GetAirportById_SuccessAsync(int airportId)
        {
            Debug.Assert(_repository is not null);
            Airport airport = await _repository.GetAirportByIdAsync(airportId);
            Assert.IsNotNull(airport);

            Debug.Assert(_context is not null);
            Airport databaseAirport = _context.Airports.First(a => a.AirportId == airportId);
            Assert.AreEqual(databaseAirport.AirportId, airport.AirportId);
            Assert.AreEqual(databaseAirport.City, airport.City);
            Assert.AreEqual(databaseAirport.Iata, airport.Iata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task GetAirportById_Failure_InvalidAirportIdAsync()
        {
            StringWriter outputStream = new();
            try
            {
                Console.SetOut(outputStream);

                Debug.Assert(_repository is not null);
                _ = await _repository.GetAirportByIdAsync(-1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.IsTrue(outputStream.ToString().Contains("Argument out of range exception: airportId = -1"));
                throw;
            }
            finally
            {
                outputStream.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportById_Failure_DatabaseExceptionAsync()
        {
            Debug.Assert(_repository is not null);
            _ = await _repository.GetAirportByIdAsync(10);
        }

        #region Dispose methods
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Debug.Assert(_context is not null);
                _context.Dispose();
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AirportRepositoryTests()
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
}
