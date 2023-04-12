using SolaxHub.Solax.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Solax.Modbus
{
    internal static class SolaxModbusExtensions
    {
        public static SolaxData ToSolaxData(this SolaxModbusData result) => new();
    }
}
