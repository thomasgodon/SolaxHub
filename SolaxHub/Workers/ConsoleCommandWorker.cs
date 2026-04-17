using MediatR;
using SolaxHub.Application.Inverter.Commands.SetInverterUseMode;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Application.PowerControl;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Workers;

internal class ConsoleCommandWorker : BackgroundService
{
    private readonly ISender _sender;
    private readonly IInverterCommandQueue _commandQueue;
    private readonly IPowerControlStateService _powerControlState;
    private readonly ILogger<ConsoleCommandWorker> _logger;

    public ConsoleCommandWorker(ISender sender, IInverterCommandQueue commandQueue, IPowerControlStateService powerControlState, ILogger<ConsoleCommandWorker> logger)
    {
        _sender = sender;
        _commandQueue = commandQueue;
        _powerControlState = powerControlState;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        PrintHelp();

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await Task.Run(() => Console.ReadLine(), cancellationToken);
            if (line is null)
                break;

            var trimmed = line.Trim();
            if (trimmed.Length == 0)
                continue;

            ProcessCommand(trimmed);
        }
    }

    private void ProcessCommand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (parts)
        {
            case ["set", "power-control", var modeStr] when int.TryParse(modeStr, out var modeNum):
                SetPowerControl(modeNum, 0);
                break;

            case ["set", "power-control", var modeStr, var wattsStr]
                when int.TryParse(modeStr, out var modeNum) && int.TryParse(wattsStr, out var watts):
                SetPowerControl(modeNum, watts);
                break;

            case ["set", "use-mode", "solar-only"]:
                _commandQueue.Enqueue(ct => _sender.Send(new SetPowerControlCommand(PowerControlMode.SelfConsumeChargeOnlyMode, 0), ct));
                Console.WriteLine("OK: use mode set to SelfConsumeChargeOnly (queued)");
                break;

            case ["set", "use-mode", var name]:
                var useMode = ParseUseMode(name);
                if (useMode is null)
                {
                    Console.WriteLine($"Unknown mode '{name}'. Valid modes: self-use | feed-in | backup | force-time | solar-only");
                    break;
                }
                _commandQueue.Enqueue(ct => _sender.Send(new SetInverterUseModeCommand(useMode.Value), ct));
                Console.WriteLine($"OK: inverter use mode set to {useMode.Value} (queued)");
                break;

            default:
                Console.WriteLine($"Unknown command: {input}");
                PrintHelp();
                break;
        }
    }

    private void SetPowerControl(int modeNum, int watts)
    {
        var mode = (PowerControlMode)modeNum;

        _powerControlState.SetActiveMode(mode);
        _powerControlState.SetPowerTarget(watts);

        if (mode == PowerControlMode.Disabled)
        {
            _commandQueue.Enqueue(ct => _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), ct));
            Console.WriteLine("OK: power control disabled (queued, periodic sending stopped)");
            return;
        }

        var wattsLabel = watts != 0 ? $", target={watts}W" : "";
        Console.WriteLine($"OK: power control mode={modeNum}{wattsLabel} (re-sent every poll cycle)");
    }

    private static InverterUseMode? ParseUseMode(string name) => name switch
    {
        "self-use" => InverterUseMode.SelfUse,
        "feed-in" => InverterUseMode.FeedInPriority,
        "backup" => InverterUseMode.BackUp,
        "force-time" => InverterUseMode.ForceTimeUse,
        _ => null
    };

    private static void PrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  set power-control <mode> [watts]   Set VPP power control mode (re-sent every poll cycle)");
        Console.WriteLine("                                       mode 0 = disable, mode 1 = GridWTarget, mode 5/6/7 = no watts needed");
        Console.WriteLine("  set use-mode <name>                Set inverter use mode: self-use | feed-in | backup | force-time | solar-only");
    }
}
