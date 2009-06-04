using System;

namespace MyLife.FilesOnline
{
    public class File : Item
    {
        public string DirectLink { get; set; }

        public string ThumbnailLink { get; set; }

        public DateTime Updated { get; set; }

        public int Size { get; set; }
    }
}