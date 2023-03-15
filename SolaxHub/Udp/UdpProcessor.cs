using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using SolaxHub.Solax.Extensions;
using SolaxHub.Udp.Extensions;

namespace SolaxHub.Udp
{
    internal class UdpProcessor : ISolaxProcessor
    {
        private readonly ILogger<UdpProcessor> _logger;
        private readonly UdpOptions _udpOptions;

        public UdpProcessor(ILogger<UdpProcessor> logger, IOptions<UdpOptions> udpOptions)
        {
            _logger = logger;
            _udpOptions = udpOptions.Value;
        }

        async Task ISolaxProcessor.ProcessResult(SolaxResult result, CancellationToken cancellationToken)
        {
            if (!_udpOptions.Enabled) return;

            await result.ToUdpPacket(nameof(result.InverterSerialNumber)).SendToAsync(_udpOptions.Host, 20000, cancellationToken);
            await result.ToUdpPacket(nameof(result.SerialNumber)).SendToAsync(_udpOptions.Host, 20001, cancellationToken);
            await result.ToUdpPacket(nameof(result.AcPower)).SendToAsync(_udpOptions.Host, 20002, cancellationToken);
            await result.ToUdpPacket(nameof(result.YieldToday)).SendToAsync(_udpOptions.Host, 20003, cancellationToken);
            await result.ToUdpPacket(nameof(result.YieldTotal)).SendToAsync(_udpOptions.Host, 20004, cancellationToken);
            await result.ToUdpPacket(nameof(result.FeedInPower)).SendToAsync(_udpOptions.Host, 20005, cancellationToken);
            await result.ToUdpPacket(nameof(result.FeedInEnergy)).SendToAsync(_udpOptions.Host, 20006, cancellationToken);
            await result.ToUdpPacket(nameof(result.ConsumeEnergy)).SendToAsync(_udpOptions.Host, 20007, cancellationToken);
            await result.ToUdpPacket(nameof(result.FeedInPowerM2)).SendToAsync(_udpOptions.Host, 20008, cancellationToken);
            await result.ToUdpPacket(nameof(result.Soc)).SendToAsync(_udpOptions.Host, 20009, cancellationToken);
            await result.ToUdpPacket(nameof(result.EpsPowerR)).SendToAsync(_udpOptions.Host, 20010, cancellationToken);
            await result.ToUdpPacket(nameof(result.EpsPowerS)).SendToAsync(_udpOptions.Host, 20011, cancellationToken);
            await result.ToUdpPacket(nameof(result.EpsPowerT)).SendToAsync(_udpOptions.Host, 20012, cancellationToken);
            await result.ToUdpPacket(nameof(result.InverterType)).SendToAsync(_udpOptions.Host, 20013, cancellationToken);
            await result.ToUdpPacket(nameof(result.InverterStatus)).SendToAsync(_udpOptions.Host, 20014, cancellationToken);
            await result.ToUdpPacket(nameof(result.UploadTime)).SendToAsync(_udpOptions.Host, 20015, cancellationToken);
            await result.ToUdpPacket(nameof(result.BatteryPower)).SendToAsync(_udpOptions.Host, 20016, cancellationToken);
            await result.ToUdpPacket(nameof(result.PvPowerMppt1)).SendToAsync(_udpOptions.Host, 20017, cancellationToken);
            await result.ToUdpPacket(nameof(result.PvPowerMppt2)).SendToAsync(_udpOptions.Host, 20018, cancellationToken);
            await result.ToUdpPacket(nameof(result.PvPowerMppt3)).SendToAsync(_udpOptions.Host, 20019, cancellationToken);
            await result.ToUdpPacket(nameof(result.PvPowerMppt4)).SendToAsync(_udpOptions.Host, 20020, cancellationToken);
            await result.ToUdpPacket(nameof(result.BatteryStatus)).SendToAsync(_udpOptions.Host, 20021, cancellationToken);
        }
    }
}
