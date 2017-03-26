using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class SetRelayState : IPlugCommand
    {
        
        public bool State
        {
            get;
            set;
        }

        public SetRelayState()
        {
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(new
            {
                system = new
                {
                    set_relay_state = new
                    {
                        state = State ? 1 : 0
                    }
                }
            });
        }
    }
}
