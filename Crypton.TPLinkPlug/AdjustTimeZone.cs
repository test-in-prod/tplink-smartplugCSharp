using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class AdjustTimeZone : IPlugCommand, IPlugResponse
    {

        public int Index
        {
            get;
            private set;
        }

        public string Zone
        {
            get;
            private set;
        }

        public string Code
        {
            get;
            private set;
        }

        public int DstOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// If not null, will adjust date/time on device
        /// </summary>
        public DateTime? DateTime
        {
            get;
            set;
        }

        public string GetJson()
        {
            if (DateTime != null)
            {
                return JsonConvert.SerializeObject(new
                {
                    time = new
                    {
                        set_timezone = new
                        {
                            year = DateTime.Value.Year,
                            month = DateTime.Value.Month,
                            mday = DateTime.Value.Day,
                            hour = DateTime.Value.Hour,
                            min = DateTime.Value.Minute,
                            sec = DateTime.Value.Second,
                            index = 0
                        }
                    }
                });
            }
            else
            {
                object @null = null;
                return JsonConvert.SerializeObject(new
                {
                    time = new
                    {
                        get_timezone = @null
                    }
                });
            }
        }

        public void Parse(string json)
        {
            var prototype = new
            {
                time = new
                {
                    get_timezone = new
                    {
                        err_code = 0,
                        index = 0,
                        zone_str = "",
                        tz_str = "",
                        dst_offset = 0
                    },
                    set_timezone = new
                    {
                        err_code = 0,
                        err_msg = ""
                    }
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(json, prototype);

            if (result.time.get_timezone != null)
            {
                Index = result.time.get_timezone.index;
                Zone = result.time.get_timezone.zone_str;
                Code = result.time.get_timezone.tz_str;
                DstOffset = result.time.get_timezone.dst_offset;
            }

        }
    }
}
