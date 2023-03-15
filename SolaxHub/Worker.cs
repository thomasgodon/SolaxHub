using Microsoft.Extensions.Options;
using SolaxHub.Solax;

namespace SolaxHub
{
    internal class Worker : BackgroundService
    {
        private readonly ISolaxClient _solaxClient;
        private readonly IEnumerable<ISolaxProcessor> _solaxProcessors;

        public Worker(ISolaxClient solaxClient, IOptions<SolaxOptions> dsmrOptions, IEnumerable<ISolaxProcessor> solaxProcessors)
        {
            _solaxClient = solaxClient;
            _solaxProcessors = solaxProcessors;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _solaxClient.Start(cancellationToken);
        }
    }
}