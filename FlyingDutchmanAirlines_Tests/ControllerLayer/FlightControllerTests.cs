using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer;

[TestClass]
public class FlightControllerTests
{
    [TestMethod]
    public async Task GetFlights_SuccessAsync()
    {
        Mock<FlightService> flightService = new();
        List<FlightView> flightViews = new(2)
        {
            new FlightView("1932",
                           new AirportInfo { City = "Groningen", Code = "GRQ" },
                           new AirportInfo { City = "Phoenix", Code = "PHX" }),
            new FlightView("841",
                           new AirportInfo { City = "New York City", Code = "JFK" },
                           new AirportInfo { City = "London", Code = "LHR" }),
        };

        _ = flightService.Setup(fs => fs.GetFlightsAsync()).Returns(FlightViewAsyncGeneratorAsync(flightViews));

        FlightController flightController = new(flightService.Object);
        ObjectResult? response = await flightController.GetFlightsAsync() as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

        Queue<FlightView>? content = response.Value as Queue<FlightView>;
        Assert.IsNotNull(content);
        Assert.IsTrue(flightViews.All(fv => content.Contains(fv)));

    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async IAsyncEnumerable<FlightView> FlightViewAsyncGeneratorAsync(IEnumerable<FlightView> flightViews)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (FlightView flightView in flightViews)
        {
            yield return flightView;
        }
    }

    [TestMethod]
    public async Task GetFlights_Failure_FlightNotFoundException_404Async()
    {
        Mock<FlightService> flightService = new();
        _ = flightService.Setup(fs => fs.GetFlightsAsync())
                          .Throws(new FlightNotFoundException());
        FlightController flightController = new(flightService.Object);
        ObjectResult? objectResult = await flightController.GetFlightsAsync() as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        Assert.AreEqual("No flights were found in the database", objectResult.Value);
    }

    [TestMethod]
    public async Task GetFlights_Failure_ArgumentExcpetion_500Async()
    {
        Mock<FlightService> flightService = new();
        _ = flightService.Setup(fs => fs.GetFlightsAsync())
                          .Throws(new ArgumentException());
        FlightController flightController = new(flightService.Object);
        ObjectResult? objectResult = await flightController.GetFlightsAsync() as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        Assert.AreEqual("An error ocurred", objectResult.Value);
    }

    [TestMethod]
    public async Task GetFlightByFlightNumber_SuccesssAsync()
    {
        Mock<FlightService> flightService = new();
        FlightView flightView = new("0", 
                                    new AirportInfo("Lagos", "LOS"), 
                                    new AirportInfo("Marrakesh", "RAK"));
        _ = flightService.Setup(fs => fs.GetFlightByFlightNumberAsync(0))
                         .Returns(Task.FromResult(flightView));
        FlightController flightController = new(flightService.Object);

        ObjectResult? objectResult = await flightController.GetFlightByFlightNumberAsync(0) as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);

        FlightView? content = objectResult.Value as FlightView;
        Assert.IsNotNull(content);

        Assert.AreEqual(flightView, content);
    }

    [TestMethod]
    public async Task GetFlightByFlightNumber_Failure_FlightNotFoundException_404Async()
    {
        Mock<FlightService> flightService = new();
        _ = flightService.Setup(fs => fs.GetFlightByFlightNumberAsync(1))
                         .Throws(new FlightNotFoundException());
        FlightController flightController = new(flightService.Object);
        ObjectResult? objectResult = await flightController.GetFlightByFlightNumberAsync(1) as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        Assert.AreEqual("The flight was not found in the database", objectResult.Value);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1)]
    public async Task GetFlightByFLightNumber_Failure_ArgumentException_400Async(int flightNumber)
    {
        Mock<FlightService> flightService = new();
        _ = flightService.Setup(fs => fs.GetFlightByFlightNumberAsync(1))
                         .Throws(new ArgumentException("Invalid flight number", nameof(flightNumber)));
        FlightController flightController = new(flightService.Object);
        ObjectResult? objectResult = await flightController.GetFlightByFlightNumberAsync(flightNumber) as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        Assert.AreEqual("Bad request", objectResult.Value);
    }
}