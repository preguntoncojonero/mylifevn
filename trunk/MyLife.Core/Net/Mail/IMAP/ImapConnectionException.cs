using System;

namespace MyLife.Net.Mail.IMAP
{
    public class ImapConnectionException : ImapException
    {
        public ImapConnectionException()
        {
        }

        public ImapConnectionException(string message)
            : base(message)
        {
        }

        public ImapConnectionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}