using System.Globalization;
using System.Text;
using SolaxHub.Solax.Http;

namespace SolaxHub.Solax.Extensions
{
    internal static class TelegramExtensions
    {

        public static byte[] ToUdpPacket(this SolaxData result, string property)
        {
            var value = result.GetType().GetProperty(property)?.GetValue(result, null);
            var subValue = value?.GetType().GetProperty("Value")?.GetValue(value, null);
            var subSubValue = subValue?.GetType().GetProperty("Value")?.GetValue(subValue, null);

            return Encoding.UTF8.GetBytes(subSubValue?.ToInvariantString() ?? subValue?.ToInvariantString() ?? value?.ToInvariantString() ?? string.Empty);
        }

        private static string? ToInvariantString(this object value)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString(CultureInfo.InvariantCulture);
            }
            return value.ToString();
        }
    }
}