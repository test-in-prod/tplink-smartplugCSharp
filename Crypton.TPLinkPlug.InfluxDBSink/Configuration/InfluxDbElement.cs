using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink.Configuration
{
    public class InfluxDbElement : ConfigurationElement
    {

        [ConfigurationProperty("writeUrl", IsRequired = true)]
        public string WriteUrl
        {
            get { return (string)base["writeUrl"]; }
            set { base["writeUrl"] = value; }
        }

    }
}
