
using FlyingDutchmanAirlines.Views;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.Views;

[TestClass]
public class FlightViewTests
{
    [TestMethod]
    public void Constrcutor_FLightView_Success()
    {
        string flightNumber = "0";
        string originCity = "Amsterdam";
        string originCityCode = "AMS";
        AirportInfo originAirportInfo = new(originCity, originCityCode);

        string destinaationCity = "Moscow";
        string destinationCityCode = "SVO";
        AirportInfo destinationAirportInfo = new(destinaationCity, destinationCityCode);

        FlightView flightView = new(flightNumber, originAirportInfo, destinationAirportInfo);
        Assert.IsNotNull(flightView);

        Assert.AreEqual(flightView.FlightNumber, flightNumber);
        Assert.AreEqual(flightView.Origin, originAirportInfo);
        Assert.AreEqual(flightView.Destination, destinationAirportInfo);
    }

    [TestMethod]
    public void Constructor_FlightViewSuccess_FlightNumber_Null()
    {
        string originCity = "Athens";
        string originCityCode = "ATH";
        AirportInfo originAirportInfo = new(originCity, originCityCode);

        string destinationCity = "Dubai";
        string destinationCode = "DXB";
        AirportInfo destinationAirportInfo = new(destinationCity, destinationCode);

        FlightView flightView = new(null!, originAirportInfo, destinationAirportInfo);

        Assert.AreEqual(flightView.FlightNumber, "No flight number found");
        Assert.AreEqual(flightView.Origin, originAirportInfo);
        Assert.AreEqual(flightView.Destination, destinationAirportInfo);
    }

    [TestMethod]
    public void Constructor_AirportInfo_Success_City_EmptyString()
    {
        string destinationCity = string.Empty;
        string destinationCityCode = "SYD";
        AirportInfo airportInfo = new(destinationCity, destinationCityCode);

        Assert.AreEqual(airportInfo.City, "No city found");
        Assert.AreEqual(airportInfo.Code, destinationCityCode);
    }

    [TestMethod]
    public void Constructor_AirportInfo_Success_Code_EmptyString()
    {
        string destinationCity = "Ushuaia";
        string destinationCityCode = string.Empty;
        AirportInfo airportInfo = new(destinationCity, destinationCityCode);

        Assert.AreEqual(airportInfo.City, destinationCity);
        Assert.AreEqual(airportInfo.Code, "No code found");
    }
}
