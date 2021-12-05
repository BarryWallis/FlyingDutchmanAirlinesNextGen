using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RespositoryLayer
{
    public class CustomerRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        /// <summary>
        /// Create a new customer repository.
        /// </summary>
        /// <param name="context">The database context to use for this repository.</param>
        public CustomerRepository(FlyingDutchmanAirlinesContext context) => _context = context;

        /// <summary>
        /// Create a new customer.
        /// </summary>
        /// <param name="name">The customer name.</param>
        /// <returns><see langword="true"/> on success; <see langword="false"/> on failure.</returns>
        public async Task<bool> CreateCustomerAsync(string name)
        {
            if (IsInvalidCustomerName(name))
            {
                return false;
            }

            try
            {
                Customer customer = new(name);
                using (_context)
                {
                    _ = _context.Customers.Add(customer);
                    _ = await _context.SaveChangesAsync();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static bool IsInvalidCustomerName(string name)
        {
            char[] forbiddenCharacters = { '!', '@', '#', '$', '%', '&', '*' };
            return string.IsNullOrEmpty(name) || name.Any(c => forbiddenCharacters.Contains(c));
        }

        /// <summary>
        /// Return the customer with the given name.
        /// </summary>
        /// <param name="name">The custromer name to return.</param>
        /// <returns>The customer with the given name.</returns>
        /// <exception cref="CustomerNotFoundException"><paramref name="name"/> was invalid or not found.</exception>
        public async Task<Customer> GetCustomerByName(string name)
            => IsInvalidCustomerName(name)
                ? throw new CustomerNotFoundException()
                : await _context.Customers.FirstOrDefaultAsync(c => c.Name == name) ?? throw new CustomerNotFoundException();
    }
}
