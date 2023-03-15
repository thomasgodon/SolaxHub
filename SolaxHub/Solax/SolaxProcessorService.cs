using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SolaxHub.Solax;

internal class SolaxProcessorService : ISolaxProcessorService
{
    private readonly ILogger<SolaxProcessorService> _logger;
    private readonly IEnumerable<ISolaxProcessor> _solaxProcessors;

    public SolaxProcessorService(ILogger<SolaxProcessorService> logger, IEnumerable<ISolaxProcessor> solaxProcessors)
    {
        _logger = logger;
        _solaxProcessors = solaxProcessors;
    }

    public async Task ProcessJson(JObject json, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogTrace(json.ToString());
            var solaxClientResponse = json.ToObject<SolaxClientResponse>();

            if (solaxClientResponse == null)
            {
                throw new NullReferenceException($"Could not cast json '{json}' to '{typeof(SolaxResult)}'");
            }

            if (solaxClientResponse.Success is false)
            {
                _logger.LogWarning("Response was not successful with reason: {reason}", solaxClientResponse.Exception);
                return;
            }

            foreach (var solaxProcessor in _solaxProcessors)
            {
                await solaxProcessor.ProcessResult(solaxClientResponse.Result, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message}", e.Message);
        }
    }
}