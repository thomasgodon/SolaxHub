using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Solax
{
    internal enum SolaxBatteryStatus
    {
        SelfUseMode = 0,
        ForceTimeUse = 1,
        BackUpMode = 2,
        FeedInPriority = 3
    }
}
