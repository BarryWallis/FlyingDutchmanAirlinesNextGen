namespace FlyingDutchmanAirlines.Views;

public record FlightView
{
    public string FlightNumber { get; init; }
    public AirportInfo Origin { get; init; }
    public AirportInfo Destination { get; init; }

    public FlightView(string flightNumber, AirportInfo origin, AirportInfo destination)
    {
        FlightNumber = string.IsNullOrWhiteSpace(flightNumber) ? "No flight number found" : flightNumber;
        Origin = origin;
        Destination = destination;
    }

}
