using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using MyLife.Models;
using MyLife.Web.Friends;

namespace MyLife.Controllers
{
    [HandleError]
    public class FriendsController : BaseController
    {
        [Authorize]
        public ActionResult Default()
        {
            ViewData[Constants.ViewData.Title] = MyLifeContext.Settings.Friends.Title;
            ViewData["Cities"] = new SelectList(Utils.GetCities());
            return View("Default", MyLifeContext.Settings.Friends.Theme);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult GetGroups()
        {
            var groups = Group.GetAllGroups(User.Identity.Name);
            var result = new List<Group>(groups);
            result.Insert(0, new Group {Name = "Tất cả"});
            return Json(new AjaxModel {Status = true, Data = result});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditFriend()
        {
            var id = Convert.ToInt32(Request.Form["friend.Id"]);
            var friend = id == 0 ? new Friend() : Friend.GetFriendById(id);
            TryUpdateModel(friend, "friend");
            friend.Letter = friend.Letter.ToUpperInvariant();
            var birthday = Request.Form["friend.Birthday"];
            if (!string.IsNullOrEmpty(birthday))
            {
                DateTime value;
                DateTime.TryParseExact(birthday, "d/M/yyyy", null, DateTimeStyles.None, out value);
                friend.Birthday = value;
            }

            var groups = Request.Form["friend.Groups"];
            if (!string.IsNullOrEmpty(groups))
            {
                foreach (var group in groups.Split(','))
                {
                    friend.Groups.Add(new Group(Convert.ToInt32(group)));
                }
            }

            if (!string.IsNullOrEmpty(Request.Form["friend.Gravatar"]))
            {
                friend.AvatarUrl = Utils.GravatarUrl(friend.Email, 80);
            }

            if (string.IsNullOrEmpty(friend.AvatarUrl))
            {
                friend.AvatarUrl = string.Format("{0}avatar/default", MyLifeContext.AbsoluteWebRoot);
            }

            friend.Save();

            var friends = Friend.GetFriends(User.Identity.Name);
            return Json(new AjaxModel {Status = true, Data = friends});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditGroup()
        {
            var id = Convert.ToInt32(Request.Form["group.Id"]);
            var group = id == 0 ? new Group() : Group.GetGroupById(id);
            UpdateModel(group, "group");
            group.Save();

            return GetGroups();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFriends()
        {
            var friends = Friend.GetFriends(User.Identity.Name);
            return Json(new AjaxModel {Status = true, Data = friends});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteFriend()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var friend = Friend.GetFriendById(id);
            friend.Delete();

            var friends = Friend.GetFriends(User.Identity.Name);
            return Json(new AjaxModel {Status = true, Data = friends});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteGroup()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var group = Group.GetGroupById(id);
            group.Delete();

            return GetGroups();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendMail()
        {
            var user = Membership.GetUser(User.Identity.Name);
            var subject = Request.Form["email.Subject"];
            var tos = Request.Form["email.Tos"].Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            var content = Request.Form["email.Content"];
            Net.Mail.SendMail.Send(user.Email, new[] {user.Email}, null, tos, subject, content);
            return Json(new {Status = true, Message = "Bức thư của bạn đã được gửi đi"});
        }
    }
}