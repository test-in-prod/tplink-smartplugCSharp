using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Handles operation of the real-time-clock on the device
    /// </summary>
    public class Clock
    {
        private static readonly object @null = null;
        private readonly PlugInterface plugInterface = null;
        private int cachedTimeZoneIndex = 0;

        #region Internal Commands
        private class GetTime : IPlugCommand, IPlugResponse
        {
            public DateTime Time
            {
                get;
                private set;
            }

            public string GetJson()
            {
                return JsonConvert.SerializeObject(new
                {
                    time = new
                    {
                        get_time = @null
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
        private class AdjustTimeZone : IPlugCommand, IPlugResponse
        {

            public int Index
            {
                get;
                set;
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
                                index = Index
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
        #endregion

        /// <summary>
        /// Gets the current date/time on the device or sets the clock
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                var time = plugInterface.Send<GetTime>(new GetTime());
                return time.Time;
            }
            set
            {
                // preserve timezone setting
                var tz = plugInterface.Send<AdjustTimeZone>(new AdjustTimeZone());
                cachedTimeZoneIndex = tz.Index;

                tz.DateTime = value;
                plugInterface.Send(tz);
            }
        }

        public Clock(PlugInterface plugInterface)
        {
            this.plugInterface = plugInterface;
        }

    }
}
