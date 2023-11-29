using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Knx.Client
{
    internal class KnxClient : IKnxClient
    {
        public Task SendValuesAsync(IEnumerable<KnxSolaxValue> values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
