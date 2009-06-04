using System.Configuration;

namespace MyLife.Configuration
{
    public class MimeTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("extension", IsRequired = true, IsKey = true)]
        public string Extension
        {
            get { return (string) this["extension"]; }
            set { this["extension"] = value; }
        }

        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType
        {
            get { return (string) this["contentType"]; }
            set { this["contentType"] = value; }
        }
    }

    [ConfigurationCollection(typeof (MimeTypeElement))]
    public class MimeTypesElement : ConfigurationElementCollection
    {
        public new MimeTypeElement this[string extension]
        {
            get { return BaseGet(extension) as MimeTypeElement; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MimeTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MimeTypeElement) element).Extension;
        }
    }
}