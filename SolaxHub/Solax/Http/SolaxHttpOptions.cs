namespace SolaxHub.Solax.Http
{
    public class SolaxHttpOptions
    {
        public string TokenId { get; init; } = null!;
        public string SerialNumber { get; init; } = null!;
        public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(7);
        public bool Enabled { get; init; } = false;
    }
}
