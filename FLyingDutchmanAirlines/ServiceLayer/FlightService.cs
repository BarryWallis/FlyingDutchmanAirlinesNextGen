using System.Globalization;

using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class FlightService
{
    private readonly FlightRepository _flightRepository;
    private readonly AirportRepository _airportRepository;

    public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
    {
        _flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));
        _airportRepository = airportRepository ?? throw new ArgumentNullException(nameof(airportRepository));
    }

    /// <summary>
    /// Get a queue of all the flights in the database.
    /// </summary>
    /// <returns>The queue of all the flights.</returns>
    /// <exception cref="FlightNotFoundException">A flight in the database has no corresponding information.</exception>
    /// <exception cref="ArgumentException">An unexpected error was encountered.</exception>
    public async IAsyncEnumerable<FlightView> GetFlightsAsync()
    {
        Queue<Flight> flights = await _flightRepository.GetFlightsAsync();
        foreach (Flight flight in flights)
        {
            Airport originAirport;
            AirportInfo originAirportInfo;

            Airport destinationAirport;
            AirportInfo destinationAirportInfo;

            try
            {
                originAirport = await _airportRepository.GetAirportByIdAsync(flight.Origin);
                originAirportInfo = new AirportInfo() { City = originAirport.City, Code = originAirport.Iata };
                destinationAirport = await _airportRepository.GetAirportByIdAsync(flight.Destination);
                destinationAirportInfo = new AirportInfo() { City = destinationAirport.City, Code = destinationAirport.Iata };
            }
            catch (FlightNotFoundException)
            {
                throw new FlightNotFoundException();
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }

            yield return new FlightView(flight.FlightNumber.ToString(CultureInfo.CurrentCulture), originAirportInfo, destinationAirportInfo);
        }
    }

    /// <summary>
    /// Get specific flight information using its flight number.
    /// </summary>
    /// <param name="flightNumber">The flight number to use.</param>
    /// <returns></returns>
    /// <exception cref="FlightNotFoundException">The flight number was not in the database.</exception>
    /// <exception cref="ArgumentException">There was an unexpected error.</exception>
    public virtual async Task<FlightView> GetFlightByFlightNumberAsync(int flightNumber)
    {
        try
        {
            Flight flight = await _flightRepository.GetFlightByFlightNumberAsync(flightNumber);
            Airport originAirport = await _airportRepository.GetAirportByIdAsync(flight.Origin);
            Airport destinationAirport = await _airportRepository.GetAirportByIdAsync(flight.Destination);

            return new FlightView(flight.FlightNumber.ToString(CultureInfo.CurrentCulture),
                                  new AirportInfo(originAirport.City, originAirport.Iata),
                                  new AirportInfo(destinationAirport.City, destinationAirport.Iata));
        }
        catch (FlightNotFoundException)
        {
            throw new FlightNotFoundException();
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid argument", nameof(flightNumber));
        }
    }
}
