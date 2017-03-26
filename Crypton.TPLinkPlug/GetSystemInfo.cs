using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class GetSystemInfo : IPlugCommand, IPlugResponse
    {

        
        public int ErrorCode
        {
            get;
            private set;
        }

        
        public string SoftwareVersion
        {
            get;
            private set;
        }

        
        public string HardwareVersion
        {
            get;
            private set;
        }

        
        public string Type
        {
            get;
            private set;
        }

        
        public string Model
        {
            get;
            private set;
        }

        
        public string MacAddress
        {
            get;
            private set;
        }

        
        public string DeviceId
        {
            get;
            private set;
        }

        
        public string HardwareId
        {
            get;
            private set;
        }

        
        public string FirmwareId
        {
            get;
            private set;
        }

        
        public string OemId
        {
            get;
            private set;
        }

        
        public string Alias
        {
            get;
            private set;
        }

        
        public string DeviceName
        {
            get;
            private set;
        }

        
        public string IconHash
        {
            get;
            private set;
        }

        
        public bool RelayState
        {
            get;
            private set;
        }

        
        public string ActiveMode
        {
            get; private set;
        }

        
        public string Feature
        {
            get;
            private set;
        }

        
        public bool IsUpdating
        {
            get;
            private set;
        }

        
        public bool IsLedOff
        {
            get;
            private set;
        }

        
        public float Latitude
        {
            get;
            private set;
        }

        
        public float Longitude
        {
            get;
            private set;
        }

        
        public int RSSI
        {
            get;
            private set;
        }

        public int OnTime
        {
            get;set;
        }

        public GetSystemInfo()
        {
        }

        public string GetJson()
        {
            return "{\"system\":{\"get_sysinfo\":null}}";
        }

        public void Parse(string json)
        {
            var responsePrototype = new
            {
                system = new
                {
                    get_sysinfo = new
                    {
                        err_code = 0,
                        sw_ver = "",
                        hw_ver = "",
                        type = "",
                        model = "",
                        mac = "",
                        deviceId = "",
                        hwId = "",
                        fwId = "",
                        oemId = "",
                        alias = "",
                        dev_name = "",
                        icon_hash = "",
                        relay_state = 0,
                        on_time = 0,
                        active_mode = "",
                        feature = "",
                        updating = 0,
                        rssi = 0,
                        led_off = 0,
                        latitude = 0.0f,
                        longitude = 0.0f
                    }
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(json, responsePrototype);

            // TODO

        }
    }
}
