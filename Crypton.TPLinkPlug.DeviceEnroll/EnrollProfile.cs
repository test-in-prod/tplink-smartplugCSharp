using Crypton.TPLinkPlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.DeviceEnroll
{

    /// <summary>
    /// Defines an enrollment profile file containing several devices
    /// </summary>
    public class EnrollProfile
    {

        /// <summary>
        /// Defines a profile entry for each device
        /// </summary>
        public class DeviceSettings
        {
            /// <summary>
            /// Matches device by its MAC address
            /// </summary>
            public string mac
            {
                get;
                set;
            }

            /// <summary>
            /// Defines device IP for configuration (ignored for actual devices as they are DHCP)
            /// </summary>
            public string ip
            {
                get;
                set;
            }

            /// <summary>
            /// Defines device SSID to connect to
            /// </summary>
            public string ssid
            {
                get;
                set;
            }

            /// <summary>
            /// Defines AP station password
            /// </summary>
            public string password
            {
                get;
                set;
            }

            /// <summary>
            /// Defines AP encryption type
            /// </summary>
            public WLanKeyType? encryptionType
            {
                get;
                set;
            }

            /// <summary>
            /// Defines device alias (e.g. name of device for identification)
            /// </summary>
            public string alias
            {
                get;
                set;
            }

            /// <summary>
            /// Sets device time to local computer's time
            /// </summary>
            public bool setTime
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Specifies defaults to use for all devices
        /// </summary>
        public DeviceSettings defaults
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies list of devices to enroll
        /// </summary>
        public List<DeviceSettings> devices
        {
            get;
            set;
        }

        public EnrollProfile()
        {
            defaults = new DeviceSettings();
            devices = new List<DeviceSettings>();
        }

        public void EnrollDevices()
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Single-device enrollment");
                devices.Add(new DeviceSettings
                {
                    alias = defaults.alias,
                    encryptionType = defaults.encryptionType,
                    ip = defaults.ip,
                    mac = defaults.mac,
                    password = defaults.password,
                    setTime = defaults.setTime,
                    ssid = defaults.ssid
                });
            }
            else
            {
                Console.WriteLine("Multi-device enrollment");
            }

            foreach (var device in devices)
            {
                EnrollDevice(device);
            }

        }

        private bool EnrollDevice(DeviceSettings device)
        {
            string alias = device.alias ?? defaults.alias;
            string mac = device.mac ?? defaults.mac;
            string ip = device.ip ?? defaults.ip;
            string ssid = device.ssid ?? defaults.ssid;
            string password = device.password ?? defaults.password;
            var enctype = device.encryptionType ?? defaults.encryptionType ?? WLanKeyType.WPA;

            var iface = new PlugInterface(ip);
            try
            {
                var sysconfig = new PlugSystem(iface);
                var clock = new Clock(iface);
                var wlan = new WLan(iface);
                sysconfig.Refresh();

                if (!string.IsNullOrEmpty(mac))
                {
                    if (mac != sysconfig.MacAddress)
                    {
                        Console.WriteLine($"Skipping device (mac mismatch {mac} != {sysconfig.MacAddress}");
                        return true;
                    }
                }
                Console.WriteLine($"MAC={sysconfig.MacAddress} DID={sysconfig.DeviceId} FID={sysconfig.FirmwareId} Alias={sysconfig.Alias}");

                if (!string.IsNullOrEmpty(alias))
                {
                    Console.Write("Setting alias...");
                    sysconfig.SetAlias(alias);
                    Console.WriteLine("OK");
                }

                Console.Write("Setting date/time...");
                clock.DateTime = DateTime.Now;
                Console.WriteLine("OK");

                Console.Write("Setting WLAN params...");
                wlan.AssociateWithStation(ssid, password, enctype);
                Console.WriteLine("OK");
                return true;
            }
            catch (Exception any)
            {
                Console.WriteLine($"error: {any.Message}");
                return false;
            }


        }

    }
}
