using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class GetTimeInfo : IPlugCommand, IPlugResponse
    {

        public DateTime Time
        {
            get;
            private set;
        }

        public string GetJson()
        {
            object a = null;
            return JsonConvert.SerializeObject(new
            {
                time = new
                {
                    get_time = a
                }
            });
        }

        public void Parse(string json)
        {
            var prototype = new
            {
                time = new
                {
                    get_time = new
                    {
                        err_code = 0,
                        year = 0,
                        month = 0,
                        mday = 0,
                        wday = 0,
                        hour = 0,
                        min = 0,
                        sec = 0
                    }
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(json, prototype);

            Time = new DateTime(
                result.time.get_time.year,
                result.time.get_time.month,
                result.time.get_time.mday,
                result.time.get_time.hour,
                result.time.get_time.min,
                result.time.get_time.sec,
                DateTimeKind.Utc);
        }
    }
}
