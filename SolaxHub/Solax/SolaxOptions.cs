namespace SolaxHub.Solax
{
    public class SolaxOptions
    {
        public string TokenId { get; init; } = null!;
        public string SerialNumber { get; init; } = null!;
        public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(7);
    }
}
