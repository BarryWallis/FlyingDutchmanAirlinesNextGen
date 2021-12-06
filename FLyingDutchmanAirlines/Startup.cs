using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDutchmanAirlines;

internal class Startup
{
    public static void Configure(IApplicationBuilder applicationBuilder)
    {
        _ = applicationBuilder.UseRouting();
        _ = applicationBuilder.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    public static void ConfigureServices(IServiceCollection services) => services.AddControllers();
}
