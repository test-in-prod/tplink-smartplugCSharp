using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public interface IPlugResponse
    {

        void Parse(string json);

    }
}
