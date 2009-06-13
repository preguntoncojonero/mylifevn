using System.Configuration;

namespace MyLife.Configuration
{
    public class FriendsElement : ConfigurationElementBase
    {
        [ConfigurationProperty("theme", IsRequired = true)]
        public string Theme
        {
            get { return (string) base["theme"]; }
            set { base["theme"] = value; }
        }

        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get { return (string) base["title"]; }
            set { base["title"] = value; }
        }
    }
}