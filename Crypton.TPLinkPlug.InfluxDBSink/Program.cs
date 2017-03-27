using Crypton.TPLinkPlug.InfluxDBSink.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink
{
    class Program
    {

        static InfluxDbSubmitter submitter = null;
        static List<DeviceController> devices = new List<DeviceController>();

        static void Main(string[] args)
        {

            var config = Configuration.MonitorSection.Current;

            submitter = new InfluxDbSubmitter(config.InfluxDb);

            foreach (DeviceConfiguration deviceConfig in config.Devices)
            {
                var controller = new DeviceController(deviceConfig);
                controller.MetricReport += (metric) => { submitter.Add(metric); };
                devices.Add(controller);
            }

            Console.CancelKeyPress += (a, b) => { Environment.Exit(0); };

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
