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

        [ConfigurationProperty("flushInterval", IsRequired = false, DefaultValue = 5000)]
        public int FlushInterval
        {
            get { return (int)base["flushInterval"]; }
            set { base["flushInterval"] = value; }
        }

        [ConfigurationProperty("maxBatchSize", IsRequired = false, DefaultValue = 2500)]
        public int MaxBatchSize
        {
            get { return (int)base["maxBatchSize"]; }
            set { base["maxBatchSize"] = value; } 
        }

    }
}
