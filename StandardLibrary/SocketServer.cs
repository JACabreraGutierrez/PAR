﻿using StandardLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class SocketServer
    {
        private Socket _socket;
        private IPEndPoint _endPoint;
        private CancellationTokenSource _cancelToken;
        private ILogger _logger;

        public SocketServer()
        {
            IPAddress ipAddress = IPAddress.TryParse(ConfigurationManager.AppSettings["IPAddress"], out IPAddress outIPAddress) ? outIPAddress : IPAddress.Loopback;
            int port = Int32.TryParse(ConfigurationManager.AppSettings["Port"], out int outPort) ? outPort : 8500;
            _endPoint = new IPEndPoint(ipAddress, port);
            _logger = new Logger();
        }
        public void Start()
        {
            int backlog = Int32.TryParse(ConfigurationManager.AppSettings["Backlog"], out int outBacklog) ? outBacklog : 1024;
            _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(_endPoint);
            _socket.Listen(backlog);

            _cancelToken = new CancellationTokenSource();
            Task.Run(() => ProcessRequest(_socket), _cancelToken.Token);
        }

        public void Stop()
        {
            _cancelToken.Token.Register(() => Console.WriteLine("stopping ..."));
            _socket.Close();
        }

        private async Task ProcessRequest(Socket socket)
        {
            do
            {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null).ConfigureAwait(false);

                Console.WriteLine("ECHO SERVER :: CLIENT CONNECTED");

                using (var stream = new NetworkStream(clientSocket, true))
                {
                    do
                    {
                        var buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                        if (bytesRead == 0)
                            break;
                        
                        WriteLog(buffer);
                        Response response = ResponseBuilder.CreateResponse(buffer);
                        byte[] byteResponse = ResponseBuilder.Encode(response);
                        await stream.WriteAsync(byteResponse, 0, byteResponse.Length).ConfigureAwait(false);
                    } while (true);
                }                    
            } while(true);
        }

        private void WriteLog(byte[] log)
        {
            _logger.Info(System.Text.Encoding.UTF8.GetString(log));
        }
    }
}
