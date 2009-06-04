using System;

namespace MyLife.FilesOnline
{
    public class FileUploadStartEventArgs : EventArgs
    {
        private readonly string fileName;
        private readonly long size;

        public FileUploadStartEventArgs(string fileName, long size)
        {
            this.fileName = fileName;
            this.size = size;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public long Size
        {
            get { return size; }
        }
    }
}