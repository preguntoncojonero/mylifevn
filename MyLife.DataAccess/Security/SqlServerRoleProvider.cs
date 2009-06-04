using System;
using System.Configuration.Provider;
using System.Data.Common;
using System.Linq;
using System.Web.Security;
using MyLife.Web.Security;

namespace MyLife.DataAccess.Security
{
    public class SqlServerRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var context = new MyLifeEntities();
            var users =
                context.tblUsers.Include("Roles").Where(Utils.BuildContainsExpression<tblUsers, string>(item => item.UserName, usernames))
                    .ToList();
            var roles =
                context.tblRoles.Where(Utils.BuildContainsExpression<tblRoles, string>(item => item.Name, roleNames)).
                    ToList();

            foreach (var user in users)
            {
                user.Roles.Load();
                foreach (var role in roles)
                {
                    if (!user.Roles.Contains(role))
                    {
                        user.Roles.Add(role);
                    }
                }
            }
            context.SaveChanges();
        }

        public override void CreateRole(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 255, "roleName");
            var context = new MyLifeEntities();
            var obj = new tblRoles {Name = roleName};
            context.AddTotblRoles(obj);

            try
            {
                context.SaveChanges();
            }
            catch (DbException)
            {
                throw new ProviderException(string.Format("The role '{0}' already exists.", roleName));
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var context = new MyLifeEntities();
            var obj = context.tblRoles.Where(item => item.Name == roleName).FirstOrDefault();
            if (obj != null)
            {
                if (throwOnPopulatedRole && obj.Users.Count() > 0)
                {
                    throw new ProviderException("This role cannot be deleted because there are users present in it.");
                }
                context.DeleteObject(obj);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var context = new MyLifeEntities();
            var list =
                context.tblRoles.Where(item => item.Name == roleName).SelectMany(item => item.Users).Where(
                    item => item.UserName.Contains(usernameToMatch)).Select(item => item.UserName).ToList();
            return list.ToArray();
        }

        public override string[] GetAllRoles()
        {
            var context = new MyLifeEntities();
            var list = context.tblRoles.Select(item => item.Name).ToList();
            return list.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var context = new MyLifeEntities();
            var list =
                context.tblUsers.Where(item => item.UserName == username).SelectMany(item => item.Roles).Select(
                    item => item.Name).ToList();
            return list.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var context = new MyLifeEntities();
            var list =
                context.tblRoles.Where(item => item.Name == roleName).SelectMany(item => item.Users).Select(
                    item => item.UserName).ToList();
            return list.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var context = new MyLifeEntities();
            return
                context.tblUsers.Where(item => item.UserName == username).SelectMany(item => item.Roles).Where(
                    item => item.Name == roleName).Count() > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var context = new MyLifeEntities();
            var users =
                context.tblUsers.Include("Roles").Where(Utils.BuildContainsExpression<tblUsers, string>(item => item.UserName, usernames))
                    .ToList();
            var roles =
                context.tblRoles.Where(Utils.BuildContainsExpression<tblRoles, string>(item => item.Name, roleNames)).
                    ToList();

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    if (user.Roles.Contains(role))
                    {
                        user.Roles.Remove(role);
                    }
                }
            }
            context.SaveChanges();
        }

        public override bool RoleExists(string roleName)
        {
            var context = new MyLifeEntities();
            return context.tblRoles.Count(item => item.Name == roleName) > 0;
        }
    }
}