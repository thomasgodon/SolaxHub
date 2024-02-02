using SolaxHub.Knx.Models;
using SolaxHub.Solax.Models;

namespace SolaxHub.Knx.Extensions
{
    internal static class KnxExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetReadGroupAddressesFromOptions(this KnxOptions options)
            => options.ReadGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);

        public static IEnumerable<KeyValuePair<string, string>> GetWriteGroupAddressesFromOptions(this KnxOptions options)
            => options.WriteGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);

        public static int ToNormalizedLockState(this SolaxLockState state)
            => state switch
            {
                SolaxLockState.Unlocked => 1,
                SolaxLockState.UnlockedAdvanced => 2,
                _ => 0,
            };
    }
}
