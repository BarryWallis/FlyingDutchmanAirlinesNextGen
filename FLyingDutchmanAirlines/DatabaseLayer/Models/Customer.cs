using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public sealed class Customer : IEquatable<Customer?>
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; }

    public Customer(string name)
    {
        Bookings = new HashSet<Booking>();
        Name = name;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Customer);

    /// <inheritdoc/>
    public bool Equals(Customer? other) => other != null && CustomerId == other.CustomerId && Name == other.Name;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(CustomerId, Name);

    /// <summary>
    /// Check two customers for equality.
    /// </summary>
    /// <param name="left">The left-side customer.</param>
    /// <param name="right">The right-side customer.</param>
    /// <returns><see langword="true"/> if both objects reqpresent the same customer; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Customer? left, Customer? right) => EqualityComparer<Customer>.Default.Equals(left, right);

    /// <summary>
    /// Check two customers for inequality.
    /// </summary>
    /// <param name="left">The left-side customer.</param>
    /// <param name="right">The right-side customer.</param>
    /// <returns><see langword="true"/> if both objects reqpresent different customers; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Customer? left, Customer? right) => !(left == right);
}
