namespace FlyingDutchmanAirlines.Views;

public record struct AirportInfo
{
    public string City { get; init; }
    public string Code { get; init; }

    public AirportInfo(string city, string code)
    {
        City = string.IsNullOrWhiteSpace(city) ? "No city found" : city;
        Code = string.IsNullOrWhiteSpace(code) ? "No code found" : code;
    }
}