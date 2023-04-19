using System.Diagnostics;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SolaxHub.Solax;
using SolaxHub.Solax.Http;

namespace SolaxHub.IotCentral
{
    internal class IotCentralProcessor : ISolaxProcessor
    {
        private readonly ILogger<IotCentralProcessor> _logger;
        private readonly IotCentralOptions _iotCentralOptions;
        private readonly Stopwatch _registerInterval;
        private DateTime _previousSentTime;
        private DeviceClient _deviceClient = default!;
        private string? _previousResult;

        public IotCentralProcessor(ILogger<IotCentralProcessor> logger, IOptions<IotCentralOptions> iotCentralOptions)
        {
            _logger = logger;
            _iotCentralOptions = iotCentralOptions.Value;
            _registerInterval = new Stopwatch();
        }

        public async Task ProcessData(SolaxData data, CancellationToken cancellationToken)
        {
            if (!_iotCentralOptions.Enabled) return;

            var serializedResult = JsonConvert.SerializeObject(data);
            if (_previousResult == serializedResult)
            {
                if (DateTime.Now.Subtract(_previousSentTime) < _iotCentralOptions.SendInterval)
                {
                    return;
                }
            }

            _previousSentTime = DateTime.Now;
            _previousResult = serializedResult;

            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)))
                {
                    ContentEncoding = Encoding.UTF8.WebName
                };

                if (_registerInterval.IsRunning is false)
                {
                    await CreateDeviceClientAsync(cancellationToken);
                }
                
                await _deviceClient.SendEventAsync(message, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not send message to device {deviceId}", _iotCentralOptions.DeviceId);
                await CreateDeviceClientAsync(cancellationToken);
            }
        }

        private async Task CreateDeviceClientAsync(CancellationToken cancellationToken)
        {
            if (_registerInterval.IsRunning && _registerInterval.Elapsed < TimeSpan.FromHours(1))
            {
                return;
            }

            var underlyingIotHub = await GetUnderlyingIotHub(cancellationToken);

            if (underlyingIotHub == null)
            {
                return;
            }

            _deviceClient = CreateDeviceClient(underlyingIotHub);
            _registerInterval.Restart();
        }

        private async Task<string?> GetUnderlyingIotHub(CancellationToken cancellationToken)
        {
            try
            {
                using var symmetricKeyProvider = new SecurityProviderSymmetricKey(_iotCentralOptions.DeviceId, _iotCentralOptions.PrimaryKey, _iotCentralOptions.SecondaryKey);
                var dps = ProvisioningDeviceClient.Create(_iotCentralOptions.ProvisioningUri, _iotCentralOptions.IdScope, symmetricKeyProvider, new ProvisioningTransportHandlerAmqp());
                var registerResult = await dps.RegisterAsync(cancellationToken);
                _logger.LogInformation("New registration succeeded for device {deviceId}", _iotCentralOptions.DeviceId);
                return registerResult.AssignedHub;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get underlying iot hub");
                return null;
            }
        }

        private DeviceClient CreateDeviceClient(string assignedIotHub)
        {
            var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(_iotCentralOptions.DeviceId, _iotCentralOptions.PrimaryKey);
            var client = DeviceClient.Create(assignedIotHub, authMethod, TransportType.Amqp);
            return client;
        }
    }
}
