using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Client
{
    public class SocketClient
    {
        private IPEndPoint _endPoint;
        private Socket _socket;
        private NetworkStream _stream;

        public SocketClient()
        {
            var ipAddress = IPAddress.TryParse(ConfigurationManager.AppSettings["IPAddress"], out IPAddress outIpAddress) ? outIpAddress : IPAddress.Loopback;
            var port = Int32.TryParse(ConfigurationManager.AppSettings["Port"], out int outPort) ? outPort : 8500;
            _endPoint = new IPEndPoint(ipAddress, port);
        }

        public void Start()
        {
            _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_endPoint);
            _stream = new NetworkStream(_socket, true);
        }

        public async void SendMessage(string message)
        {
            byte[] buffer  = System.Text.Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }
        public async Task<T> ReceiveMessage<T>()
        {
            string content = await ReadAsync();
            return Decoder<T>(content);
        }
        public void Disconnect()
        {
            _socket.Disconnect(true);
            _socket.Dispose();
        }
        private async Task<string> ReadAsync()
        {
            string result = string.Empty;

            try
            {
                var buffer = new byte[1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int numBytesRead = 0;
                    do
                    {
                        int previusNumBytes = numBytesRead;
                        numBytesRead = _stream.Read(buffer, numBytesRead, buffer.Length);
                        ms.Write(buffer, previusNumBytes, numBytesRead);
                    } while (numBytesRead == buffer.Length);
                    result = System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
            }catch (Exception) { }

            return result;
        }

        private T Decoder<T>(string body)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
        }
    }
}
