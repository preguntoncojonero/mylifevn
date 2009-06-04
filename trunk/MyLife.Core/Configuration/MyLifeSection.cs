using System.Configuration;

namespace MyLife.Configuration
{
    public sealed class MyLifeSection : ConfigurationSection
    {
        [ConfigurationProperty("cacheProvider", IsRequired = true)]
        public string CacheProvider
        {
            get { return (string) base["cacheProvider"]; }
            set { base["cacheProvider"] = value; }
        }

        [ConfigurationProperty("theme", IsRequired = true)]
        public string Theme
        {
            get { return (string) base["theme"]; }
            set { base["theme"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("slogan", IsRequired = true)]
        public string Slogan
        {
            get { return (string) base["slogan"]; }
            set { base["slogan"] = value; }
        }

        [ConfigurationProperty("keywords", IsRequired = true)]
        public string Keywords
        {
            get { return (string) base["keywords"]; }
            set { base["keywords"] = value; }
        }

        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get { return (string) base["description"]; }
            set { base["description"] = value; }
        }

        [ConfigurationProperty("shortDateFormat", IsRequired = true)]
        public string ShortDateFormat
        {
            get { return (string) base["shortDateFormat"]; }
            set { base["shortDateFormat"] = value; }
        }

        [ConfigurationProperty("shortDateTimeFormat", IsRequired = true)]
        public string ShortDateTimeFormat
        {
            get { return (string) base["shortDateTimeFormat"]; }
            set { base["shortDateTimeFormat"] = value; }
        }

        [ConfigurationProperty("longDateFormat", IsRequired = true)]
        public string LongDateFormat
        {
            get { return (string) base["longDateFormat"]; }
            set { base["longDateFormat"] = value; }
        }

        [ConfigurationProperty("longDateTimeFormat", IsRequired = true)]
        public string LongDateTimeFormat
        {
            get { return (string) base["longDateTimeFormat"]; }
            set { base["longDateTimeFormat"] = value; }
        }

        [ConfigurationProperty("blogs", IsRequired = true)]
        public BlogsElement Blogs
        {
            get { return (BlogsElement) base["blogs"]; }
        }

        [ConfigurationProperty("storage", IsRequired = true)]
        public StorageElement Storage
        {
            get { return (StorageElement) base["storage"]; }
        }

        [ConfigurationProperty("links", IsRequired = true)]
        public LinksElement Links
        {
            get { return (LinksElement) base["links"]; }
        }

        [ConfigurationProperty("mimeTypes")]
        public MimeTypesElement MimeTypes
        {
            get { return (MimeTypesElement) base["mimeTypes"]; }
        }

        [ConfigurationProperty("news", IsRequired = true)]
        public NewsElement News
        {
            get { return (NewsElement) base["news"]; }
        }

        [ConfigurationProperty("friends", IsRequired = true)]
        public FriendsElement Friends
        {
            get { return (FriendsElement) base["friends"]; }
        }

        [ConfigurationProperty("moneyBox", IsRequired = true)]
        public MoneyBoxElement MoneyBox
        {
            get { return (MoneyBoxElement) base["moneyBox"]; }
        }
    }
}