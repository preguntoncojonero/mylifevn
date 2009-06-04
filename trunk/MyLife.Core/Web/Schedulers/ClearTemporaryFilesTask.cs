using System;
using System.IO;
using System.Linq;

namespace MyLife.Web.Schedulers
{
    public class ClearTemporaryFilesTask : Task
    {
        public ClearTemporaryFilesTask()
            : this(86400000) // Daily clear
        {
        }

        public ClearTemporaryFilesTask(double interval) : base(interval)
        {
        }

        protected override void ExecuteTask()
        {
            var path = Path.Combine(MyLifeContext.WorkingFolder, "Uploads");
            var dt = DateTime.Now;

            // Avatars
            var avatars =
                Directory.GetFiles(Path.Combine(path, "Avatars")).Where(
                    file => (dt - File.GetCreationTime(file)).Days > 1).ToList();
            foreach (var avatar in avatars)
            {
                File.Delete(avatar);
            }
        }
    }
}