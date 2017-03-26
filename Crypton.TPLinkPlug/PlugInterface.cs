using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Provides an interface class for connecting to TP-Link SmartPlug over Ethernet
    /// </summary>
    public class PlugInterface
    {

        private readonly string connectHost = null;
        private readonly int connectPort = 0;

        /// <summary>
        /// Creates a new instance of PlugInterface to setup a future connection
        /// to a TP-Link SmartPlug on a given host address and port
        /// </summary>
        /// <param name="hostname">Host Address of a SmartPlug</param>
        /// <param name="port">Control messaging port, typically 9999</param>
        public PlugInterface(string hostname, int port = 9999)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentNullException(nameof(hostname));
            if (port < 0 || port > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException($"Port must be between 0 and {UInt16.MaxValue}");
            connectHost = hostname;
            connectPort = port;
        }

        /// <summary>
        /// Sends command to SmartPlug, not expecting a response
        /// </summary>
        /// <param name="command"></param>
        public void Send(IPlugCommand command)
        {
            Send<NullResponse>(command);
        }


        /// <summary>
        /// Sends command to SmartPlug expecting a response of a certain type
        /// </summary>
        /// <param name="command"></param>
        public TPlugResponse Send<TPlugResponse>(IPlugCommand command) where TPlugResponse : IPlugResponse, new()
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            string json = command.GetJson();

            TcpClient tcpClient = null;
            try
            {
                tcpClient = new TcpClient(connectHost, connectPort);

                if (tcpClient.Connected)
                {
                    var ns = tcpClient.GetStream();
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    var pseudoCrypted = smartPlugEncrypt(jsonBytes);
                    ns.Write(pseudoCrypted, 0, pseudoCrypted.Length);

                    Thread.Sleep(100); // give some time for device to respond

                    byte[] receiveBuffer = new byte[1024 * 2];
                    int read = ns.Read(receiveBuffer, 0, receiveBuffer.Length);

                    if (typeof(TPlugResponse) != typeof(NullResponse))
                    {
                        TPlugResponse resp = new TPlugResponse();
                        if (read > 4)
                        {
                            // read actual payload minus 4-byte header
                            byte[] actual = new byte[read - 4];

                            if (actual.Length > 0)
                            {
                                Array.Copy(receiveBuffer, 4, actual, 0, read - 4);
                                var pseudoDecrypted = smartPlugDecrypt(actual);
                                var response = Encoding.UTF8.GetString(pseudoDecrypted);

                                resp.Parse(response);
                            }
                        }
                        return resp;
                    }
                }
            }
            finally
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }

            return default(TPlugResponse);
        }

        /// <summary>
        /// Encrypts payload of bytes using TP-Link's XOR algorithm
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private byte[] smartPlugEncrypt(byte[] payload)
        {
            byte key = 171;
            byte[] header = new byte[] { 0, 0, 0, 0 };
            byte[] result = new byte[header.Length + payload.Length];
            Array.Copy(header, result, header.Length);
            for (int j = 4, i = 0; j < result.Length && i < payload.Length; j++, i++)
            {
                byte p = payload[i];
                byte a = (byte)(key ^ p);
                key = a;
                result[j] = a;
            }
            return result;
        }

        /// <summary>
        /// Decrypts payload of bytes using TP-Link's XOR algorithm
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private byte[] smartPlugDecrypt(byte[] payload)
        {
            byte key = 171;
            byte[] result = new byte[payload.Length];
            for (int i = 0; i < payload.Length; i++)
            {
                byte p = payload[i];
                byte a = (byte)(key ^ p);
                key = p;
                result[i] = a;
            }
            return result;
        }

    }
}
