using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Solax.Models
{
    internal enum SolaxRemoteControlPowerControlMode
    {
        Disabled = 0,
        EnabledPowerControl = 1, // battery charge level in absence of PV
        EnabledGridControl = 2, // computed variation of Power Control, grid import level in absence of PV
        EnabledBatteryControl = 3, // computed variation of Power Control, battery import without of PV
        EnabledFeedInPriority = 4, // variation of Battery Control with fixed target 0
        EnabledNoDischarge = 5, // missing HL from grid
        EnabledQuantityControl = 6,
        EnabledSocTargetControl = 7
    }
}