using Newtonsoft.Json;

namespace SolaxHub.Solax.Http
{
    internal class SolaxHttpClientResponse
    {
        [JsonProperty("success")]
        public bool Success { get; init; }
        [JsonProperty("exception")]
        public string Exception { get; init; } = default!;
        [JsonProperty("result")]
        public SolaxHttpResult Result { get; init; } = default!;
    }
}
