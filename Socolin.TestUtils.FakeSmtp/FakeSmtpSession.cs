using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;

namespace Socolin.TestUtils.FakeSmtp;

[PublicAPI]
public class FakeSmtpSession
{
    private enum SessionState
    {
        Init,
        Login,
        Headers,
        Data,
        End
    }

    private readonly byte[] _rawBuffer = new byte[16 * 1024];
    private readonly Socket _clientSocket;
    private string _buffer = string.Empty;
    private SessionState _state;
    private string _mailData = string.Empty;
    private bool ExtendedSmtp { get; set; } = false;
    private string Login { get; set; }
    private string Password { get; set; }

    public FakeSmtpSession(Socket clientSocket)
    {
        _clientSocket = clientSocket;
    }

    private int LoadMoreData()
    {
        int count;
        try
        {
            count = _clientSocket.Receive(_rawBuffer);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            return 0;
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Disconnecting)
        {
            return 0;
        }

        if (count == 0)
        {
            _clientSocket.Close();
        }

        var data = Encoding.UTF8.GetString(_rawBuffer, 0, count);
        _buffer += data;

        return count;
    }

    private string GetNextLine()
    {
        while (true)
        {
            var idx = _buffer.IndexOf("\r\n", StringComparison.InvariantCulture);
            if (idx > -1)
            {
                var line = _buffer.Substring(0, idx);
                _buffer = _buffer.Remove(0, idx + 2);
                return line;
            }

            var count = LoadMoreData();
            if (count == 0)
                return null;
        }
    }

    private void SendLine(string line)
    {
        _clientSocket.Send(Encoding.UTF8.GetBytes(line + "\r\n"));
    }

    private void SendLine(int code, string line)
    {
        _clientSocket.Send(Encoding.UTF8.GetBytes(code + " " + line + "\r\n"));
    }


    public void ReceiveMail()
    {
        // This assume, all will work as planned, very few error handling since it intend to run in localhost
        // without strange utf8 characters which could lead in a failure during decode etc...
        // This could be improved if needed

        SendLine(220, "smtp.localhost FakeSmtpServer");
        while (true)
        {
            var line = GetNextLine();
            if (line == null)
                break;
            if (!HandleLine(line))
                break;
        }
    }

    public FakeSmtpMail GetMail()
    {
        if (_state != SessionState.End)
            return null;

        var mailParts = _mailData.Split(new[] {"\r\n\r\n"}, 2, StringSplitOptions.None);
        var headersPart = mailParts[0];

        var mail = new FakeSmtpMail();

        foreach (var headerData in headersPart.Split(new[] {"\r\n"}, StringSplitOptions.None))
        {
            var header = headerData.Split(new[] {':'}, 2);
            var headerName = header[0].Trim();
            var headerValue = header[1].Trim();

            if (!mail.Headers.ContainsKey(headerName))
            {
                mail.Headers[headerName] = new List<string>();
            }

            mail.Headers[headerName].Add(headerValue);
        }

        mail.Data = DecodeBody(mailParts[1]);

        return mail;
    }

    private static string DecodeBody(string input)
    {
        var i = 0;
        var output = new List<byte>();
        while (i < input.Length)
        {
            if (input[i] == '=' && input[i + 1] == '\r' && input[i + 2] == '\n')
            {
                i += 3;
            }
            else if (input[i] == '=')
            {
                var sHex = input;
                sHex = sHex.Substring(i + 1, 2);
                var hex = Convert.ToInt32(sHex, 16);
                var b = Convert.ToByte(hex);
                output.Add(b);
                i += 3;
            }
            else
            {
                output.Add((byte)input[i]);
                i++;
            }
        }


        return Encoding.UTF8.GetString(output.ToArray());
    }

    private bool HandleLine(string line)
    {
        Console.WriteLine(line);
        switch (_state)
        {
            case SessionState.Init:
                if (line.StartsWith("HELO"))
                {
                    SendLine(250, "some-text");
                    _state = SessionState.Headers;
                    return true;
                }

                if (line.StartsWith("EHLO"))
                {
                    var clientName = line.Split(new[] {' '}, 2);
                    ExtendedSmtp = true;
                    SendLine($"250-smtp.localhost Hello {clientName}");
                    SendLine("250-SIZE 1000000");
                    SendLine("250 AUTH LOGIN PLAIN CRAM-MD5");
                    _state = SessionState.Login;
                    return true;
                }

                return false;
            case SessionState.Login:
                if (line.StartsWith("AUTH"))
                {
                    var authInfo = line.Split(new[] {" "}, StringSplitOptions.None);
                    if (authInfo.Length == 3)
                    {
                        if (authInfo[1] == "login")
                        {
                            Login = Encoding.UTF8.GetString(Convert.FromBase64String(authInfo[2]));
                        }
                    }

                    // FIXME: Check password
                    SendLine(235, "Ok");
                    _state = SessionState.Headers;
                    return true;
                }

                return false;
            case SessionState.Headers:
                if (line == "DATA")
                {
                    _state = SessionState.Data;
                    SendLine(354, "End data with <CR><LF>.<CR><LF>");
                }
                else
                    SendLine(250, "Ok");

                return true;
            case SessionState.Data:
                _mailData += line + "\r\n";
                if (_mailData.EndsWith("\r\n.\r\n"))
                {
                    _state = SessionState.End;
                    SendLine(250, "Ok");
                }

                return true;
            case SessionState.End:
                if (line == "QUIT")
                {
                    SendLine(221, "Bye");
                    _clientSocket.Close();
                    return false;
                }
                else
                {
                    SendLine(250, "Ok");
                }

                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}