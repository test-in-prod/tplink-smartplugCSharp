using Crypton.TPLinkPlug;
using Crypton.TPLinkPlug.InfluxDBSink.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink
{
    class DeviceController
    {

        private readonly DeviceConfiguration config = null;
        private readonly PlugInterface plugInterface = null;
        private readonly EMeter emeter = null;

        public event Action<Metric> MetricReport;

        public DeviceController(DeviceConfiguration config)
        {
            this.config = config;
            plugInterface = new PlugInterface(config.Host);
            emeter = new EMeter(plugInterface);
            emeter.Updated += Emeter_Updated;
            emeter.Start();
        }

        private void Emeter_Updated(EMeter obj)
        {
            var metric = new Metric(config.Host, config.Alias, emeter.Voltage, emeter.Current, emeter.Power, emeter.TotalPower);
            MetricReport?.BeginInvoke(metric, null, null);
        }
    }
}
