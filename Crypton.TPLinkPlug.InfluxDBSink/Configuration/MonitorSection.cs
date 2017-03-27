using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink.Configuration
{
    public class MonitorSection : ConfigurationSection
    {

        public static MonitorSection Current
        {
            get
            {
                return ConfigurationManager.GetSection("monitor") as MonitorSection;
            }
        }

        [ConfigurationProperty("influxdb", IsRequired = true)]
        public InfluxDbElement InfluxDb
        {
            get { return (InfluxDbElement)base["influxdb"]; }
            set { base["influxdb"] = value; }
        }

        [ConfigurationProperty("devices", IsRequired = true)]
        public DeviceElementCollection Devices
        {
            get { return (DeviceElementCollection)base["devices"]; }
            set { base["devices"] = value; }
        }


    }
}
