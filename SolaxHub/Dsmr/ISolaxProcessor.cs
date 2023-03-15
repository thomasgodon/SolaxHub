using DSMRParser.Models;

namespace SolaxHub.Dsmr
{
    public interface ISolaxProcessor
    {
        Task ProcessTelegram(Telegram telegram, CancellationToken cancellationToken);
    }
}
