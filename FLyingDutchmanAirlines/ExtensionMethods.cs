using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines
{
    internal static class ExtensionMethods
    {
        internal static bool IsNonNegative(this int x) => !x.IsNegative();

        internal static bool IsNegative(this int x) => x < 0;
    }
}
