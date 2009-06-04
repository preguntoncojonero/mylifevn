using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Profile;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Validation;
using xVal.ServerSide;

namespace MyLife.Web.Profile
{
    public class MyLifeProfile : ProfileBase
    {
        [Required(ErrorMessage = "Bạn chưa nhập họ và tên"), StringLength(255)]
        public string FullName
        {
            get
            {
                var fullname = GetPropertyValue("FullName") as string;
                if (string.IsNullOrEmpty(fullname))
                {
                    fullname = UserName;
                }
                return fullname;
            }
            set { SetPropertyValue("FullName", value); }
        }

        [Required(ErrorMessage = "Bạn chưa nhập ngày tháng năm sinh")]
        [RegularExpression(Constants.Regulars.Date, ErrorMessage = "Giá trị bạn nhập không hợp lệ")]
        public DateTime Birthday
        {
            get { return (DateTime) GetPropertyValue("Birthday"); }
            set { SetPropertyValue("Birthday", value); }
        }

        public string Address
        {
            get { return (string) GetPropertyValue("Address"); }
            set { SetPropertyValue("Address", value); }
        }

        public string City
        {
            get { return (string) GetPropertyValue("City"); }
            set { SetPropertyValue("City", value); }
        }

        public bool Sex
        {
            get { return (bool) GetPropertyValue("Sex"); }
            set { SetPropertyValue("Sex", value); }
        }

        public override void Save()
        {
            Validate();
            base.Save();
            CacheFactory.GetCacheManager(MyLifeContext.Settings.CacheProvider).Flush();
        }

        public void Validate()
        {
            var errors = DataAnnotationsValidationRunner.GetErrors(this);
            if (errors.Any())
            {
                if (errors.Count() != 1 || errors.ElementAt(0).PropertyName != "Birthday")
                {
                    throw new RulesException(errors);
                }
            }
        }

        public static MyLifeProfile GetProfile(string username)
        {
            var key = "Membership_GetProfile_" + username;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.CacheProvider);
            if (cache.Contains(key))
            {
                return (MyLifeProfile) cache[key];
            }
            var profile = Create(username) as MyLifeProfile;
            if (profile != null)
            {
                cache.Add(key, profile);
            }
            return profile;
        }
    }
}