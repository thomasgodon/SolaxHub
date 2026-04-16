using MediatR;
using SolaxHub.Application.Inverter.Commands.SetBatteryChargePowerTarget;
using SolaxHub.Application.Inverter.Commands.SetBatteryDischargePowerTarget;
using SolaxHub.Application.Inverter.Commands.SetInverterUseMode;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Workers;

internal class ConsoleCommandWorker : BackgroundService
{
    private readonly ISender _sender;
    private readonly IInverterCommandQueue _commandQueue;
    private readonly ILogger<ConsoleCommandWorker> _logger;

    public ConsoleCommandWorker(ISender sender, IInverterCommandQueue commandQueue, ILogger<ConsoleCommandWorker> logger)
    {
        _sender = sender;
        _commandQueue = commandQueue;
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
            case ["set", "discharge", var wattsStr] when int.TryParse(wattsStr, out var watts):
                _commandQueue.Enqueue(ct => _sender.Send(new SetBatteryDischargePowerTargetCommand(watts), ct));
                Console.WriteLine(watts <= 0
                    ? "OK: power control disabled (queued)"
                    : $"OK: discharge power target set to {watts}W (queued)");
                break;

            case ["set", "charge", var wattsStr] when int.TryParse(wattsStr, out var watts):
                _commandQueue.Enqueue(ct => _sender.Send(new SetBatteryChargePowerTargetCommand(watts), ct));
                Console.WriteLine(watts <= 0
                    ? "OK: power control disabled (queued)"
                    : $"OK: charge power target set to {watts}W (queued)");
                break;

            case ["set", "mode", "solar-only"]:
                _commandQueue.Enqueue(ct => _sender.Send(new SetPowerControlCommand(PowerControlMode.SelfConsumeChargeOnlyMode, 0), ct));
                Console.WriteLine("OK: use mode set to SelfConsumeChargeOnly (queued)");
                break;

            case ["set", "mode", var name]:
                var useMode = ParseUseMode(name);
                if (useMode is null)
                {
                    Console.WriteLine($"Unknown mode '{name}'. Valid modes: self-use, feed-in, backup, force-time, solar-only");
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
        Console.WriteLine("  set discharge <watts>   Set battery discharge power target (0 = disable)");
        Console.WriteLine("  set charge <watts>      Set battery charge power target from grid (0 = disable)");
        Console.WriteLine("  set mode <name>         Set inverter mode: self-use | feed-in | backup | force-time | solar-only");
    }
}
