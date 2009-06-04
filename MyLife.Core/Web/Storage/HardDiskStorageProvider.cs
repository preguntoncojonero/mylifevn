using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;

namespace MyLife.Web.Storage
{
    [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
    public class HardDiskStorageProvider : StorageProvider
    {
        private string location;

        #region StorageProvider Members

        public override void Save(string key, byte[] data)
        {
            var path = Path.Combine(location, key);
            var stream = new FileStream(path, FileMode.Create);
            var writer = new BinaryWriter(stream);
            writer.Write(data);
            writer.Close();
            stream.Close();
        }

        public override void Update(string key, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string key)
        {
            var path = Path.Combine(location, key);
            File.Delete(path);
        }

        public override byte[] Load(string key)
        {
            try
            {
                var path = Path.Combine(location, key);
                var stream = new FileStream(path, FileMode.Open);
                var reader = new BinaryReader(stream);
                var data = reader.ReadBytes((int) stream.Length);
                reader.Close();
                stream.Close();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            location = config["location"];
            if (string.IsNullOrEmpty(location))
            {
                location = MyLifeContext.WorkingFolder + "Uploads";
            }
        }

        #endregion
    }
}