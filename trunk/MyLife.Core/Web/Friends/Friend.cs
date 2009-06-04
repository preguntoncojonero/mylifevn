using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Web.Script.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Web.Security;
using xVal.ServerSide;

namespace MyLife.Web.Friends
{
    public class Friend : BizObject<Friend, int>
    {
        private static readonly ICacheManager CacheManager;

        static Friend()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.Friends.CacheProvider);
        }

        public Friend()
        {
        }

        public Friend(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn chưa nhập tên của người bạn")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Bạn chưa xác định chữ cái đại diện")]
        [RegularExpression("[a-zA-Z]", ErrorMessage = "Chữ cái đại diện phải là từ A-Z")]
        public string Letter { get; set; }

        public string NickName { get; set; }

        [RegularExpression(Constants.Regulars.Date, ErrorMessage = "Giá trị không đúng định dạng ngày/tháng/năm")]
        public DateTime Birthday { get; set; }

        public int DaysOfBirthday
        {
            get
            {
                var now = DateTime.Now.ToUniversalTime().AddHours(7).Date;
                var birthday = new DateTime(now.Year, Birthday.Month, Birthday.Day);
                return (birthday - now).Days;
            }
        }

        public bool Gender { get; set; }

        [RegularExpression(Constants.Regulars.Email, ErrorMessage = "Địa chỉ email của bạn không hợp lệ")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string YahooNick { get; set; }

        public string SkypeNick { get; set; }

        [RegularExpression(Constants.Regulars.Website, ErrorMessage = "Địa chỉ trang web này không hợp lệ")]
        public string Website { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        [ScriptIgnore]
        public string Notes { get; set; }

        public string AvatarUrl { get; set; }

        public IList<Group> Groups { get; set; }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<FriendsProvider>().InsertFriend(this);
        }

        protected override void OnDataInserting()
        {
            base.OnDataInserting();
            if (Birthday < Constants.DateTime.MinSqlDate)
            {
                Birthday = Constants.DateTime.MinSqlDate;
            }
        }

        protected override void OnDataInserted()
        {
            base.OnDataInserted();
            CacheManager.Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<FriendsProvider>().UpdateFriend(this);
        }

        protected override void OnDataUpdated()
        {
            base.OnDataUpdated();
            CacheManager.Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<FriendsProvider>().DeleteFriend(Id);
        }

        protected override void OnDataDeleted()
        {
            base.OnDataDeleted();
            CacheManager.Flush();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new SecurityException(Messages.NotAuthorization);
            }

            if (!IsNew)
            {
                if (!User.Identity.Name.Equals(CreatedBy) && !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new MyLifeSecurityException();
                }
            }
        }

        public static Friend GetFriendById(int id)
        {
            var key = "Friends_GetFriendById_" + id;
            if (CacheManager.Contains(key))
            {
                return (Friend) CacheManager[key];
            }
            var friend = ServiceManager.GetService<FriendsProvider>().GetFriendById(id);
            if (friend != null)
            {
                CacheManager.Add(key, friend);
            }
            return friend;
        }

        public static IList<Friend> GetFriends(string user)
        {
            var key = "Friends_GetFriends_" + user.ToLowerInvariant();
            if (CacheManager.Contains(key))
            {
                return (IList<Friend>) CacheManager[key];
            }
            var friends = ServiceManager.GetService<FriendsProvider>().GetFriends(user);
            CacheManager.Add(key, friends);
            return friends;
        }

        public override void Validate()
        {
            try
            {
                base.Validate();
            }
            catch (RulesException ex)
            {
                if (ex.Errors.Count() != 1 || ex.Errors.ElementAt(0).PropertyName != "Birthday")
                {
                    throw;
                }
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}