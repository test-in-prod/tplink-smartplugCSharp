using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    class Program
    {
        static void Main(string[] args)
        {

            var ip = "192.168.40.13";
            var port = 9999;

            var client = new PlugInterface(ip, port);

            //var result = client.Send<GetTimeInfo>(new GetTimeInfo());

            var emeter = new EMeter(client);
            emeter.Debug();

            //client.Send(new SetRelayState() { State = true });
            //client.Close();
            //client.Open();
            //var result = client.Send<GetSystemInfo>(new GetSystemInfo());
            //client.Send(new SetNightMode() { IsOff = false });
        }

        static byte[] encrypt(byte[] value)
        {
            var key = (byte)171;
            List<byte> result = new List<byte>();
            result.AddRange(new byte[] { 0, 0, 0, 0 });
            foreach (byte i in value)
            {
                var a = (byte)(key ^ i);
                key = a;
                result.Add(a);
            }
            return result.ToArray();
        }

        static byte[] decrypt(byte[] value)
        {
            byte key = 171;
            var result = new List<byte>();
            foreach (byte i in value)
            {
                byte a = (byte)(key ^ i);
                key = i;
                result.Add(a);
            }
            return result.ToArray();
        }


    }
}
