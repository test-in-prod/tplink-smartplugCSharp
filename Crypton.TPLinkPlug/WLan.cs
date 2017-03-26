using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Configures wireless LAN connectivity for the device
    /// </summary>
    public class WLan
    {

        private readonly PlugInterface plugInterface = null;

        #region Internal Commands
        private class GetWLanScanInfo : IPlugCommand, IPlugResponse
        {
            
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
                        get_scaninfo = new
                        {
                            ap_list = new[]
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

        private class SetStationInfo : IPlugCommand, IPlugResponse
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
        #endregion

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

        /// <summary>
        /// Performs a scan of available access points
        /// </summary>
        public IEnumerable<AccessPointEntry> AccessPointList
        {
            get
            {
                return plugInterface.Send<GetWLanScanInfo>(new GetWLanScanInfo()).AccessPointList;
            }
        }

        public WLan(PlugInterface plugInterface)
        {
            this.plugInterface = plugInterface;
        }

        /// <summary>
        /// Associates device with the specified access point, returning the device MAC address
        /// </summary>
        /// <param name="ssid">SSID of access point</param>
        /// <param name="password">AP password</param>
        /// <param name="keyType">AP key type</param>
        /// <returns></returns>
        public string AssociateWithStation(string ssid, string password, WLanKeyType keyType)
        {
            var cmd = plugInterface.Send<SetStationInfo>(new SetStationInfo() { SSID = ssid, Password = password, KeyType = keyType });
            return cmd.MacAddress;
        }

    }
}
