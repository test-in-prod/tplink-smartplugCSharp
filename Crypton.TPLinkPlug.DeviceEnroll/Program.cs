using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug.DeviceEnroll
{
    class Program
    {

        static EnrollProfile currentProfile = null;



        static void Main(string[] args)
        {
            currentProfile = new EnrollProfile();

            var options = new OptionSet();
            options.Add("profile=", "Specifies device enrollment profile", (string profile) =>
            {
                if (File.Exists(profile))
                {
                    string filecontent = File.ReadAllText(profile);
                    try
                    {
                        currentProfile = JsonConvert.DeserializeObject<EnrollProfile>(filecontent);
                    }
                    catch (Exception any)
                    {
                        Console.WriteLine($"tpsp-enroll: invalid profile json: {any.Message}");
                        Environment.Exit(1);
                    }
                    Console.WriteLine($"Loaded profile JSON: {profile}");
                }
            });
            options.Add("ip=", "Specifies device IP for control (192.168.0.1 is default)", (string ip) =>
            {
                currentProfile.defaults.ip = ip;
            });
            options.Add("ssid=", "Specifies connection AP SSID", (string ssid) =>
            {
                currentProfile.defaults.ssid = ssid;
            });
            options.Add("password=", "Specifies AP password", (string password) =>
            {
                currentProfile.defaults.password = password;
            });
            options.Add("enc=", "Specifies AP encryption type (Open, WEP, WPA, WPA2)", (string enctype) =>
            {
                currentProfile.defaults.encryptionType = (WLanKeyType)Enum.Parse(typeof(WLanKeyType), enctype);
            });
            options.Add("alias=", "Specifies device alias", (string alias) =>
            {
                currentProfile.defaults.alias = alias;
            });

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
                Console.WriteLine($"tpsp-enroll: {ex.Message}");
                ShowUsage(options);
                return;
            }

            currentProfile.EnrollDevices();
        }

        private static void ShowUsage(OptionSet p)
        {
            Console.WriteLine("Usage: tpsp-enroll OPTIONS");
            Console.WriteLine("Enroll a TP-Link SmartPlug device onto a network of your choice");
            Console.WriteLine("without pesky phone apps and stormy clouds");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
