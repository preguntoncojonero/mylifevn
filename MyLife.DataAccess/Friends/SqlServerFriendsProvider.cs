using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using MyLife.Web.Friends;

namespace MyLife.DataAccess.Friends
{
    public class SqlServerFriendsProvider : FriendsProvider
    {
        public override int InsertGroup(Group group)
        {
            var context = new FriendsEntities();
            var obj = new tblFriends_Groups();
            group.CopyToObject(obj);
            context.AddTotblFriends_Groups(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateGroup(Group group)
        {
            var context = new FriendsEntities();
            var obj = new tblFriends_Groups {Id = group.Id};
            context.AttachTo("tblFriends_Groups", obj);
            group.CopyToObject(obj);
            context.SaveChanges();
        }

        public override void DeleteGroup(int id)
        {
            var context = new FriendsEntities();
            var obj = context.tblFriends_Groups.Include("Friends").Where(item => item.Id == id).FirstOrDefault();
            if (obj != null)
            {
                obj.Friends.Clear();
                context.DeleteObject(obj);
                context.SaveChanges();
            }
        }

        public override IList<Group> GetAllGroups(string user)
        {
            var context = new FriendsEntities();
            var list =
                context.tblFriends_Groups.Where(item => item.CreatedBy == user).OrderBy(item => item.Name).ToList();
            return Convert(list);
        }

        private static IList<Group> Convert(List<tblFriends_Groups> list)
        {
            var retval = new List<Group>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        public override Group GetGroupById(int id)
        {
            var context = new FriendsEntities();
            var obj = context.tblFriends_Groups.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        private static Group Convert(tblFriends_Groups obj)
        {
            if (obj == null)
            {
                return null;
            }
            var group = new Group(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            group.CopyFromObject(obj);
            return group;
        }

        public override Friend GetFriendById(int id)
        {
            var context = new FriendsEntities();
            var obj = context.tblFriends_Friends.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        private static Friend Convert(tblFriends_Friends obj)
        {
            if (obj == null)
            {
                return null;
            }
            var friend = new Friend(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            friend.CopyFromObject(obj);
            friend.Groups = new List<Group>(Convert(obj.Groups));
            return friend;
        }

        private static IList<Group> Convert(EntityCollection<tblFriends_Groups> entityCollection)
        {
            var retval = new List<Group>();
            foreach (var obj in entityCollection)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        public override int InsertFriend(Friend friend)
        {
            var context = new FriendsEntities();
            var obj = new tblFriends_Friends();
            context.AddTotblFriends_Friends(obj);
            friend.CopyToObject(obj);

            if (friend.Groups != null)
            {
                foreach (var group in friend.Groups)
                {
                    var item = new tblFriends_Groups {Id = group.Id};
                    context.AttachTo("tblFriends_Groups", item);
                    obj.Groups.Add(item);
                }
            }

            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateFriend(Friend friend)
        {
            var context = new FriendsEntities();
            var obj = context.tblFriends_Friends.Include("Groups").Where(item => item.Id == friend.Id).FirstOrDefault();
            if (obj != null)
            {
                obj.Groups.Clear();
                friend.CopyToObject(obj);
                if (friend.Groups.Count > 0)
                {
                    var groups =
                        context.tblFriends_Groups.Where(
                            Utils.BuildContainsExpression<tblFriends_Groups, int>(item => item.Id,
                                                                                  friend.Groups.Select(item => item.Id)));
                    foreach (var group in groups)
                    {
                        obj.Groups.Add(group);
                    }
                }

                context.SaveChanges();
            }
        }

        public override void DeleteFriend(int id)
        {
            var context = new FriendsEntities();
            var obj = context.tblFriends_Friends.Include("Groups").Where(item => item.Id == id).FirstOrDefault();
            if (obj != null)
            {
                obj.Groups.Clear();
                context.DeleteObject(obj);
                context.SaveChanges();
            }
        }

        public override IList<Friend> GetFriends(string user)
        {
            var context = new FriendsEntities();
            var list =
                context.tblFriends_Friends.Include("Groups").Where(item => item.CreatedBy == user).OrderBy(
                    item => item.Letter).ToList();
            return Convert(list);
        }

        private static IList<Friend> Convert(List<tblFriends_Friends> list)
        {
            var retval = new List<Friend>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }
    }
}