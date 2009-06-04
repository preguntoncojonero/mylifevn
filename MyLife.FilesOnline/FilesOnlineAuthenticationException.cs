using System;

namespace MyLife.FilesOnline
{
    public class FilesOnlineAuthenticationException : FilesOnlineException
    {
        public FilesOnlineAuthenticationException()
        {
        }

        public FilesOnlineAuthenticationException(string message) : base(message)
        {
        }

        public FilesOnlineAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}