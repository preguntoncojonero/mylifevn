using System.Configuration;

namespace MyLife.Configuration
{
    public class BlogsElement : ConfigurationElementBase
    {
        [ConfigurationProperty("defaultTheme", IsRequired = true)]
        public string DefaultTheme
        {
            get { return (string) base["defaultTheme"]; }
            set { base["defaultTheme"] = value; }
        }
    }
}