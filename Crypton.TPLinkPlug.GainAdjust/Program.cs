using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.GainAdjust
{
    class Program
    {
        static void Main(string[] args)
        {

            var iface = new PlugInterface("192.168.40.13");
            var emeter = new EMeter(iface);

            Console.CancelKeyPress += (e, a) => { Environment.Exit(0); };

            emeter.Start();

            while (true)
            {
                Console.Clear();

                Console.WriteLine($"{emeter.Voltage}V  {emeter.Current}A  {emeter.Power}W  {emeter.Total}kWh");
                Console.WriteLine($"VGain: {emeter.VGain}  IGain: {emeter.IGain}");
                Console.WriteLine("Q - increase VGain, A - decrease VGain");
                Console.WriteLine("W - increase IGain, S - decrease IGain");

                if (Console.KeyAvailable && emeter.VGain > 0 && emeter.IGain > 0)
                {
                    var key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            emeter.SetGain(emeter.VGain + 100, emeter.IGain);
                            break;
                        case ConsoleKey.A:
                            emeter.SetGain(emeter.VGain - 100, emeter.IGain);
                            break;
                        case ConsoleKey.W:
                            emeter.SetGain(emeter.VGain, emeter.IGain + 100);
                            break;
                        case ConsoleKey.S:
                            emeter.SetGain(emeter.VGain, emeter.IGain - 100);
                            break;
                    }
                }

                Thread.Sleep(1000);

            }

        }
    }
}
