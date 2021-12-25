namespace FlyingDutchmanAirlines;

internal static class ExtensionMethods
{
    internal static bool IsNonNegative(this int x) => !x.IsNegative();

    internal static bool IsNegative(this int x) => x < 0;
}
