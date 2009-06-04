using System.Configuration;

namespace MyLife.Configuration
{
    public class LinksElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty defaultProvider;
        private static readonly ConfigurationProperty providers;
        private static readonly ConfigurationProperty cacheProvider;

        static LinksElement()
        {
            providers = new ConfigurationProperty("providers", typeof (ProviderSettingsCollection), null,
                                                  ConfigurationPropertyOptions.None);
            defaultProvider = new ConfigurationProperty("defaultProvider", typeof (string), null,
                                                        ConfigurationPropertyOptions.IsRequired);
            cacheProvider = new ConfigurationProperty("cacheProvider", typeof(string), null,
                                                      ConfigurationPropertyOptions.IsRequired);
        }

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection) base[providers]; }
        }

        [ConfigurationProperty("defaultProvider", IsRequired = true)]
        public string DefaultProvider
        {
            get { return (string) base[defaultProvider]; }
            set { base[defaultProvider] = value; }
        }

        [ConfigurationProperty("cacheProvider")]
        public string CacheProvider
        {
            get { return (string)base[cacheProvider]; }
            set { base[cacheProvider] = value; }
        }
    }
}