using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using SolaxHub.Solax.Http;

namespace SolaxHub
{
    internal class Worker : BackgroundService
    {
        private readonly ISolaxClientFactory _solaxClientFactory;
        private readonly IEnumerable<ISolaxProcessor> _solaxProcessors;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, ISolaxClientFactory solaxClientFactory, IOptions<SolaxHttpOptions> dsmrOptions, IEnumerable<ISolaxProcessor> solaxProcessors)
        {
            _logger = logger;
            _solaxClientFactory = solaxClientFactory;
            _solaxProcessors = solaxProcessors;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = _solaxClientFactory.CreateSolaxClient();

            if (client == null)
            {
                _logger.LogError("No Solax Client was enabled");
                return;
            }

            await client.Start(cancellationToken);
        }
    }
}