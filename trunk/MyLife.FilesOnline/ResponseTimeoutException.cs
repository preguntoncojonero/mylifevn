using System;

namespace MyLife.FilesOnline
{
    public class ResponseTimeoutException : FilesOnlineException
    {
        public ResponseTimeoutException(Exception ex)
            : base(ex.Message, ex)
        {
        }
    }
}