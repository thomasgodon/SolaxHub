using Newtonsoft.Json;

namespace SolaxHub.Solax
{
    internal class SolaxClientResponse
    {
        [JsonProperty("success")]
        public bool Success { get; init; }
        [JsonProperty("exception")]
        public string Exception { get; init; } = default!;
        [JsonProperty("result")]
        public SolaxResult Result { get; init; } = default!;
    }
}
