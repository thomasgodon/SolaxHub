﻿namespace SolaxHub.Solax;

internal interface ISolaxProcessorService
{
    Task ProcessData(SolaxData data, CancellationToken cancellationToken);
}