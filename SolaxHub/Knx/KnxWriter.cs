using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolaxHub.Knx.Client;
using SolaxHub.Solax;

namespace SolaxHub.Knx
{
    internal class KnxWriter : ISolaxWriter
    {
        private readonly IKnxClient _knxClient;
        private ISolaxClient? _solaxClient;

        public KnxWriter(IKnxClient knxClient)
        {
            _knxClient = knxClient;
        }

        public void SetSolaxClient(ISolaxClient solaxClient)
        {
            _solaxClient = solaxClient;
        }
    }
}
