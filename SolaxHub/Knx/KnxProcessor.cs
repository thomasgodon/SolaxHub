using Microsoft.Extensions.Options;
using SolaxHub.Solax;

namespace SolaxHub.Knx
{
    internal class KnxProcessor : ISolaxProcessor
    {
        private readonly ILogger<KnxProcessor> _logger;
        private readonly KnxOptions _knxOptions;


        public KnxProcessor(ILogger<KnxProcessor> logger, IOptions<KnxOptions> knxOptions)
        {
            _logger = logger;
            _knxOptions = knxOptions.Value;
        }

        public async Task ProcessData(SolaxData data, CancellationToken cancellationToken)
        {
            if (!_knxOptions.Enabled) return;
            
        }
    }
}
