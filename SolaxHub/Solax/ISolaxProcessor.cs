namespace SolaxHub.Solax
{
    public interface ISolaxProcessor
    {
        Task ProcessTelegram(Telegram telegram, CancellationToken cancellationToken);
    }
}
