using System.Diagnostics;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SolaxHub.IotCentral.Extensions;
using SolaxHub.IotCentral.Models;
using SolaxHub.Solax;

namespace SolaxHub.IotCentral
{
    internal class IotCentralPublisherService : ISolaxConsumer
    {
        private readonly ILogger<IotCentralPublisherService> _logger;
        private readonly ISolaxProcessorService _solaxProcessorService;
        private readonly List<(DeviceClient Client,Stopwatch Interval, IotDevice DeviceOptions)> _deviceClients = new();
        private readonly IotCentralOptions _options;
        private string? _previousResult;

        public bool Enabled => _options.IotDevices.Any(m => m.Enabled);

        public IotCentralPublisherService(
            ILogger<IotCentralPublisherService> logger, 
            IOptions<IotCentralOptions> iotCentralOptions,
            ISolaxProcessorService solaxProcessorService
            )
        {
            _logger = logger;
            _solaxProcessorService = solaxProcessorService;
            _options = iotCentralOptions.Value;
        }

        private async Task PopulateDeviceList(CancellationToken cancellationToken)
        {
            foreach (var optionsIotDevice in _options.IotDevices)
            {
                var client = await CreateDeviceClientAsync(optionsIotDevice, cancellationToken);
                if (client == null)
                {
                    continue;
                }

                var interval = new Stopwatch();
                interval.Start();
                _deviceClients.Add((client, interval, optionsIotDevice));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Enabled is false)
            {
                return;
            }

            if (_deviceClients.Any() is false)
            {
                await PopulateDeviceList(cancellationToken);
            }

            while (cancellationToken.IsCancellationRequested is false)
            {
                // get solax data
                var data = _solaxProcessorService.ReadSolaxData();

                // process solax data
                var serializedResult = JsonConvert.SerializeObject(data.ToDeviceData());

                foreach (var (client, interval, deviceOptions) in _deviceClients)
                {
                    if (!deviceOptions.Enabled) return;

                    if (interval.Elapsed < deviceOptions.SendInterval)
                    {
                        continue;
                    }

                    if (_previousResult == serializedResult)
                    {
                        continue;
                    }

                    try
                    {
                        var message = new Message(Encoding.UTF8.GetBytes(serializedResult))
                        {
                            ContentEncoding = Encoding.UTF8.WebName
                        };

                        await client.SendEventAsync(message, cancellationToken);
                        _logger.LogDebug("Send to device with id: {deviceId}", deviceOptions.DeviceId);
                        interval.Restart();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Could not send message to device {deviceId}", deviceOptions.DeviceId);
                    }
                }

                _previousResult = serializedResult;

                // wait for next poll
                // lowest send interval from the configured iot devices is used
                await Task.Delay(_options.IotDevices.Select(m => m.SendInterval).Min(), cancellationToken);
            }
        }

        private async Task<DeviceClient?> CreateDeviceClientAsync(IotDevice options, CancellationToken cancellationToken)
        {
            var underlyingIotHub = await GetUnderlyingIotHub(options, cancellationToken);

            if (underlyingIotHub == null)
            {
                return null;
            }

            var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(options.DeviceId, options.PrimaryKey);
            var client = DeviceClient.Create(underlyingIotHub, authMethod, TransportType.Amqp);
            if (client == null)
            {
                _logger.LogError("Could not create device {deviceId}", options.DeviceId);
            }
            return client;
        }

        private async Task<string?> GetUnderlyingIotHub(IotDevice options, CancellationToken cancellationToken)
        {
            try
            {
                using var symmetricKeyProvider = new SecurityProviderSymmetricKey(options.DeviceId, options.PrimaryKey, options.SecondaryKey);
                var dps = ProvisioningDeviceClient.Create(options.ProvisioningUri, options.IdScope, symmetricKeyProvider, new ProvisioningTransportHandlerAmqp());
                var registerResult = await dps.RegisterAsync(cancellationToken);
                _logger.LogInformation("New registration succeeded for device {deviceId}", options.DeviceId);
                return registerResult.AssignedHub;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not get underlying iot hub");
                return null;
            }
        }
    }
}
