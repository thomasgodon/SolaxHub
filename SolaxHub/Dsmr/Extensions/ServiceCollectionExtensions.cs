﻿using System.IO.Ports;

namespace SolaxHub.Dsmr.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxOptions>(configuration.GetSection(nameof(SolaxOptions)));
            serviceCollection.AddSingleton<ISolaxClient, SolaxClient>();
            serviceCollection.AddSingleton<IDsmrProcessorService, SolaxProcessorService>();
            serviceCollection.AddSingleton<SerialPort>();

            return serviceCollection;
        }
    }
}
