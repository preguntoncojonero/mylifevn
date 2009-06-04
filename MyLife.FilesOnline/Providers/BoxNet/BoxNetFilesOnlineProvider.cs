using System;
using System.Threading;
using System.Xml;

namespace MyLife.FilesOnline.Providers.BoxNet
{
    public class BoxNetFilesOnlineProvider : FilesOnlineProvider
    {
        private FileUploader fileUploader;
        private string fileUploading;
        private Thread threadUpload;

        public new BoxNetUser User
        {
            get { return (BoxNetUser) base.User; }
            protected set { base.User = value; }
        }

        public override bool IsAuthenticated
        {
            get { return User != null && User.IsAuthenticated; }
        }

        public override void CreateFolder(Folder parent, string folder)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Folder parent, Item item)
        {
            throw new NotImplementedException();
        }

        public override void Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new FilesOnlineException("You must supply both a Login/E-mail and Password.");
            }

            var str = string.Format(Constants.LoginRequest, email, password);
            var request = new HttpRequest(Constants.BoxAPILoginUrl, Proxy)
                              {
                                  ContentType = "text/xml",
                                  PostData = Utils.StrToByteArray(str)
                              };
            var document = new XmlDocument();
            document.Load(request.GetResponse());

            if (Utils.GetNodeTextString(document.SelectSingleNode("//status")).ToLower() != "logged")
            {
                throw new FilesOnlineAuthenticationException(
                    string.Format("Login failed. Response from Box.net was '{0}'",
                                  Utils.GetNodeTextString(document.SelectSingleNode("//status")).ToLower()));
            }

            User = new BoxNetUser
                       {
                           Sid = Utils.GetNodeTextString(document.SelectSingleNode("//user/sid")),
                           AccessId = Utils.GetNodeTextInt(document.SelectSingleNode("//user/access_id")),
                           Email = Utils.GetNodeTextString(document.SelectSingleNode("//user/email")),
                           Free = Utils.GetNodeTextBool(document.SelectSingleNode("//user/free")),
                           Login = Utils.GetNodeTextString(document.SelectSingleNode("//user/login")),
                           SpaceAmount = Utils.GetNodeTextLong(document.SelectSingleNode("//user/space_amount")),
                           SpaceUsed = Utils.GetNodeTextLong(document.SelectSingleNode("//user/space_used")),
                           UserId = Utils.GetNodeTextInt(document.SelectSingleNode("//user/user_id"))
                       };
        }

        public override void Logout()
        {
            User = null;
        }

        public override void Share(Item item)
        {
            throw new NotImplementedException();
        }

        public override void UnShare(Item item)
        {
            throw new NotImplementedException();
        }

        public override void AsyncUpload(Folder parent, string file, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void AsyncUpload(Folder parent, string filePath)
        {
            if (!IsAuthenticated)
            {
                throw new FilesOnlineAuthenticationException("You must login before you can upload files.");
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new FilesOnlineException("Specified upload file does not exist on this machine.");
            }

            if (parent == null)
            {
                throw new FilesOnlineException("Not all parameters were supplied or were invalid.");
            }

            if ((threadUpload != null) && threadUpload.IsAlive)
            {
                throw new FilesOnlineException(
                    "Only one file can be upload at a time. Please wait for the current file to finish upload.");
            }

            fileUploader = new FileUploader(string.Format(Constants.BoxAPIUploadUrl, User.Sid), this) {Proxy = Proxy};
            fileUploader.SetField("location", parent.Id.ToString());
            fileUploading = filePath;
            threadUpload = new Thread(Upload);
            threadUpload.Start();
            Busy = true;
        }

        private void Upload()
        {
            try
            {
                fileUploader.SendFile(fileUploading);
            }
            finally
            {
                threadUpload.Abort();
                Busy = false;
            }
        }

        public override void StopAsyncUpload()
        {
            if (threadUpload != null)
            {
                threadUpload.Abort();
                Busy = false;
            }
        }

        public void RaiseFileUploadComplete()
        {
            OnFileUploadComplete(EventArgs.Empty);
        }

        public void RaiseFileUploading(long bytesSent, long totalBytes)
        {
            OnFileUploading(new FileUploadEventArgs(bytesSent, totalBytes));
        }

        public override Folder GetFolders()
        {
            if (!IsAuthenticated)
            {
                throw new FilesOnlineAuthenticationException("You must login before you can view the file list.");
            }

            var str = string.Format(Constants.FileTreeRequest, 0, 0);
            var request = new HttpRequest(string.Format(Constants.BoxAPIGeneralUrl, User.Sid), Proxy);
            request.ContentType = "text/xml";
            request.PostData = Utils.StrToByteArray(str);
            var document = new XmlDocument();
            document.Load(request.GetResponse());
            if (Utils.GetNodeTextString(document.SelectSingleNode("//status")).ToLower() != "listing_ok")
            {
                throw new FilesOnlineException(
                    string.Format("Listing files failed. Response from Box.net was '{0}'",
                                  Utils.GetNodeTextString(document.SelectSingleNode("//status")).ToLower()));
            }
            var folderNode = document.SelectSingleNode("//folder");
            return LoadFolderObject(folderNode, null);
        }

        private Folder LoadFolderObject(XmlNode folderNode, Folder parent)
        {
            var folder = new Folder();
            folder.Id = Utils.GetAttributeValueInt(folderNode, "id");
            folder.Parent = parent;
            folder.Name = Utils.GetAttributeValueString(folderNode, "name");
            folder.Shared = Utils.GetAttributeValueBool(folderNode, "shared");
            folder.SharedLink = Utils.GetAttributeValueString(folderNode, "shared_link");
            foreach (XmlNode node in folderNode.SelectNodes("files/file"))
            {
                folder.Files.Add(LoadFileObject(node, folder));
            }
            foreach (XmlNode node2 in folderNode.SelectNodes("folders/folder"))
            {
                folder.Folders.Add(LoadFolderObject(node2, folder));
            }
            return folder;
        }

        private File LoadFileObject(XmlNode fileNode, Folder parent)
        {
            var file = new File();
            file.Id = Utils.GetAttributeValueInt(fileNode, "id");
            file.Parent = parent;
            file.Size = Utils.GetAttributeValueInt(fileNode, "size");
            file.Name = Utils.GetAttributeValueString(fileNode, "file_name");
            file.SharedLink = Utils.GetAttributeValueString(fileNode, "shared_link");
            file.ThumbnailLink = Utils.GetAttributeValueString(fileNode, "thumbnail");
            file.Created =
                new DateTime(0x7b2, 1, 1).AddSeconds(Utils.GetAttributeValueLong(fileNode, "created"));
            file.Updated =
                new DateTime(0x7b2, 1, 1).AddSeconds(Utils.GetAttributeValueLong(fileNode, "updated"));
            file.Shared = Utils.GetAttributeValueBool(fileNode, "shared");
            if (!User.Free && file.Shared)
            {
                file.DirectLink =
                    string.Format(Constants.DirectLinkFormat, Utils.GetFileSharedId(file.SharedLink),
                                  Utils.GetFileExtension(file.Name));
            }
            return file;
        }
    }
}