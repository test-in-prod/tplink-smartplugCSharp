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

        static bool isAdjusting = false;

        static void Main(string[] args)
        {

            var iface = new PlugInterface("192.168.40.13");
            var emeter = new EMeter(iface);

            Console.CancelKeyPress += (e, a) => { Environment.Exit(0); };

            emeter.Updated += Emeter_Updated;
            emeter.Start();

            while (true)
            {
                if (Console.KeyAvailable && emeter.VGain > 0 && emeter.IGain > 0)
                {
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            emeter.SetGain(emeter.VGain + 100, emeter.IGain);
                            isAdjusting = true;
                            break;
                        case ConsoleKey.A:
                            emeter.SetGain(emeter.VGain - 100, emeter.IGain);
                            isAdjusting = true;
                            break;
                        case ConsoleKey.W:
                            emeter.SetGain(emeter.VGain, emeter.IGain + 100);
                            isAdjusting = true;
                            break;
                        case ConsoleKey.S:
                            emeter.SetGain(emeter.VGain, emeter.IGain - 100);
                            isAdjusting = true;
                            break;
                    }
                }

                Thread.Sleep(100);
            }

        }

        private static void Emeter_Updated(EMeter emeter)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.WriteLine($"{emeter.Voltage}V  {emeter.Current}A  {emeter.Power}W  {emeter.TotalPower}kWh".PadRight(Console.BufferWidth - 1));
            Console.WriteLine($"VGain: {emeter.VGain}  IGain: {emeter.IGain}".PadRight(Console.BufferWidth - 1));
            Console.WriteLine("Q - increase VGain, A - decrease VGain");
            Console.WriteLine("W - increase IGain, S - decrease IGain");
            if (isAdjusting)
            {
                Console.WriteLine("Adjusting...");
                isAdjusting = false;
            }
            else
            {
                Console.WriteLine($"".PadRight(Console.BufferWidth - 1));
            }
        }
    }
}
