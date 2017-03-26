using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    internal class NullResponse : IPlugResponse
    {
        public void Parse(string json)
        {
        }
    }
}
