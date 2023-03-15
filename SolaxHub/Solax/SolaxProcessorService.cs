using System.Text;

namespace SolaxHub.Solax;

internal class SolaxProcessorService : ISolaxProcessorService
{
    private readonly ILogger<SolaxProcessorService> _logger;
    private readonly StringBuilder _buffer = new();
    private readonly DSMRTelegramParser _dsmrParser;
    private readonly IEnumerable<ISolaxProcessor> _dsmrProcessors;

    public SolaxProcessorService(ILogger<SolaxProcessorService> logger, IEnumerable<ISolaxProcessor> dsmrProcessors)
    {
        _dsmrParser = new DSMRTelegramParser();
        _logger = logger;
        _dsmrProcessors = dsmrProcessors;
    }

    public async Task ProcessMessage(string message, CancellationToken cancellationToken)
    {
        try
        {
            if (!_dsmrParser.TryParse(message, out Telegram? telegram))
            {
                return;
            }

            if (telegram?.DSMRVersion == null)
            {
                return;
            }

            _logger.LogTrace(telegram?.ToString());

            foreach (var dsmrProcessor in _dsmrProcessors)
            {
                await dsmrProcessor.ProcessTelegram(telegram, cancellationToken);
            }
        }
        catch (InvalidOBISIdException e)
        {
            _logger.LogWarning($"{e.Message} - {message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}