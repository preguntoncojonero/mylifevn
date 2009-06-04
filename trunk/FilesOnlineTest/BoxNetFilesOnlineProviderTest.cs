using MyLife.FilesOnline;
using MyLife.FilesOnline.Providers.BoxNet;
using NUnit.Framework;

namespace FilesOnlineTest
{
    [TestFixture]
    public class BoxNetFilesOnlineProviderTest
    {
        #region Setup/Teardown

        [SetUp]
        public void Login()
        {
            provider = new BoxNetFilesOnlineProvider();
            //provider.Proxy = new WebProxy("127.0.0.1", 9666);
            provider.Login("nguyen.dainghia@gmail.com", "Gioxoay@123");
        }

        #endregion

        private FilesOnlineProvider provider;

        [Test]
        public void GetFolders()
        {
            var folder = provider.GetFolders();
        }

        [Test]
        public void Upload()
        {
            provider.AsyncUpload(new Folder {Id = 0}, "F:\\Pictures\\Nghiand.jpg");
        }
    }
}