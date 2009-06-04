using System;

namespace MyLife.FilesOnline
{
    public class FileUploadEventArgs : EventArgs
    {
        public long BytesSent;
        public long TotalBytes;

        public FileUploadEventArgs(long bytesSent, long totalBytes)
        {
            BytesSent = bytesSent;
            TotalBytes = totalBytes;
        }
    }
}