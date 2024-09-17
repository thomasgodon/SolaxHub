using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculateRemoteControlRequestHandler : IRequestHandler<CalculateRemoteControlRequest, SolaxPowerControlCalculation>
    {
        private readonly ISolaxControllerService _solaxControllerService;
        private readonly ILogger<CalculateRemoteControlRequestHandler> _logger;
        private readonly SolaxModbusOptions _solaxModbusOptions;

        public CalculateRemoteControlRequestHandler(
            ISolaxControllerService solaxControllerService,
            IOptions<SolaxModbusOptions> solaxModbusOptions,
            ILogger<CalculateRemoteControlRequestHandler> logger)
        {
            _solaxControllerService = solaxControllerService;
            _logger = logger;
            _solaxModbusOptions = solaxModbusOptions.Value;
        }

        public Task<SolaxPowerControlCalculation> Handle(CalculateRemoteControlRequest request, CancellationToken cancellationToken)
            => _solaxControllerService.PowerControlMode switch
            {
                SolaxPowerControlMode.PowerControlMode => Task.FromResult(CalculatePowerControlMode()),
                SolaxPowerControlMode.PushPowerPositiveNegativeMode => Task.FromResult(CalculatePushPowerPositiveNegativeMode()),

                _ => Task.FromResult(SolaxPowerControlCalculation.Disabled())
            };

        /// <summary>
        /// This mode controls the active and reactive power at the AC port of the inverter for a period of time. In this mode the PV runs at the highest possible power and the system can feed/take power to/from the grid.
        /// </summary>
        /// <returns></returns>
        private SolaxPowerControlCalculation CalculatePowerControlMode()
        {
            // The exit of mode 1 based on the two limit values, “Time_of_Duration” and “RemoteCtrlTimeOut”. That is, the “Time_of_Duration” is judged first,
            // if during this period there is no new power target command, then continue to wait for the time of “RemoteCtrlTimeOut”, if “RemoteCtrlTimeOut” is satisfied,
            // then the mode 1 is exited to the regular work mode of the inverter.

            // The key of the model is that the system will try to meet the target power as much as possible.
            // In this mode the AC port power target is positive for charging and negative for discharging
            // the battery input represents charging and the output represents discharging; the PV is always the input to the inverter.

            var mode = BitConverter.GetBytes(Convert.ToUInt16(SolaxPowerControlMode.PowerControlMode)).Reverse();
            var targetSetType = BitConverter.GetBytes(Convert.ToUInt16(SolaxPowerControlTargetSetType.Set)).Reverse();
            var activePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(_solaxControllerService.PowerControlImportLimit))).Reverse();
            var reactivePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(0))).Reverse();
            var timeOfDuration = BitConverter.GetBytes(Convert.ToUInt16(_solaxModbusOptions.PollInterval.TotalSeconds + 3)).Reverse();
            var timeOut = BitConverter.GetBytes(Convert.ToUInt16(0)).Reverse();

            _logger.LogTrace("mode: {Mode} - active power: {ActivePower} - duration: {Duration} - timeout: {Timeout}", SolaxPowerControlMode.PowerControlMode, _solaxControllerService.PowerControlImportLimit, _solaxModbusOptions.PollInterval.TotalSeconds + 3, 0);

            var dataset = mode
                .Concat(targetSetType)
                .Concat(activePower)
                .Concat(reactivePower)
                .Concat(timeOfDuration)
                .Concat(timeOut)
                .ToArray();

            return new SolaxPowerControlCalculation(SolaxPowerControlMode.PowerControlMode, dataset);
        }

        /// <summary>
        /// This mode directly controls the battery charging/discharging power, the PV power is as high as possible and the system can feed/take power to/from the grid.
        /// </summary>
        /// <returns></returns>
        private SolaxPowerControlCalculation CalculatePushPowerPositiveNegativeMode()
        {
            // The positive and negative values of the data in this model are defined as: positive means battery discharge, negative means battery charge.

            var pushPowerValue = _solaxControllerService.PowerControlBatteryChargeLimit > 0
                ? _solaxControllerService.PowerControlBatteryChargeLimit
                : 0;

            var mode = BitConverter.GetBytes(Convert.ToUInt16(SolaxPowerControlMode.PushPowerPositiveNegativeMode)).Reverse();
            var pushPower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(pushPowerValue))).Reverse();

            _logger.LogTrace("mode: {Mode} - push power: {PushPower}", SolaxPowerControlMode.PushPowerPositiveNegativeMode, pushPowerValue);

            var dataset = mode
                .Concat(pushPower)
                .ToArray();

            return new SolaxPowerControlCalculation(SolaxPowerControlMode.PushPowerPositiveNegativeMode, dataset);
        }

        private static byte[] ReverseBits(byte[] data)
        {
            if (data.Length != 4)
            {
                return data;
            }
            var reversed = new byte[data.Length];
            Buffer.BlockCopy(data, 0, reversed, 2, 2);
            Buffer.BlockCopy(data, 2, reversed, 0, 2);
            return reversed;
        }
    }
}
