using System.Text.Json;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax;

internal class SolaxProcessorService : ISolaxProcessorService
{
    private readonly ILogger<SolaxProcessorService> _logger;
    private readonly IEnumerable<ISolaxConsumer> _solaxConsumers;
    private SolaxData? _latestSolaxData;
    private readonly object _latestSolaxDataLock = new ();

    public SolaxProcessorService(ILogger<SolaxProcessorService> logger, IEnumerable<ISolaxConsumer> solaxConsumers)
    {
        _logger = logger;
        _solaxConsumers = solaxConsumers;
    }

    public async Task ConsumeSolaxDataAsync(SolaxData data, CancellationToken cancellationToken)
    {
        lock (_latestSolaxDataLock)
        {
            _latestSolaxData = data;
        }

        _logger.LogTrace("{log}", JsonSerializer.Serialize(_latestSolaxData));

        try
        {
            foreach (var solaxProcessor in _solaxConsumers.Where(m => m.Enabled))
            {
                await solaxProcessor.ConsumeSolaxDataAsync(_latestSolaxData, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message}", e.Message);
        }
    }

    public SolaxData? ReadSolaxData()
    {
        lock (_latestSolaxDataLock)
        {
            return _latestSolaxData;
        }
    }
}