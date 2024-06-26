﻿using SolaxHub.Knx.Client;
using SolaxHub.Knx.Models;
using SolaxHub.Knx.Services;

namespace SolaxHub.Knx.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKnx(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<KnxOptions>(configuration.GetSection(nameof(KnxOptions)));
            serviceCollection.AddSingleton<IKnxClient, KnxClient>();
            serviceCollection.AddSingleton<IKnxValueBufferService, KnxValueBufferService>();
            return serviceCollection;
        }
    }
}
