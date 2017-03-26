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
    /// Performs commands for energy metering
    /// </summary>
    public class EMeter
    {

        private readonly PlugInterface plugInterface;

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

        private class StartCalibration : IPlugCommand, IPlugResponse
        {

            public int VTarget
            {
                get;
                set;
            }

            public int ITarget
            {
                get;
                set;
            }

            public string GetJson()
            {
                return JsonConvert.SerializeObject(new
                {
                    emeter = new
                    {
                        start_calibration = new
                        {
                            vtarget = VTarget,
                            itarget = ITarget
                        }
                    }
                });
            }

            public void Parse(string json)
            {

            }
        }
        #endregion

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

        public float Total
        {
            get;
            private set;
        }

        public int VGain
        {
            get;
            private set;
        }

        public int IGain
        {
            get;
            private set;
        }

        public EMeter(PlugInterface plugInterface)
        {
            this.plugInterface = plugInterface;
        }

        public async Task Start()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var gain = plugInterface.Send<AdjustGain>(new AdjustGain());
                    VGain = gain.VGain;
                    IGain = gain.IGain;

                    var power = plugInterface.Send<GetRealtime>(new GetRealtime());
                    Voltage = power.Voltage;
                    Current = power.Current;
                    Power = power.Power;
                    Total = power.Total;
                }
            });
        }

        public void Debug()
        {
            //  var a = plugInterface.Send<StartCalibration>(new StartCalibration() { VTarget = 16578, ITarget = 13406 });
            var gain = plugInterface.Send<AdjustGain>(new AdjustGain());
            gain.VGain = 13578;
            plugInterface.Send(gain);

            while (true)
            {
                var resp = plugInterface.Send<GetRealtime>(new GetRealtime());

                Console.WriteLine($"{resp.Current}A {resp.Voltage}V {resp.Power}W {resp.Total}kWh");
                Thread.Sleep(1000);


            }


        }

        public void SetGain(int vgain, int igain)
        {
            var setting = plugInterface.Send<AdjustGain>(new AdjustGain());
            setting.VGain = vgain;
            setting.IGain = igain;
            plugInterface.Send(setting);
        }
        

    }
}
