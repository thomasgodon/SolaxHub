using SolaxHub.Solax;

namespace SolaxHub
{
    internal class Worker : BackgroundService
    {
        private readonly ISolaxModbusClient _solaxModbusClient;

        public Worker(ISolaxModbusClient solaxModbusClient)
        {
            _solaxModbusClient = solaxModbusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _solaxModbusClient.Start(cancellationToken);
        }
    }
}