using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Http;
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

        async Task ISolaxProcessor.ProcessData(SolaxData data, CancellationToken cancellationToken)
        {
            if (!_udpOptions.Enabled) return;

            await data.ToUdpPacket(nameof(data.InverterSerialNumber)).SendToAsync(_udpOptions.Host, 20000, cancellationToken);
            await data.ToUdpPacket(nameof(data.SerialNumber)).SendToAsync(_udpOptions.Host, 20001, cancellationToken);
            await data.ToUdpPacket(nameof(data.AcPower)).SendToAsync(_udpOptions.Host, 20002, cancellationToken);
            await data.ToUdpPacket(nameof(data.YieldToday)).SendToAsync(_udpOptions.Host, 20003, cancellationToken);
            await data.ToUdpPacket(nameof(data.YieldTotal)).SendToAsync(_udpOptions.Host, 20004, cancellationToken);
            await data.ToUdpPacket(nameof(data.FeedInPower)).SendToAsync(_udpOptions.Host, 20005, cancellationToken);
            await data.ToUdpPacket(nameof(data.FeedInEnergy)).SendToAsync(_udpOptions.Host, 20006, cancellationToken);
            await data.ToUdpPacket(nameof(data.ConsumeEnergy)).SendToAsync(_udpOptions.Host, 20007, cancellationToken);
            await data.ToUdpPacket(nameof(data.FeedInPowerM2)).SendToAsync(_udpOptions.Host, 20008, cancellationToken);
            await data.ToUdpPacket(nameof(data.Soc)).SendToAsync(_udpOptions.Host, 20009, cancellationToken);
            await data.ToUdpPacket(nameof(data.EpsPowerR)).SendToAsync(_udpOptions.Host, 20010, cancellationToken);
            await data.ToUdpPacket(nameof(data.EpsPowerS)).SendToAsync(_udpOptions.Host, 20011, cancellationToken);
            await data.ToUdpPacket(nameof(data.EpsPowerT)).SendToAsync(_udpOptions.Host, 20012, cancellationToken);
            await data.ToUdpPacket(nameof(data.InverterType)).SendToAsync(_udpOptions.Host, 20013, cancellationToken);
            await data.ToUdpPacket(nameof(data.InverterStatus)).SendToAsync(_udpOptions.Host, 20014, cancellationToken);
            //await data.ToUdpPacket(nameof(data.UploadTime)).SendToAsync(_udpOptions.Host, 20015, cancellationToken);
            await data.ToUdpPacket(nameof(data.BatteryPower)).SendToAsync(_udpOptions.Host, 20016, cancellationToken);
            await data.ToUdpPacket(nameof(data.PvPowerMppt1)).SendToAsync(_udpOptions.Host, 20017, cancellationToken);
            await data.ToUdpPacket(nameof(data.PvPowerMppt2)).SendToAsync(_udpOptions.Host, 20018, cancellationToken);
            await data.ToUdpPacket(nameof(data.PvPowerMppt3)).SendToAsync(_udpOptions.Host, 20019, cancellationToken);
            await data.ToUdpPacket(nameof(data.PvPowerMppt4)).SendToAsync(_udpOptions.Host, 20020, cancellationToken);
            await data.ToUdpPacket(nameof(data.BatteryStatus)).SendToAsync(_udpOptions.Host, 20021, cancellationToken);
        }
    }
}
