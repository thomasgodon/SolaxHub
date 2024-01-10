using SolaxHub.Solax;

namespace SolaxHub
{
    internal class Worker : BackgroundService
    {
        private readonly ISolaxClientFactory _solaxClientFactory;

        public Worker(ISolaxClientFactory solaxClientFactory)
        {
            _solaxClientFactory = solaxClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = _solaxClientFactory.CreateSolaxClient();
            await client.Start(cancellationToken);
        }
    }
}