using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Provides operational information and commands of the device
    /// </summary>
    public class PlugSystem
    {

        private readonly PlugInterface plugInterface = null;

        #region Internal Commands 
        private class GetSystemInfo : IPlugCommand, IPlugResponse
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
                get;
                private set;
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
                var prototype = new
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

                var result = JsonConvert.DeserializeAnonymousType(json, prototype);

                SoftwareVersion = result.system.get_sysinfo.sw_ver;
                HardwareVersion = result.system.get_sysinfo.hw_ver;
                Type = result.system.get_sysinfo.type;
                Model = result.system.get_sysinfo.model;
                MacAddress = result.system.get_sysinfo.mac;
                DeviceId = result.system.get_sysinfo.deviceId;
                HardwareId = result.system.get_sysinfo.hwId;
                FirmwareId = result.system.get_sysinfo.fwId;
                OemId = result.system.get_sysinfo.oemId;
                Alias = result.system.get_sysinfo.alias;
                DeviceName = result.system.get_sysinfo.dev_name;
                IconHash = result.system.get_sysinfo.icon_hash;
                RelayState = result.system.get_sysinfo.relay_state == 1 ? true : false;
                OnTime = result.system.get_sysinfo.on_time;
                ActiveMode = result.system.get_sysinfo.active_mode;
                Feature = result.system.get_sysinfo.feature;
                IsUpdating = result.system.get_sysinfo.updating == 1 ? true : false;
                RSSI = result.system.get_sysinfo.rssi;
                IsLedOff = result.system.get_sysinfo.led_off == 1 ? true : false;
                Latitude = result.system.get_sysinfo.latitude;
                Longitude = result.system.get_sysinfo.longitude;
            }
        }
        private class SetAliasCommand : IPlugCommand
        {

            public string Alias
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
                        set_dev_alias = new
                        {
                            alias = Alias ?? string.Empty
                        }
                    }
                });
            }
        }

        private class RebootCmd : IPlugCommand
        {

            public int Delay
            {
                get;
                set;
            } = 1;
            public string GetJson()
            {
                return JsonConvert.SerializeObject(new
                {
                    system = new
                    {
                        reboot = new
                        {
                            delay = Delay
                        }
                    }
                });
            }
        }

        private class SetRelayState : IPlugCommand
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

        private class FactoryResetCmd : IPlugCommand
        {

            public int Delay
            {
                get;
                set;
            } = 1;
            public string GetJson()
            {
                return JsonConvert.SerializeObject(new
                {
                    system = new
                    {
                        reset = new
                        {
                            delay = Delay
                        }
                    }
                });
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the software version string
        /// </summary>
        public string SoftwareVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the hardware version string
        /// </summary>
        public string HardwareVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the produce type string
        /// </summary>
        public string Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the model string
        /// </summary>
        public string Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the MAC address
        /// </summary>
        public string MacAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device ID
        /// </summary>
        public string DeviceId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the hardware ID
        /// </summary>
        public string HardwareId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the firmware ID
        /// </summary>
        public string FirmwareId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the OEM ID
        /// </summary>
        public string OemId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device alias
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device name
        /// </summary>
        public string DeviceName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device icon hash key
        /// </summary>
        public string IconHash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the relay state (plug output on or off)
        /// </summary>
        public bool RelayState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device mode
        /// </summary>
        public string ActiveMode
        {
            get; private set;
        }

        /// <summary>
        /// Gets device feature codes
        /// </summary>
        public string Feature
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether firmware update is in progress
        /// </summary>
        public bool IsUpdating
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether physical button LED is on or off
        /// </summary>
        public bool IsLedOff
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets user-defined latitude
        /// </summary>
        public float Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets user-defined longitude
        /// </summary>
        public float Longitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets signal strength in dBm
        /// </summary>
        public int RSSI
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the amount of time the relay (end device) has been on
        /// </summary>
        public TimeSpan OnTime
        {
            get; set;
        }
        #endregion

        public PlugSystem(PlugInterface plugInterface)
        {
            this.plugInterface = plugInterface;
        }


        /// <summary>
        /// Refreshes information from device for current PlugSystem instance
        /// </summary>
        public void Refresh()
        {
            var query = plugInterface.Send<GetSystemInfo>(new GetSystemInfo());
            SoftwareVersion = query.SoftwareVersion;
            HardwareVersion = query.HardwareVersion;
            Type = query.Type;
            Model = query.Model;
            MacAddress = query.MacAddress;
            DeviceId = query.DeviceId;
            HardwareId = query.HardwareId;
            FirmwareId = query.FirmwareId;
            OemId = query.OemId;
            Alias = query.Alias;
            DeviceName = query.DeviceName;
            IconHash = query.IconHash;
            RelayState = query.RelayState;
            ActiveMode = query.ActiveMode;
            Feature = query.Feature;
            IsUpdating = query.IsUpdating;
            IsLedOff = query.IsLedOff;
            Latitude = query.Latitude;
            Longitude = query.Longitude;
            RSSI = query.RSSI;
            OnTime = TimeSpan.FromSeconds(query.OnTime);
        }

        public void SetLocation(float lat, float lon)
        {

        }

        /// <summary>
        /// Sets device alias
        /// </summary>
        /// <param name="alias"></param>
        public void SetAlias(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                plugInterface.Send(new SetAliasCommand() { Alias = alias });
                Alias = alias;
            }
        }

        /// <summary>
        /// Reboots device with a delay
        /// </summary>
        /// <param name="delaySeconds"></param>
        public void Reboot(int delaySeconds = 1)
        {
            if (delaySeconds >= 0)
            {
                plugInterface.Send(new RebootCmd() { Delay = delaySeconds });
            }
        }

        /// <summary>
        /// Performs a factory reset of the device
        /// </summary>
        /// <param name="delaySeconds"></param>
        public void FactoryReset(int delaySeconds = 1)
        {
            if(delaySeconds >= 0)
            {
                plugInterface.Send(new FactoryResetCmd() { Delay = delaySeconds });
            }
        }

        /// <summary>
        /// Sets relay state (true for on, false for off)
        /// </summary>
        /// <param name="state"></param>
        public void SetRelay(bool state)
        {
            plugInterface.Send(new SetRelayState() { State = state });
        }

    }
}
