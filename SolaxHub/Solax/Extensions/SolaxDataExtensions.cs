using System.Globalization;
using System.Text;
using SolaxHub.Solax.Http;

namespace SolaxHub.Solax.Extensions
{
    internal static class SolaxDataExtensions
    {

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