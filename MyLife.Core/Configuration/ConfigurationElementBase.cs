using System.Configuration;

namespace MyLife.Configuration
{
    public abstract class ConfigurationElementBase : ConfigurationElement
    {
        private static readonly ConfigurationProperty providers;

        static ConfigurationElementBase()
        {
            providers = new ConfigurationProperty("providers", typeof (ProviderSettingsCollection), null,
                                                  ConfigurationPropertyOptions.IsDefaultCollection);
        }

        [ConfigurationProperty("providers", IsDefaultCollection = true)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection) base[providers]; }
        }

        [ConfigurationProperty("cacheProvider", IsRequired = true)]
        public string CacheProvider
        {
            get { return (string) base["cacheProvider"]; }
            set { base["cacheProvider"] = value; }
        }

        [ConfigurationProperty("defaultProvider", IsRequired = true)]
        public string DefaultProvider
        {
            get { return (string) base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }
}