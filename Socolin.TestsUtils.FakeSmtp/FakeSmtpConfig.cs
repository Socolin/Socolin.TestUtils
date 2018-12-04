using System.Net;

namespace Socolin.TestsUtils.FakeSmtp
{
    public class FakeSmtpConfig
    {
        public IPAddress Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}