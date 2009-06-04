using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Web.Profile;
using MyLife.Serialization;

namespace MyLife.DataAccess.Security
{
    public class SqlServerProfileProvider : ProfileProvider
    {
        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                   DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate,
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                                     string usernameToMatch, int pageIndex, int pageSize,
                                                                     out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                                     DateTime userInactiveSinceDate, int pageIndex,
                                                                     int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption,
                                                             int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                        DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
                                                                          SettingsPropertyCollection collection)
        {
            var properties = new SettingsPropertyValueCollection();
            if (collection.Count < 1)
                return properties;

            var username = (string) context["UserName"];
            foreach (SettingsProperty property in collection)
            {
                properties.Add(new SettingsPropertyValue(property));
            }

            var db = new MyLifeEntities();
            var profile = db.tblProfiles.Where(item => item.UserName == username).FirstOrDefault();
            if (profile != null)
            {
                var names =
                    profile.PropertyNames.Split(new[] {";#"}, StringSplitOptions.RemoveEmptyEntries);
                var values =
                    profile.PropertyValues.Split(new[] {";#"}, StringSplitOptions.RemoveEmptyEntries);
                if (names.Length > 0 && values.Length > 0)
                {
                    for (var i = 0; i < names.Length; i++)
                    {
                        var property = properties[names[i]];
                        if (property == null) continue;
                        property.PropertyValue = Base64Serializer.Deserialize(values[i]);
                    }
                }
            }

            return properties;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            var username = (string) context["UserName"];
            var userIsAuthenticated = (bool) context["IsAuthenticated"];
            if (string.IsNullOrEmpty(username) || collection.Count < 1)
                return;

            var db = new MyLifeEntities();
            var profile = db.tblProfiles.Where(item => item.UserName == username).FirstOrDefault();
            if (profile == null)
            {
                profile = new tblProfiles {LastUpdatedDate = DateTime.UtcNow};
                db.AddTotblProfiles(profile);
                profile.UserName = username.ToLowerInvariant();
            }
            
            var names = new StringBuilder();
            var values = new StringBuilder();
            foreach (SettingsPropertyValue property in collection)
            {
                var name = property.Name;
                var value = property.PropertyValue;
                var allowAnonymous = (bool) property.Property.Attributes["AllowAnonymous"];
                if (!userIsAuthenticated && !allowAnonymous) continue;
                if (value == null) continue;
                names.Append(name).Append(";#");
                values.Append(Base64Serializer.Serialize(value)).Append(";#");
            }
            profile.PropertyNames = names.ToString();
            profile.PropertyValues = values.ToString();
            profile.LastUpdatedDate = DateTime.UtcNow;
            db.SaveChanges();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (name.Length < 1)
                name = "SqlServerProfileProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "SqlServerProfileProvider profile provider");
            }

            base.Initialize(name, config);

            if (config == null)
                throw new ArgumentNullException("config");

            config.Remove("description");

            if (config.Count > 0)
            {
                var attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException("Unrecognized attribute: " + attribUnrecognized);
            }
        }
    }
}