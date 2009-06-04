using System;

namespace MyLife.FilesOnline
{
    public abstract class Item
    {
        public DateTime Created { get; set; }

        public Folder Parent { get; set; }

        public object Id { get; set; }

        public string Name { get; set; }

        public bool Shared { get; set; }

        public string SharedLink { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}