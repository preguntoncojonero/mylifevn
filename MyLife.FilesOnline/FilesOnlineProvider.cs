using System;
using System.Net;

namespace MyLife.FilesOnline
{
    public abstract class FilesOnlineProvider
    {
        public IWebProxy Proxy { get; set; }

        public bool Busy { get; protected set; }

        public abstract bool IsAuthenticated { get; }

        public virtual User User { get; protected set; }

        public static event FileUploadHandler FileUploading;

        public static event EventHandler FileUploadComplete;

        public static event FileUploadStartHandler FileUploadStart;

        protected virtual void OnFileUploading(FileUploadEventArgs e)
        {
            if (FileUploading != null)
            {
                FileUploading(this, e);
            }
        }

        protected virtual void OnFileUploadComplete(EventArgs e)
        {
            if (FileUploadComplete != null)
            {
                FileUploadComplete(this, e);
            }
        }

        protected virtual void OnFileUploadStart(FileUploadStartEventArgs e)
        {
            if (FileUploadStart != null)
            {
                FileUploadStart(this, e);
            }
        }

        public abstract void CreateFolder(Folder parent, string folder);

        public abstract void Delete(Folder parent, Item item);

        public abstract void Login(string user, string password);

        public abstract void Logout();

        public abstract void Share(Item item);

        public abstract void UnShare(Item item);

        public abstract void AsyncUpload(Folder parent, string file, byte[] data);

        public abstract void AsyncUpload(Folder parent, string filePath);

        public abstract void StopAsyncUpload();

        public abstract Folder GetFolders();
    }
}