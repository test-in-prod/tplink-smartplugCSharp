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

            TcpClient sock_tcp = new TcpClient(ip, port);
            sock_tcp.ReceiveBufferSize = 1024 * 16;

           // var cmd = "{\"netif\":{\"set_stainfo\":{\"ssid\":\"InternetOfShit\",\"password\":\"_qZ4XHxoFYlg60S\",\"key_type\":3}}}";
            var crypted = encrypt(Encoding.UTF8.GetBytes(cmd));
            byte[] recvBuffer = new byte[2048];

            var ns = sock_tcp.GetStream();
            ns.Write(crypted, 0, crypted.Length);
            Thread.Sleep(1000);

            var read = ns.Read(recvBuffer, 0, recvBuffer.Length);

            // actual bytes received
            byte[] actual = new byte[read - 4];

            // skip 4-byte header and a 1-byte tail at the end
            Array.Copy(recvBuffer, 4, actual, 0, read - 4);
            sock_tcp.Close();

            var decrypted = decrypt(actual);
            var response = Encoding.UTF8.GetString(decrypted);
            Console.WriteLine($"Sent: {cmd}");

            Console.WriteLine($"Recv: {response}");


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
