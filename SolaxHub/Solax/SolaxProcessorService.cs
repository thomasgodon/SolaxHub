﻿using Newtonsoft.Json;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax;

internal class SolaxProcessorService : ISolaxProcessorService
{
    private readonly ILogger<SolaxProcessorService> _logger;
    private readonly IEnumerable<ISolaxConsumer> _solaxProcessors;

    public SolaxProcessorService(ILogger<SolaxProcessorService> logger, IEnumerable<ISolaxConsumer> solaxProcessors)
    {
        _logger = logger;
        _solaxProcessors = solaxProcessors;
    }

    public async Task ProcessData(SolaxData data, CancellationToken cancellationToken)
    {
        _logger.LogTrace("{log}", JsonConvert.SerializeObject(data));

        try
        {
            foreach (var solaxProcessor in _solaxProcessors)
            {
                await solaxProcessor.ProcessData(data, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message}", e.Message);
        }
    }
}