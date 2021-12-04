using FlyingDutchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.DatabaseLayer
{
    public partial class FlyingDutchmanAirlinesContext : DbContext
    {
        public FlyingDutchmanAirlinesContext()
        {
        }

        public FlyingDutchmanAirlinesContext(DbContextOptions<FlyingDutchmanAirlinesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airport> Airports { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Flight> Flights { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Environment.GetEnvironmentVariable("FlyingDutchmanAirlines_Connection_String") ?? string.Empty;
                _ = optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Airport>(entity =>
              {
                  _ = entity.ToTable("Airport");

                  _ = entity.Property(e => e.AirportId)
                      .ValueGeneratedNever()
                      .HasColumnName("AirportID");

                  _ = entity.Property(e => e.City)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                  _ = entity.Property(e => e.Iata)
                      .HasMaxLength(3)
                      .IsUnicode(false)
                      .HasColumnName("IATA");
              });

            _ = modelBuilder.Entity<Booking>(entity =>
              {
                  _ = entity.ToTable("Booking");

                  _ = entity.Property(e => e.BookingId).HasColumnName("BookingID");

                  _ = entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                  _ = entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(d => d.CustomerId)
                      .HasConstraintName("FK__Booking__Custome__71D1E811");

                  _ = entity.HasOne(d => d.FlightNumberNavigation)
                      .WithMany(p => p.Bookings)
                      .HasForeignKey(d => d.FlightNumber)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__Booking__FlightN__4F7CD00D");
              });

            _ = modelBuilder.Entity<Customer>(entity =>
              {
                  _ = entity.ToTable("Customer");

                  _ = entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                  _ = entity.Property(e => e.Name)
                      .HasMaxLength(50)
                      .IsUnicode(false);
              });

            _ = modelBuilder.Entity<Flight>(entity =>
              {
                  _ = entity.HasKey(e => e.FlightNumber);

                  _ = entity.ToTable("Flight");

                  _ = entity.Property(e => e.FlightNumber).ValueGeneratedNever();

                  _ = entity.HasOne(d => d.DestinationNavigation)
                      .WithMany(p => p.FlightDestinationNavigations)
                      .HasForeignKey(d => d.Destination)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Flight_AirportDestination");

                  _ = entity.HasOne(d => d.OriginNavigation)
                      .WithMany(p => p.FlightOriginNavigations)
                      .HasForeignKey(d => d.Origin)
                      .OnDelete(DeleteBehavior.ClientSetNull);
              });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
