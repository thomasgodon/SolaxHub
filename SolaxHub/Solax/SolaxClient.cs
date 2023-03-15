using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolaxHub.Solax
{
    internal class SolaxClient : ISolaxClient
    {
        private readonly ILogger<SolaxClient> _logger;
        private readonly SolaxOptions _solaxClientOptions;
        private readonly ISolaxProcessorService _solaxProcessorService;
        private const string BaseUrl = "https://www.solaxcloud.com/proxyApp/proxy/api/getRealtimeInfo.do?";

        public SolaxClient(ILogger<SolaxClient> logger, ISolaxProcessorService solaxProcessorService, IOptions<SolaxOptions> solaxClientOptions)
        {
            _logger = logger;
            _solaxClientOptions = solaxClientOptions.Value;
            _solaxProcessorService = solaxProcessorService;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
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
                await _solaxProcessorService.ProcessJson(json, cancellationToken);
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
