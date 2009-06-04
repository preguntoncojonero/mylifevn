using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MyLife.Net.Mail.IMAP
{
    public class ImapClient : IDisposable
    {
        private TcpClient connection;
        private SslStream sslStream;
        private Stream stream;
        private StreamReader streamReader;
        private int tag;

        public ImapClient()
        {
            ConnectTimeout = 30;
        }

        private string Tag
        {
            get { return "kw" + (tag++).ToString().PadLeft(4, '0') + " "; }
        }

        /// <summary>
        /// Maximum number of seconds to wait when connecting to an IMAP server. The default value is 30s.
        /// </summary>
        public int ConnectTimeout { get; set; }

        public ConnectionState State { get; private set; }

        /// <summary>
        /// Return if currently connected to an IMAP server.
        /// </summary>
        public string ConnectedToHost { get; private set; }

        public string LoggedInUser { get; private set; }

        /// <summary>
        /// After selecting a mailbox (by calling SelectMailbox), this property will be updated to reflect the total number of emails in the mailbox.
        /// </summary>
        public int NumMessages { get; private set; }

        /// <summary>
        /// After selecting a mailbox (by calling SelectMailbox), this property will be updated to name of mailbox.
        /// </summary>
        public string SelectedMailbox { get; private set; }

        /// <summary>
        /// True if the IMAP connection should be SSL.
        /// </summary>
        public bool Ssl { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            connection = null;
            streamReader = null;
            stream = null;
            sslStream = null;
        }

        #endregion

        /// <summary>
        /// Connects to an IMAP server, but does not login.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns>Returns true for success, false for failure.</returns>
        public bool Connect(string hostname, int port)
        {
            string read;
            State = ConnectionState.Connecting;
            connection = new TcpClient();
            try
            {
                connection.Connect(hostname, port);
                if (!Ssl)
                {
                    stream = connection.GetStream();
                    streamReader = new StreamReader(stream, Encoding.ASCII);
                }
                else
                {
                    sslStream = new SslStream(connection.GetStream(), false, CertificateValidationCallback);
                    sslStream.AuthenticateAsClient(hostname);
                    streamReader = new StreamReader(sslStream, Encoding.ASCII);
                }

                read = streamReader.ReadLine();
                if (read.StartsWith("* OK "))
                {
                    State = ConnectionState.Connected;
                    ConnectedToHost = hostname;
                    return true;
                }

                State = ConnectionState.Closed;
                throw new ImapConnectionException(read);
            }
            catch (Exception ex)
            {
                State = ConnectionState.Closed;
                throw new ImapConnectionException("Connection failed", ex);
            }
        }

        private static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
                                                          SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        /// <summary>
        /// Logs into the IMAP server. The component must first be connected to an IMAP server by calling Connect.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public void Login(string user, string password)
        {
            if (State != ConnectionState.Connected)
            {
                throw new ImapConnectionException("Connection must be connected before authentication");
            }
            State = ConnectionState.Authenticating;
            Write("CAPABILITY\r\n");
            var read = Read();
            var loginType = GetLoginType(read);
            Read();
            switch (loginType)
            {
                case LoginType.Plain:
                    Write("LOGIN " + user + " " + password + "\r\n");
                    break;
                case LoginType.Login:
                    Write("AUTHENTICATE LOGIN\r\n");
                    Read();
                    UntaggedWrite(string.Format("{0}\r\n", Base64Encode(user)));
                    Read();
                    UntaggedWrite(string.Format("{0}\r\n", Base64Encode(password)));
                    break;
                default:
                    throw new ImapException("Authentication type unsupported: " + loginType);
            }

            var response = Read();
            if (response.StartsWith("* OK") || response.Substring(7, 2) == "OK")
                State = ConnectionState.Open;
            else
            {
                State = ConnectionState.Broken;
                throw new ImapException("Authentication failed: " + response);
            }
        }

        private static string Base64Encode(string data)
        {
            try
            {
                var encData_byte = Encoding.UTF8.GetBytes(data);
                var encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        private void UntaggedWrite(string message)
        {
            var command = Encoding.ASCII.GetBytes(message.ToCharArray());
            try
            {
                if (!Ssl)
                    stream.Write(command, 0, command.Length);
                else
                    sslStream.Write(command, 0, command.Length);
            }
            catch (Exception e)
            {
                throw new Exception("Write error :" + e.Message);
            }
        }

        private static LoginType GetLoginType(string message)
        {
            return LoginType.Login;
        }

        private void Write(string message)
        {
            message = Tag + message;
            var command = Encoding.ASCII.GetBytes(message.ToCharArray());
            try
            {
                if (!Ssl)
                    stream.Write(command, 0, command.Length);
                else
                    sslStream.Write(command, 0, command.Length);
            }
            catch (Exception e)
            {
                throw new Exception("Write error :" + e.Message);
            }
        }

        private string Read()
        {
            return streamReader.ReadLine();
        }

        /// <summary>
        /// Selects a mailbox.
        /// </summary>
        /// <param name="mailbox"></param>
        /// <returns>Returns true for success, false for failure.</returns>
        public bool SelectMailbox(string mailbox)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a NOOP command to the IMAP server and receives the response. The component must be connected and authenticated for this to succeed. Sending a NOOP is a good way of determining whether the connection to the IMAP server is up and active.
        /// </summary>
        /// <returns>Returns true for success, false for failure.</returns>
        public bool Noop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects cleanly from the IMAP server.
        /// </summary>
        /// <returns>Returns true for success, false for failure.</returns>
        public bool Disconnect()
        {
            ConnectedToHost = string.Empty;
            LoggedInUser = string.Empty;
            SelectedMailbox = string.Empty;
            NumMessages = 0;

            try
            {
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new ImapConnectionException("Error closing connection", ex);
            }
        }
    }
}