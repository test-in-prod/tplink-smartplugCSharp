using Mono.Options;
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

        static string IPAddress = null;

        static void Main(string[] args)
        {
            var options = new OptionSet();
            options.Add("ip=", "IP address of device", (string ip) => { IPAddress = ip; });

            if (args.Length == 0)
            {
                ShowUsage(options);
                return;
            }

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message);
                ShowUsage(options);
                return;
            }

            Console.Clear();

            var iface = new PlugInterface(IPAddress);
            var emeter = new EMeter(iface);

            Console.CancelKeyPress += (e, a) => { Environment.Exit(0); };

            emeter.Updated += Emeter_Updated;
            emeter.Start();

            while (true)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.WriteLine("Press CTRL+C to exit");
                Console.WriteLine();
                Console.WriteLine($"\t{emeter.Voltage}V  {emeter.Current}A  {emeter.Power}W  {emeter.TotalPower}kWh".PadRight(Console.BufferWidth - 1));
                Console.WriteLine($"\tVGain: {emeter.VGain}  IGain: {emeter.IGain}".PadRight(Console.BufferWidth - 1));
                Console.WriteLine();
                Console.WriteLine("Q - increase VGain, A - decrease VGain");
                Console.WriteLine("W - increase IGain, S - decrease IGain");

                if (Console.KeyAvailable && emeter.VGain > 0 && emeter.IGain > 0)
                {
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            Console.WriteLine("Adjusting...");
                            emeter.SetGain(emeter.VGain + 100, emeter.IGain);
                            break;
                        case ConsoleKey.A:
                            Console.WriteLine("Adjusting...");
                            emeter.SetGain(emeter.VGain - 100, emeter.IGain);
                            break;
                        case ConsoleKey.W:
                            Console.WriteLine("Adjusting...");
                            emeter.SetGain(emeter.VGain, emeter.IGain + 100);
                            break;
                        case ConsoleKey.S:
                            Console.WriteLine("Adjusting...");
                            emeter.SetGain(emeter.VGain, emeter.IGain - 100);
                            break;
                    }
                    Console.Clear();
                }

                Thread.Sleep(100);
            }

        }

        private static void ShowUsage(OptionSet options)
        {
            Console.WriteLine("Usage: tpsp-gain --ip=IPAddress");
            Console.WriteLine("Adjusts voltage and current gain of device");
            Console.WriteLine("to provide more accurate readings");
            Console.WriteLine();
            Console.WriteLine("When adjusting, connect a constant resistive load (e.g. a lamp)");
            Console.WriteLine("to the device through a calibrated VA meter (e.g. ");
            Console.WriteLine("Kill-A-Watt, AC clamp meter, multimeter, or equivalent)");
            Console.WriteLine("Press Q/A, W/S keyboard buttons and slowly adjust gain");
            Console.WriteLine("until V/A/W readings are close to your independent instrument");
        }

        private static void Emeter_Updated(EMeter emeter)
        {

        }
    }
}
