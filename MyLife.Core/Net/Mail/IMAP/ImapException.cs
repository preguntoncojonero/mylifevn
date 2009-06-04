using System;

namespace MyLife.Net.Mail.IMAP
{
    public class ImapException : Exception
    {
        public ImapException()
        {
        }

        public ImapException(string message)
            : base(message)
        {
        }

        public ImapException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}