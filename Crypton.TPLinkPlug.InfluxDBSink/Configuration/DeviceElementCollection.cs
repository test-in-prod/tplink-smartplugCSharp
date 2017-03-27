using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink.Configuration
{
    public class DeviceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DeviceConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as DeviceConfiguration).Host;
        }
    }
}
