using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class SetNightMode : IPlugCommand
    {

        public bool IsOff
        {
            get;
            set;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(new
            {
                system = new
                {
                    set_led_off = new
                    {
                        off = IsOff ? 1 : 0
                    }
                }
            });
        }
    }
}
