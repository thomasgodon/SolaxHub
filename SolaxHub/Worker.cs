using SolaxHub.Solax;

namespace SolaxHub
{
    internal class Worker : BackgroundService
    {
        private readonly ISolaxClientFactory _solaxClientFactory;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, ISolaxClientFactory solaxClientFactory)
        {
            _logger = logger;
            _solaxClientFactory = solaxClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = _solaxClientFactory.CreateSolaxClient();
            await client.Start(cancellationToken);
        }
    }
}