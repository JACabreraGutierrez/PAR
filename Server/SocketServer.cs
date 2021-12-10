using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class SocketServer
    {
        private int _port;
        private int _backlog;
        private IPAddress _ipAddress;
        internal void Start()
        {
            IPEndPoint endPoint = new IPEndPoint(_ipAddress, _port);
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(_backlog);

            Task.Run(() => ProcessRequest(socket));
        }

        private async void ProcessRequest(Socket socket)
        {
            do
            {
                Socket clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                using (NetworkStream stream = new NetworkStream(clientSocket, true))
                {
                    var buffer = new byte[1024];
                    do
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (bytesRead == 0)
                            break;
                        string stringRead = System.Text.Encoding.UTF8.GetString(buffer);

                        Response response = ResponseBuilder.CreateResponse(stringRead);
                        string responseJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        byte[] bytesResponse = Encoding.UTF8.GetBytes(responseJson);

                        await stream.WriteAsync(bytesResponse, 0, bytesResponse.Length).ConfigureAwait(false);
                    } while (true);
                }
            } while (true);
        }
    }
}
