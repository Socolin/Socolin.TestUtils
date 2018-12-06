using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Socolin.TestUtils.FakeSmtp
{
    public interface IMailReceiver
    {
        List<FakeSmtpMail> Mails { get; }
    }

    public class FakeSmtpServer : IDisposable, IMailReceiver
    {
        private Socket _socket;
        public List<FakeSmtpMail> Mails { get; } = new List<FakeSmtpMail>();
        private string Username { get; set; }
        private string Password { get; set; }
        private bool Running { get; set; }

        public FakeSmtpServer()
        {
            Username = Guid.NewGuid().ToString();
            Password = Guid.NewGuid().ToString();
        }

        public FakeSmtpConfig Start()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
            _socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            _socket.Listen(5);
            var listenPort = ((IPEndPoint) _socket.LocalEndPoint).Port;

            Running = true;
            var t = new Thread(Run);
            t.Start();

            return new FakeSmtpConfig
            {
                Port = listenPort,
                Host = IPAddress.Loopback,
                Username = Username,
                Password = Password
            };
        }

        public void Stop()
        {
            _socket.Close();
            Running = false;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        private void Run(object obj)
        {
            while (Running)
            {
                using (var clientSocket = _socket.Accept())
                {
                    clientSocket.ReceiveTimeout = 200;
                    try
                    {
                        var session = new FakeSmtpSession(clientSocket);
                        session.ReceiveMail();
                        var mail = session.GetMail();
                        Mails.Add(mail);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }
    }
}