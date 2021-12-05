using System.Diagnostics;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RespositoryLayer;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        private FlyingDutchmanAirlinesContext? _context;
        private CustomerRepository? _customerRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
                new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new(dbContextOptions);
            Debug.Assert(_context is not null);

            _customerRepository = new(_context);
            Assert.IsNotNull(_customerRepository);
        }

        [TestMethod]
        public async Task CreateCustomer_SuccessAsync()
        {
            Debug.Assert(_customerRepository is not null);
            bool result = await _customerRepository.CreateCustomerAsync("Donald Knuth");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsEmptyAsync()
        {
            Debug.Assert(_customerRepository is not null);
            bool result = await _customerRepository.CreateCustomerAsync(string.Empty);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsNull()
        {
            Debug.Assert(_customerRepository is not null);
            bool result = await _customerRepository.CreateCustomerAsync(null!);
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
            Debug.Assert(_customerRepository is not null);
            bool result = await _customerRepository.CreateCustomerAsync($"Donald Knuth{invalidCharacter}");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_DatabaseAccessErrorAsync()
        {
            Debug.Assert(_customerRepository is not null);
            bool result = await _customerRepository.CreateCustomerAsync(null!);
            Assert.IsFalse(result);
        }
    }
}