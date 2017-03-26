using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class GetWLanScanInfo : IPlugCommand, IPlugResponse
    {

        public class AccessPointEntry
        {
            public string SSID
            {
                get;
                internal set;
            }

            public WLanKeyType KeyType
            {
                get;
                internal set;
            }

            public override string ToString()
            {
                return $"{SSID} ({KeyType})";
            }
        }

        public IEnumerable<AccessPointEntry> AccessPointList
        {
            get;
            private set;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(new
            {
                netif = new
                {
                    get_scaninfo = new
                    {
                        refresh = 1
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
                    get_scaninfo=new
                    {
                        ap_list = new []
                        {
                            new
                            {
                                ssid = "",
                                key_type = 0
                            }
                        },
                        err_code = 0,
                        err_msg = ""
                    }
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(json, prototype);

            AccessPointList = result.netif.get_scaninfo.ap_list.Select(x => new AccessPointEntry
            {
                SSID = x.ssid,
                KeyType = (WLanKeyType)x.key_type
            });

        }
    }
}
