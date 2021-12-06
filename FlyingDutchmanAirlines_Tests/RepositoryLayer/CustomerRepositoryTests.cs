using System;
using System.Diagnostics;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class CustomerRepositoryTests : IDisposable
{
    private const string TestCustomerName = "Linus Torvalds";

    private FlyingDutchmanAirlinesContext? _context;
    private CustomerRepository? _repository;
    private bool _disposedValue;

    [TestInitialize]
    public async Task TestInitialize()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
            new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new(dbContextOptions);
        Debug.Assert(_context is not null);

        Customer customer = new(TestCustomerName);
        _ = _context.Customers.Add(customer);
        _ = await _context.SaveChangesAsync();

        _repository = new(_context);
        Assert.IsNotNull(_repository);
    }

    [TestMethod]
    public async Task CreateCustomer_SuccessAsync()
    {
        Debug.Assert(_repository is not null);
        bool result = await _repository.CreateCustomerAsync("Donald Knuth");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_NameIsEmptyAsync()
    {
        Debug.Assert(_repository is not null);
        bool result = await _repository.CreateCustomerAsync(string.Empty);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_NameIsNull()
    {
        Debug.Assert(_repository is not null);
        bool result = await _repository.CreateCustomerAsync(null!);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow('#')]
    [DataRow('$')]
    [DataRow('%')]
    [DataRow('&')]
    [DataRow('*')]
    public async Task CreateCustomer_Failure_NameContainsInvalidCharactersAsync(char invalidCharacter)
    {
        Debug.Assert(_repository is not null);
        bool result = await _repository.CreateCustomerAsync($"Donald Knuth{invalidCharacter}");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_DatabaseAccessErrorAsync()
    {
        Debug.Assert(_repository is not null);
        bool result = await _repository.CreateCustomerAsync(null!);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetCustomerByName_SuccessAsync()
    {
        Debug.Assert(_repository is not null);
        Customer customer = await _repository.GetCustomerByName(TestCustomerName);
        Assert.IsNotNull(customer);

        Debug.Assert(_context is not null);
        Customer databaseCustomer = await _context.Customers.FirstAsync();
        Assert.AreEqual(customer, databaseCustomer);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("#")]
    [DataRow("$")]
    [DataRow("%")]
    [DataRow("&")]
    [DataRow("*")]
    [ExpectedException(typeof(CustomerNotFoundException))]
    public async Task GetCustomerByName_Failure_InvalidName(string name)
    {
        Debug.Assert(_repository is not null);
        _ = await _repository.GetCustomerByName(name);
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
    // ~CustomerRepositoryTests()
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
