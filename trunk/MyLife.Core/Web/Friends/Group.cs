using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace MyLife.Web.Friends
{
    public class Group : BizObject<Group, int>
    {
        private static readonly ICacheManager CacheManager;

        static Group()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.Friends.CacheProvider);
        }

        public Group()
        {
        }

        public Group(int id) : base(id)
        {
        }

        public Group(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn chưa nhập tên nhóm")]
        public string Name { get; set; }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<FriendsProvider>().InsertGroup(this);
        }

        protected override void OnDataInserted()
        {
            base.OnDataInserted();
            CacheManager.Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<FriendsProvider>().UpdateGroup(this);
        }

        protected override void OnDataUpdated()
        {
            base.OnDataUpdated();
            CacheManager.Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<FriendsProvider>().DeleteGroup(Id);
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
                if (
                    !User.Identity.Name.Equals(CreatedBy, StringComparison.InvariantCultureIgnoreCase) &&
                    !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new SecurityException(Messages.NotAuthorization);
                }
            }
        }

        public static IList<Group> GetAllGroups(string user)
        {
            var key = "Friends_GetAllGroups_" + user.ToLowerInvariant();
            if (CacheManager.Contains(key))
            {
                return (IList<Group>) CacheManager[key];
            }
            var groups = ServiceManager.GetService<FriendsProvider>().GetAllGroups(user);
            CacheManager.Add(key, groups);
            return groups;
        }

        public static Group GetGroupById(int id)
        {
            var key = "Friends_GetGroupById_" + id;
            if (CacheManager.Contains(key))
            {
                return (Group) CacheManager[key];
            }
            var group = ServiceManager.GetService<FriendsProvider>().GetGroupById(id);
            if (group != null)
            {
                CacheManager.Add(key, group);
            }

            return group;
        }
    }
}