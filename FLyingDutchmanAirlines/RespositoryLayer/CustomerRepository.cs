using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RespositoryLayer;

public class CustomerRepository
{
    private readonly FlyingDutchmanAirlinesContext? _context;

    /// <summary>
    /// Create a new customer repository without injecting a context. This should only be used during testing.
    /// </summary>
    /// <exception cref="InvalidOperationException">This constructor was called from the production assembly.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public CustomerRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new InvalidOperationException("This constructor should only be used for testing.");
        }
    }

    /// <summary>
    /// Create a new customer repository.
    /// </summary>
    /// <param name="context">The database context to use for the cusotmers.</param>
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
                Debug.Assert(_context is not null);
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
    public virtual async Task<Customer> GetCustomerByNameAsync(string name)
    {
        Debug.Assert(_context is not null); 
        return IsInvalidCustomerName(name)
                   ? throw new CustomerNotFoundException()
                   : await _context.Customers.FirstOrDefaultAsync(c => c.Name == name) ?? throw new CustomerNotFoundException();
    }
}
