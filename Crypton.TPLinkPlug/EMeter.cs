using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Provides a class for automatic energy monitoring
    /// </summary>
    public class EMeter : IDisposable
    {
        internal static readonly object @null = null;

        private readonly PlugInterface plugInterface;
        private TimeSpan reportInterval;
        private Timer fetchReportTimer;
        private readonly object lockHandle = new object();

        #region Internal command classes
        private class GetRealtime : IPlugCommand, IPlugResponse
        {
            public float Current
            {
                get;
                private set;
            }

            public float Voltage
            {
                get;
                private set;
            }

            public float Power
            {
                get; private set;
            }

            public float Total
            {
                get;
                private set;
            }

            public string GetJson()
            {
                return JsonConvert.SerializeObject(new { emeter = new { get_realtime = new { } } });
            }

            public void Parse(string json)
            {
                var prototype = new
                {
                    emeter = new
                    {
                        get_realtime = new
                        {
                            current = 0.0f,
                            voltage = 0.0f,
                            power = 0.0f,
                            total = 0.0f,
                            err_code = 0,
                            err_msg = ""
                        }
                    }
                };

                var result = JsonConvert.DeserializeAnonymousType(json, prototype);

                Current = result.emeter.get_realtime.current;
                Voltage = result.emeter.get_realtime.voltage;
                Power = result.emeter.get_realtime.power;
                Total = result.emeter.get_realtime.total;

            }
        }

        private class AdjustGain : IPlugCommand, IPlugResponse
        {
            private int vgain, igain;
            private bool reset = false;

            public int VGain
            {
                get { return vgain; }
                set { vgain = value; reset = true; }
            }

            public int IGain
            {
                get { return igain; }
                set { igain = value; reset = true; }
            }

            public string GetJson()
            {
                if (reset)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        emeter = new
                        {
                            set_vgain_igain = new
                            {
                                vgain = vgain,
                                igain = igain
                            }
                        }
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        emeter = new
                        {
                            get_vgain_igain = new { }
                        }
                    });
                }
            }

            public void Parse(string json)
            {
                var prototype = new
                {
                    emeter = new
                    {
                        get_vgain_igain = new
                        {
                            vgain = 0,
                            igain = 0,
                            err_code = 0
                        }
                    }
                };

                var result = JsonConvert.DeserializeAnonymousType(json, prototype);

                vgain = result.emeter.get_vgain_igain.vgain;
                igain = result.emeter.get_vgain_igain.igain;
            }
        }

        private class ClearStats : IPlugCommand
        {
            public string GetJson()
            {

                return JsonConvert.SerializeObject(new
                {
                    emeter = new
                    {
                        erase_emeter_stat = @null
                    }
                });
            }
        }

        #endregion

        /// <summary>
        /// Gets the current line voltage
        /// </summary>
        public float Voltage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the energy current in amperes
        /// </summary>
        public float Current
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current energy consumption in watts
        /// </summary>
        public float Power
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total counted power in kilowatt-hours
        /// </summary>
        public float TotalPower
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the voltage gain
        /// </summary>
        public int VGain
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current gain
        /// </summary>
        public int IGain
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets reporting interval
        /// </summary>
        public TimeSpan RefreshInterval
        {
            get { return reportInterval; }
            set
            {
                if (fetchReportTimer != null)
                    throw new InvalidOperationException("RefreshInterval cannot be adjusted while the timer is running; Stop the timer, adjust, and Start it again");
                if (reportInterval.TotalMilliseconds >= 100)
                {
                    reportInterval = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ReportInterval must be at least 100 milliseconds");
                }
            }
        }

        /// <summary>
        /// Fires when EMeter counts have been updated
        /// </summary>
        public event Action<EMeter> Updated;

        /// <summary>
        /// Fires when there is an exception while running the EMeter
        /// </summary>
        public event Action<EMeter, Exception> Exception;

        public EMeter(PlugInterface plugInterface)
        {
            this.plugInterface = plugInterface;
            this.reportInterval = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Starts E-meter interval timer which will gather statistics at specified RefreshInterval
        /// </summary>
        public void Start()
        {
            if (fetchReportTimer == null)
            {
                fetchReportTimer = new Timer(fetchReportOnInterval, null, 0, (int)reportInterval.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Stops the E-meter interval timer
        /// </summary>
        public void Stop()
        {
            if (fetchReportTimer != null)
            {
                fetchReportTimer.Dispose();
                fetchReportTimer = null;
            }
        }

        private void fetchReportOnInterval(object state)
        {
            lock (lockHandle)
            {
                try
                {
                    var gain = plugInterface.Send<AdjustGain>(new AdjustGain());

                    VGain = gain.VGain;
                    IGain = gain.IGain;

                    var usage = plugInterface.Send<GetRealtime>(new GetRealtime());

                    Current = usage.Current;
                    Voltage = usage.Voltage;
                    Power = usage.Power;
                    TotalPower = usage.Total;

                    Updated?.BeginInvoke(this, null, null);
                }
                catch (Exception any)
                {
                    Exception?.Invoke(this, any);
                }
            }
        }
                
        /// <summary>
        /// Issues a command to clear energy metering statistics on the device
        /// </summary>
        public void ClearUsage()
        {
            plugInterface.Send(new ClearStats());
        }

        /// <summary>
        /// Adjusts energy meter gain
        /// </summary>
        /// <param name="vgain"></param>
        /// <param name="igain"></param>
        public void SetGain(int vgain, int igain)
        {
            if (vgain > 0 && igain > 0)
            {
                var setting = plugInterface.Send<AdjustGain>(new AdjustGain());
                setting.VGain = vgain;
                setting.IGain = igain;
                plugInterface.Send(setting);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"vgain and igain must be greater than 0");
            }
        }

        /// <summary>
        /// Disposes the automatic metering timer
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
