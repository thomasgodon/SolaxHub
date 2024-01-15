﻿using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Client;

internal interface IKnxClient
{
    Task SendValuesAsync(IEnumerable<KnxValue> values, CancellationToken cancellationToken);
    Task ConnectAsync(CancellationToken cancellationToken);
    void SetValueReadDelegate(IKnxValueReadDelegate @delegate);
    void SetValueWriteDelegate(IKnxValueWriteDelegate @delegate);
}