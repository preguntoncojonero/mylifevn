using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.Friends
{
    public abstract class FriendsProvider : ProviderBase
    {
        // Group
        public abstract int InsertGroup(Group group);
        public abstract void UpdateGroup(Group group);
        public abstract void DeleteGroup(int id);
        public abstract IList<Group> GetAllGroups(string user);
        public abstract Group GetGroupById(int id);

        // Friend
        public abstract Friend GetFriendById(int id);
        public abstract int InsertFriend(Friend friend);
        public abstract void UpdateFriend(Friend friend);
        public abstract void DeleteFriend(int id);
        public abstract IList<Friend> GetFriends(string user);
    }
}