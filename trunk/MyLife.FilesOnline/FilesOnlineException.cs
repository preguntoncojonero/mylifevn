using System;

namespace MyLife.FilesOnline
{
    public class FilesOnlineException : Exception
    {
        public FilesOnlineException()
        {
        }

        public FilesOnlineException(string message) : base(message)
        {
        }

        public FilesOnlineException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}