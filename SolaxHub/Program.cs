using SolaxHub.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services
            .AddSolaxHub(configuration)
            .AddSolaxHubObservability(configuration);
    })
    .ConfigureLogging((context, builder) =>
    {
        builder.AddSolaxHubLogging(context.Configuration);
    })
    .Build();

await host.RunAsync();
