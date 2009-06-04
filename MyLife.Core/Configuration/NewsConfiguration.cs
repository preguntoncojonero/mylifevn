using System.Configuration;

namespace MyLife.Configuration
{
    public class NewsElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty cacheProvider;
        private static readonly ConfigurationProperty defaultProvider;
        private static readonly ConfigurationProperty providers;

        static NewsElement()
        {
            providers = new ConfigurationProperty("providers", typeof (ProviderSettingsCollection), null,
                                                  ConfigurationPropertyOptions.IsDefaultCollection);
            defaultProvider = new ConfigurationProperty("defaultProvider", typeof (string), null,
                                                        ConfigurationPropertyOptions.None);
            cacheProvider = new ConfigurationProperty("cacheProvider", typeof (string), null,
                                                      ConfigurationPropertyOptions.IsRequired);
        }

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection) base[providers]; }
        }

        [ConfigurationProperty("cacheProvider")]
        public string CacheProvider
        {
            get { return (string) base[cacheProvider]; }
            set { base[cacheProvider] = value; }
        }

        [ConfigurationProperty("defaultProvider", IsRequired = true)]
        public string DefaultProvider
        {
            get { return (string) base[defaultProvider]; }
            set { base[defaultProvider] = value; }
        }
    }
}