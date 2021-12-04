﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FlyingDutchmanAirlines
{
    class Program
    {
        static void Main() => InitializeHost();

        private static void InitializeHost() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    _ = webBuilder.UseStartup<Startup>();
                    _ = webBuilder.UseUrls("http://0.0.0.0:8080");
                })
                .Build()
                .Run();
    }
}