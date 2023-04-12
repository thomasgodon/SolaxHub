using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolaxHub.Solax.Http
{
    internal class SolaxHttpClient : ISolaxClient
    {
        private readonly ILogger<SolaxHttpClient> _logger;
        private readonly SolaxHttpOptions _solaxClientOptions;
        private readonly ISolaxProcessorService _solaxProcessorService;
        private const string BaseUrl = "https://www.solaxcloud.com/proxyApp/proxy/api/getRealtimeInfo.do?";

        public SolaxHttpClient(ILogger<SolaxHttpClient> logger, ISolaxProcessorService solaxProcessorService, IOptions<SolaxHttpOptions> solaxClientOptions)
        {
            _logger = logger;
            _solaxClientOptions = solaxClientOptions.Value;
            _solaxProcessorService = solaxProcessorService;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (_solaxClientOptions.Enabled is false)
            {
                _logger.LogWarning($"{nameof(SolaxHttpClient)} not enabled");
                return;
            }

            await Task.Run(async () =>
            {
                // Keep this task alive until it is cancelled
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(_solaxClientOptions.PollInterval, cancellationToken);
                    await CallSolaxApi(cancellationToken);
                }
            }, cancellationToken);
        }

        private async Task CallSolaxApi(CancellationToken cancellationToken)
        {
            using var client = new HttpClient();

            try
            {
                var result = await client.GetStringAsync(BuildUrl(), cancellationToken);
                var json = JObject.Parse(result);
                _logger.LogTrace(json.ToString());

                var solaxClientResponse = json.ToObject<SolaxHttpClientResponse>();
                if (solaxClientResponse == null)
                {
                    throw new NullReferenceException($"Could not cast json '{json}' to '{typeof(SolaxHttpResult)}'");
                }

                if (solaxClientResponse.Success is false)
                {
                    _logger.LogWarning("Response was not successful with reason: {reason}", solaxClientResponse.Exception);
                    return;
                }

                await _solaxProcessorService.ProcessData(solaxClientResponse.Result.ToSolaxData(), cancellationToken);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Could not parse result from '{url}'", BuildUrl().AbsoluteUri);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while calling '{url}'", BuildUrl().AbsoluteUri);
            }
        }

        private Uri BuildUrl()
        {
            return new Uri($"{BaseUrl}tokenId={_solaxClientOptions.TokenId}&sn={_solaxClientOptions.SerialNumber}");
        }
    }
}
