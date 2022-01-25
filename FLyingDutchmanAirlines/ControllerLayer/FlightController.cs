using System.Net;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("{controller}")]
public class FlightController : Controller
{
    private readonly FlightService _flightService;

    /// <summary>
    /// Create a new flight controller.
    /// </summary>
    /// <param name="flightService">The flight service to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="flightService"/> is <see langword="null"/>.</exception>
    public FlightController(FlightService flightService) => _flightService = flightService
                                                                             ?? throw new ArgumentNullException(nameof(flightService));

    public static void ConfigureServices(IServiceCollection serviceDescriptors)
    {
        _ = serviceDescriptors.AddControllers();
        _ = serviceDescriptors.AddTransient(typeof(FlightService), typeof(FlightService));
        _ = serviceDescriptors.AddTransient(typeof(AirportRepository), typeof(AirportRepository));
        _ = serviceDescriptors.AddTransient(typeof(FlightRepository), typeof(FlightRepository));
        _ = serviceDescriptors.AddTransient(typeof(FlyingDutchmanAirlinesContext), typeof(FlyingDutchmanAirlinesContext));
    }

    /// <summary>
    /// Get a list of all flights,
    /// </summary>
    /// <returns>The appropriate HTTP status code and data.</returns>
    [HttpGet]
    public async Task<IActionResult> GetFlightsAsync()
    {
        try
        {
            Queue<FlightView> flightViews = new();
            await foreach (FlightView flightView in _flightService.GetFlightsAsync())
            {
                flightViews.Enqueue(flightView);
            }

            return StatusCode((int)HttpStatusCode.OK, flightViews);
        }
        catch (FlightNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.NotFound, "No flights were found in the database");
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error ocurred");
        }
    }

    /// <summary>
    /// Get information for a specific flight.
    /// </summary>
    /// <param name="flightNumber">The flight number of the flight to get.</param>
    /// <returns>The appropriate HTTP status code and data.</returns>
    [HttpGet("{flightNumber}")]
    public async Task<IActionResult> GetFlightByFlightNumberAsync(int flightNumber)
    {
        try
        {
            if (!flightNumber.IsPositive())
            {
#pragma warning disable CA2201 // Do not raise reserved exception types
                throw new Exception();
#pragma warning restore CA2201 // Do not raise reserved exception types
            }
            FlightView flightView = await _flightService.GetFlightByFlightNumberAsync(flightNumber);
            return StatusCode((int)HttpStatusCode.OK, flightView);
        }
        catch (FlightNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.NotFound, "The flight was not found in the database");
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, "Bad request");
        }
    }
}
