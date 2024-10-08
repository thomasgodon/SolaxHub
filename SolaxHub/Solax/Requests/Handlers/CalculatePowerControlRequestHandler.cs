﻿using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Requests.Handlers
{
    internal class CalculatePowerControlRequestHandler : IRequestHandler<CalculatePowerControlRequest, SolaxPowerControlCalculation>
    {
        private readonly ISolaxControllerService _solaxControllerService;
        private readonly SolaxModbusOptions _solaxModbusOptions;

        public CalculatePowerControlRequestHandler(
            ISolaxControllerService solaxControllerService,
            IOptions<SolaxModbusOptions> solaxModbusOptions)
        {
            _solaxControllerService = solaxControllerService;
            _solaxModbusOptions = solaxModbusOptions.Value;
        }

        public Task<SolaxPowerControlCalculation> Handle(CalculatePowerControlRequest request, CancellationToken cancellationToken)
            => _solaxControllerService.PowerControlMode switch
            {
                SolaxPowerControlMode.PowerControlMode => Task.FromResult(CalculatePowerControlMode(request.SolaxData)),
                SolaxPowerControlMode.ElectricQuantityTargetControlMode => Task.FromResult(CalculateElectricQuantityTargetControlMode(request.SolaxData)),

                _ => Task.FromResult(SolaxPowerControlCalculation.Disabled())
            };

        /// <summary>
        /// This mode controls the active and reactive power at the AC port of the inverter for a period of time. In this mode the PV runs at the highest possible power and the system can feed/take power to/from the grid.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private SolaxPowerControlCalculation CalculatePowerControlMode(SolaxData data)
        {
            // The exit of mode 1 based on the two limit values, “Time_of_Duration” and “RemoteCtrlTimeOut”. That is, the “Time_of_Duration” is judged first,
            // if during this period there is no new power target command, then continue to wait for the time of “RemoteCtrlTimeOut”, if “RemoteCtrlTimeOut” is satisfied,
            // then the mode 1 is exited to the regular work mode of the inverter.

            // The key of the model is that the system will try to meet the target power as much as possible.
            // In this mode the AC port power target is positive for charging and negative for discharging
            // the battery input represents charging and the output represents discharging; the PV is always the input to the inverter.

            var activePowerValue = _solaxControllerService.PowerControlImportLimit - (data.HouseLoad - data.PvCurrent1) > 0
                ? _solaxControllerService.PowerControlImportLimit - (data.HouseLoad - data.PvCurrent1)
                : 0;

            var mode = BitConverter.GetBytes(Convert.ToUInt16(SolaxPowerControlMode.PowerControlMode)).Reverse();
            var targetSetType = BitConverter.GetBytes(Convert.ToUInt16(SolaxPowerControlTargetSetType.Set)).Reverse();
            var activePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(activePowerValue))).Reverse();
            var reactivePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(0))).Reverse();
            var timeOfDuration = BitConverter.GetBytes(Convert.ToUInt16(_solaxModbusOptions.PollInterval.TotalSeconds + 3)).Reverse();
            var timeOut = BitConverter.GetBytes(Convert.ToUInt16(0)).Reverse();

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
        /// This mode controls the AC port of the inverter to input/output a certain amount of electric energy with a certain power. In this mode the PV runs at the highest possible power and the system can feed/take power to/from the grid.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private SolaxPowerControlCalculation CalculateElectricQuantityTargetControlMode(SolaxData data)
        {
            // This mode controls the AC port of the inverter to input/output a certain amount of electric energy with a certain power.
            // In this mode the PV runs at the highest possible power and the system can feed/take power to/from the grid.

            // if the energy target value (0x0084 & 0x0085) is not updated within the set time (0x0088) after completing the commands,
            // then this mode exits. When the energy target value is not reached, the mode runs until the next command is entered.


            return new SolaxPowerControlCalculation(SolaxPowerControlMode.ElectricQuantityTargetControlMode, [0, 0]);
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
