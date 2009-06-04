using MyLife.Net.Mail.IMAP;
using NUnit.Framework;

namespace ImapUnitTest
{
    [TestFixture]
    public class IMapClientUnitTest
    {
        private readonly ImapClient client;

        public IMapClientUnitTest()
        {
            client = new ImapClient {Ssl = true};
        }

        [Test]
        public void Login()
        {
            if (client.State != ConnectionState.Connected)
            {
                Connect();
            }

            client.Login("nguyen.dainghia@gmail.com", "Gioxoay@1234");
        }

        [Test]
        public void Connect()
        {
            var retval = client.Connect("imap.gmail.com", 993);
            Assert.AreEqual(true, retval);
        }
    }
}