using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class SetStationInfo : IPlugCommand, IPlugResponse
    {

        public string SSID
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public WLanKeyType KeyType
        {
            get;
            set;
        }

        public string MacAddress
        {
            get;
            private set;
        }

        public SetStationInfo()
        {

        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(new
            {
                netif = new
                {
                    set_stainfo = new
                    {
                        ssid = SSID,
                        password = Password,
                        key_type = (int)KeyType
                    }
                }
            });
        }

        public void Parse(string json)
        {
            var prototype = new
            {
                netif = new
                {
                    set_stainfo = new
                    {
                        mac = "",
                        err_code = 0,
                        err_msg = ""
                    }
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(json, prototype);

            MacAddress = result.netif.set_stainfo.mac;
        }
    }
}
