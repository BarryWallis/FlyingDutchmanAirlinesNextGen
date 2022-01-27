
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RespositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDutchmanAirlines;

internal class Startup
{
    public static void Configure(IApplicationBuilder applicationBuilder)
    {
        _ = applicationBuilder.UseRouting();
        _ = applicationBuilder.UseEndpoints(endpoints => endpoints.MapControllers());
        _ = applicationBuilder.UseSwagger();
        _ = applicationBuilder.UseSwaggerUI(so => so.SwaggerEndpoint("/swagger/v1/swagger.json",
                                                                     "Flying Dutchman Airlines"));
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddControllers();

        _ = services.AddTransient(typeof(FlightService), typeof(FlightService));
        _ = services.AddTransient(typeof(BookingService), typeof(BookingService));
        _ = services.AddTransient(typeof(FlightRepository), typeof(FlightRepository));
        _ = services.AddTransient(typeof(AirportRepository), typeof(AirportRepository));
        _ = services.AddTransient(typeof(BookingRepository), typeof(BookingRepository));
        _ = services.AddTransient(typeof(CustomerRepository), typeof(CustomerRepository));

        _ = services.AddDbContext<FlyingDutchmanAirlinesContext>(ServiceLifetime.Transient);
        _ = services.AddTransient(typeof(FlyingDutchmanAirlinesContext), typeof(FlyingDutchmanAirlinesContext));
        _ = services.AddSwaggerGen();
    }
}
