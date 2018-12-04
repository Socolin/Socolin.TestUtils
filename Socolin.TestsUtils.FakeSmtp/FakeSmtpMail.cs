using System.Collections.Generic;

namespace Socolin.TestsUtils.FakeSmtp
{
    public class FakeSmtpMail
    {
        public IDictionary<string, List<string>> Headers { get; } = new Dictionary<string, List<string>>();
        public string Data { get; set; }

        public IEnumerable<string> To => Headers.ContainsKey("To") ? Headers["To"] : new List<string>();
        public IEnumerable<string> Subject => Headers.ContainsKey("Subject") ? Headers["Subject"] : new List<string>();
    }
}