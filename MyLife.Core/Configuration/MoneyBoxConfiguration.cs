using System.Configuration;

namespace MyLife.Configuration
{
    public class MoneyBoxElement : ConfigurationElementBase
    {
        [ConfigurationProperty("numberOfRecordsOnPage", IsRequired = true)]
        public int NumberOfRecordsOnPage
        {
            get { return (int) base["numberOfRecordsOnPage"]; }
            set { base["numberOfRecordsOnPage"] = value; }
        }

        [ConfigurationProperty("symmetricCryptoProvider", IsRequired = true)]
        public string SymmetricCryptoProvider
        {
            get { return (string) base["symmetricCryptoProvider"]; }
            set { base["symmetricCryptoProvider"] = value; }
        }
    }
}