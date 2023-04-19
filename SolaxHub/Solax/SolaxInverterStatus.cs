namespace SolaxHub.Solax
{
    internal enum SolaxInverterStatus
    {
        WaitMode = 100,
        CheckMode = 101,
        NormalMode = 102,
        FaultMode = 103,
        PermanentFaultMode = 104,
        UpdateMode = 105,
        EpsCheckMode = 106,
        EpsMode = 107,
        SelfTestMode = 108,
        IdleMode = 109,
        StandbyMode = 110,
        PvWakeUpBatMode = 111,
        GenCheckMode = 112,
        GenRunMode = 113
    }
}
