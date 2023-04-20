using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knx;
using Knx.DatapointTypes;
using Knx.DatapointTypes.Dpt2ByteFloat;
using Knx.DatapointTypes.Dpt2ByteSignedValue;
using Knx.DatapointTypes.Dpt8BitSignedValue;
using SolaxHub.Solax;

namespace SolaxHub.Knx.Extensions
{
    internal static class KnxExtensions
    {
        public static IEnumerable<(KnxLogicalAddress address, DatapointType type)> ToKnxMessages(this SolaxData data, KnxLogicalAddress startKnxLogicalAddress)
        {
            yield return new(
                new KnxLogicalAddress(
                    startKnxLogicalAddress.Group,
                    startKnxLogicalAddress.MiddleGroup,
                    (byte)startKnxLogicalAddress.SubGroup),
                new DptPower(data.FeedInEnergy));

            yield return new(
                new KnxLogicalAddress(
                    startKnxLogicalAddress.Group,
                    startKnxLogicalAddress.MiddleGroup,
                    (byte)(startKnxLogicalAddress.SubGroup + 1)),
                new DptPower(data.HouseLoad));

            yield return new(
                new KnxLogicalAddress(
                    startKnxLogicalAddress.Group,
                    startKnxLogicalAddress.MiddleGroup,
                    (byte)(startKnxLogicalAddress.SubGroup + 2)),
                new DptPercentV16((short)data.Soc));
        }
    }
}
