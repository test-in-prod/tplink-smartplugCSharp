using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.InfluxDBSink
{
    class Metric
    {

        public string Host
        {
            get;
            private set;
        }

        public string Alias
        {
            get;
            private set;
        }

        public float Voltage
        {
            get;
            private set;
        }

        public float Current
        {
            get;
            private set;
        }

        public float Power
        {
            get;
            private set;
        }

        public float TotalPower
        {
            get;
            private set;
        }

        public DateTime Time
        {
            get;
            private set;
        }

        public Metric(string host, string alias, float voltage, float current, float power, float totalPower)
        {
            Host = host;
            Alias = alias;
            Voltage = voltage;
            Current = current;
            Power = power;
            TotalPower = totalPower;
            Time = DateTime.UtcNow;
        }

        public long Timestamp
        {
            get
            {
                return (long)((Time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds * 1000 * 1000);
            }
        }

        public override string ToString()
        {
            return $"emeter,collector={Environment.MachineName},host={EscapeString(Host)},alias={EscapeString(Alias)} voltage={Voltage.ToString(CultureInfo.InvariantCulture)},current={Current.ToString(CultureInfo.InvariantCulture)},power={Power.ToString(CultureInfo.InvariantCulture)},total={TotalPower.ToString(CultureInfo.InvariantCulture)} {Timestamp}";
        }

        public static string EscapeString(string item)
        {
            return item.Replace(",", "\\,").Replace(" ", "\\ ").Replace("=", "\\=");
        }
    }
}
