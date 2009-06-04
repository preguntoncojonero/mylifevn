using System.Collections.Generic;

namespace MyLife.FilesOnline
{
    public class Folder : Item
    {
        private List<File> files;
        private List<Folder> folders;

        public List<File> Files
        {
            get
            {
                if (files == null)
                {
                    files = new List<File>();
                }
                return files;
            }
        }

        public List<Folder> Folders
        {
            get
            {
                if (folders == null)
                {
                    folders = new List<Folder>();
                }
                return folders;
            }
        }
    }
}