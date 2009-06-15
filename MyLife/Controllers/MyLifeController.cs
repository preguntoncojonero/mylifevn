using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using MyLife.Linq;
using MyLife.Models;
using MyLife.Net.Mail;
using MyLife.Web.Blogs;
using MyLife.Web.Mvc;
using Messages=MyLife.Web.Messages;

namespace MyLife.Controllers
{
    [HandleError]
    public class MyLifeController : BaseController
    {
        public string CurrentWeb
        {
            get { return RouteData.Values[Constants.RouteData.User] as string; }
        }

        [Authorize]
        public ActionResult Default()
        {
            var profile = GetProfile();
            ViewData[Constants.ViewData.Title] = "Trang nhà của " + profile.UserName;
            ViewData["News"] = Web.News.News.GetNews(10);
            return View("MyLife", MyLifeContext.Settings.Theme);
        }

        public ActionResult Explore(int indexOfPage)
        {
            if (indexOfPage < 1)
            {
                indexOfPage = 1;
            }

            int total;
            var users = Membership.GetAllUsers(indexOfPage - 1, 20, out total);
            ViewData[Constants.ViewData.Title] = "Các thành viên của MyLife";

            ViewData[Constants.ViewData.PageNavigator.BaseUrl] = "/explore";
            ViewData[Constants.ViewData.PageNavigator.IndexOfPage] = indexOfPage;
            ViewData[Constants.ViewData.PageNavigator.TotalPages] = Utils.CalcTotalPages(total, 20);

            return View("Explore", MyLifeContext.Settings.Theme, users);
        }

        public ActionResult Home()
        {
            ViewData[Constants.ViewData.Title] = MyLifeContext.Settings.Name + " - " + MyLifeContext.Settings.Slogan;
            ViewData["NewestUsers"] = Blog.GetNewesUsers(10);
            ViewData["RecentPosts"] = Post.GetRecentPosts(15);
            ViewData["News"] = Web.News.News.GetNews(10);
            return View("Home", MyLifeContext.Settings.Theme);
        }

        private static void OnUserRegisted(MembershipUser user)
        {
            MyLifeContext.Current.User = new GenericPrincipal(new GenericIdentity(user.UserName), null);
            FormsAuthentication.SetAuthCookie(user.UserName, false);

            var tpl = new XmlMailTemplate(Path.Combine(MyLifeContext.WorkingFolder, "App_Data\\Register.xml")) { Data = new { user.UserName } };
            tpl.Process();
            Net.Mail.SendMail.Send(user.Email, tpl.Subject, tpl.Body);

            // Register new blog
            var blog = Blog.New();
            blog.Save();
        }

        [MyLifeValidateAntiForgeryToken]
        public ActionResult Register()
        {
            if (IsPostRequest)
            {
                var username = Request.Form["UserName"].ToLowerInvariant();
                var password = Request.Form["Password"];
                var email = Request.Form["Email"];
                var obj = new AjaxModel();

                // Deny user name in bad list
                var streamReader = new StreamReader(MyLifeContext.WorkingFolder + "App_Data\\BlockUsers.txt");
                var words = streamReader.ReadToEnd().Replace(Environment.NewLine, " ").Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                streamReader.Close();
                if (new List<string>(words).Contains(username) ||
                    !Regex.Match(username, Constants.Regulars.User).Success)
                {
                    obj.Message = string.Format("Tên đăng nhập '{0}' không được chấp nhận", username);
                    goto Return;
                }

                MembershipCreateStatus status;
                var user = Membership.CreateUser(username, password, email, null, null, true, out status);

                switch (status)
                {
                    case MembershipCreateStatus.Success:
                        OnUserRegisted(user);

                        obj.Status = true;
                        obj.Message = "Bạn đã đăng ký thành công.";
                        obj.RedirectUrl = "/" + user.UserName;
                        goto Return;
                    case MembershipCreateStatus.DuplicateEmail:
                        obj.Message = "Địa chỉ email này đã có người sử dụng.";
                        goto Return;
                    case MembershipCreateStatus.DuplicateUserName:
                        obj.Message = "Tên đăng nhập này đã có người sử dụng.";
                        goto Return;
                    case MembershipCreateStatus.InvalidEmail:
                        obj.Message = "Địa chỉ email không hợp lệ";
                        goto Return;
                    case MembershipCreateStatus.InvalidPassword:
                        obj.Message = "Mật khẩu của bạn quá ngắn hoặc quá đơn giản";
                        goto Return;
                    default:
                        obj.Message = "Hệ thống không chấp nhận việc đăng ký thành viên, bạn hãy thử lại sau";
                        goto Return;
                }

                Return:
                return Json(obj);
            }

            ViewData[Constants.ViewData.Title] = MyLifeContext.Settings.Name + " - Đăng ký tài khoản";
            return View("Register", MyLifeContext.Settings.Theme);
        }

        [MyLifeValidateAntiForgeryToken]
        public ActionResult Login()
        {
            if (IsPostRequest)
            {
                var username = Request.Form["UserName"].ToLowerInvariant();
                var password = Request.Form["Password"];
                var returnUrl = Request.Form["ReturnUrl"];
                var obj = new AjaxModel();
                if (Membership.ValidateUser(username, password))
                {
                    HttpContext.User = new GenericPrincipal(new GenericIdentity(username), null);
                    FormsAuthentication.SetAuthCookie(username, false);
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = string.Format("/{0}", username);
                    }
                    obj.Status = true;
                    obj.RedirectUrl = returnUrl;
                    obj.Message = "Bạn đã đăng nhập thành công";
                }
                else
                {
                    obj.Message = "Tên đăng nhập hoặc mật khẩu của bạn không hợp lệ";
                }

                return Json(obj);
            }

            ViewData[Constants.ViewData.Title] = MyLifeContext.Settings.Name + " - " + Messages.Login;
            return View("Login", MyLifeContext.Settings.Theme);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [MyLifeValidateAntiForgeryToken]
        public ActionResult ResetPassword()
        {
            if (IsPostRequest)
            {
                var username = Request.Form["user.UserName"].ToLowerInvariant();
                var obj = new AjaxModel();

                var user = Membership.GetUser(username);
                if (user == null)
                {
                    obj.Message = "Tên đăng nhập này không tồn tại";
                    goto Return;
                }

                if (user.IsLockedOut)
                {
                    obj.Message = "Tài khoản của bạn đã bị khóa, hãy liên hệ với người quản lý";
                    obj.RedirectUrl = "/contact";
                    goto Return;
                }

                var password = user.ResetPassword();

                if (string.IsNullOrEmpty(password))
                {
                    obj.Message = "Có lỗi trong quá trình khởi tạo lại mật khẩu";
                    goto Return;
                }

                obj.Status = true;
                obj.Message = "Mật khẩu đã được khởi tạo và gửi về hòm mail của bạn";
                obj.RedirectUrl = "/login";

                var tpl = new XmlMailTemplate(Server.MapPath("App_Data/ResetPassword.xml"))
                              {
                                  Data = new
                                             {
                                                 user.UserName,
                                                 Password = password
                                             }
                              };
                tpl.Process();
                Net.Mail.SendMail.Send(user.Email, tpl.Subject, tpl.Body);

                Return:
                return Json(obj);
            }

            ViewData[Constants.ViewData.Title] = MyLifeContext.Settings.Name + " - " + Messages.ResetPassword;
            return View("ResetPassword", MyLifeContext.Settings.Theme);
        }

        [Authorize]
        [MyLifeValidateAntiForgeryToken]
        public ActionResult ChangePassword()
        {
            if (IsPostRequest)
            {
                var oldPassword = Request.Form["OldPassword"].Trim();
                var newPassword = Request.Form["NewPassword"].Trim();
                var obj = new AjaxModel();
                var user = Membership.GetUser(User.Identity.Name);
                obj.Status = user.ChangePassword(oldPassword, newPassword);
                if (obj.Status)
                {
                    obj.Message = "Bạn đã thay đổi mật khẩu thành công";
                    obj.RedirectUrl = string.Format("/{0}/profile", User.Identity.Name);
                }
                else
                {
                    obj.Message = "Bạn không thể thay đổi mật khẩu, hãy thử lại";
                }
                return Json(obj);
            }

            ViewData[Constants.ViewData.Title] = string.Format("{0} - {1}", MyLifeContext.Settings.Name,
                                                               Messages.ChangePassword);
            return View("ChangePassword", MyLifeContext.Settings.Theme);
        }

        public ActionResult SiteMap()
        {
            var rootUrl = MyLifeContext.AbsoluteWebRoot;
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemapindex = new XElement(ns + "sitemapindex");
            var users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                var sitemap = new XElement(ns + "sitemap");
                sitemap.Add(new XElement(ns + "loc", string.Format("{0}{1}/blog/sitemap", rootUrl, user.UserName)));
                sitemap.Add(new XElement(ns + "lastmod",
                                         user.LastLoginDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                sitemapindex.Add(sitemap);
            }
            var document = new XDocument(sitemapindex);
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + document, "text/xml");
        }

        [Authorize]
        public ActionResult Profile()
        {
            var profile = GetProfile();
            if (IsPostRequest)
            {
                UpdateModel(profile, "profile");
                profile.Save();
                return
                    Json(new AjaxModel
                             {
                                 Status = true,
                                 Message = "Hồ sơ của bạn đã được lưu trữ",
                                 RedirectUrl = "/" + profile.UserName
                             });
            }

            ViewData[Constants.ViewData.Title] = "Hồ sơ cá nhân";
            ViewData["Cities"] = new SelectList(Utils.GetCities(), profile.City);
            return View("Profile", MyLifeContext.Settings.Theme, profile);
        }

        public ActionResult Emoticon(string id)
        {
            var response = HttpContext.Response;
            var cache = response.Cache;
            var cacheDuration = TimeSpan.FromSeconds(86400);
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            response.ContentType = "image/gif";
            try
            {
                response.TransmitFile(string.Format("{0}/Content/Emoticons/{1}.gif", MyLifeContext.WorkingFolder, id));
            }
            catch
            {
                response.TransmitFile(string.Format("{0}/Content/Emoticons/{1}.gif", MyLifeContext.WorkingFolder,
                                                    "default"));
            }

            return new EmptyResult();
        }

        public ActionResult Avatar(string user)
        {
            return new AvatarResult(user);
        }

        [Authorize]
        public ActionResult Settings()
        {
            ViewData[Constants.ViewData.Title] = "Thiết lập hệ thống";
            return View("Settings");
        }

        [ValidateInput(false)]
        [Authorize(Roles = Constants.Roles.Administrators)]
        public ActionResult SendMail()
        {
            if (IsPostRequest)
            {
                var subject = Request.Form["Subject"];
                var tos = Request.Form["Tos"];
                var body = Request.Form["Body"];
                Net.Mail.SendMail.Send("nguyen.dainghia@gmail.com", new[] {"nguyen.dainghia@gmail.com"}, null,
                                       tos.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries), subject, body);
                return Json(new {Status = true});
            }

            var users = Membership.GetAllUsers();
            var emails = (from MembershipUser item in users select item).Select(item => item.Email);
            ViewData[Constants.ViewData.Title] = "Gửi thư cho các thành viên thành viên";
            return View("SendMail", MyLifeContext.Settings.Theme, emails.ToString(";"));
        }

        public ActionResult Contact()
        {
            if (IsPostRequest)
            {
                var name = Request.Form["Name"];
                var email = Request.Form["Email"];
                var content = Request.Form["Content"];

                Net.Mail.SendMail.Send(email, "nguyen.dainghia@gmail.com",
                                       string.Format("MyLife - Phản hồi từ {0}", name), content);
                return Json(new {Status = true, Message = "Phản hồi của bạn đã được ghi nhận, cảm ơn bạn!"});
            }

            ViewData[Constants.ViewData.Title] = "MyLife - Liên hệ";
            return View("Contact", MyLifeContext.Settings.Theme);
        }

        #region News

        public ActionResult News(string slug)
        {
            var news = Web.News.News.GetNewsBySlug(slug);
            if (news == null)
            {
                throw new HttpException(404, null);
            }

            ViewData[Constants.ViewData.Title] = string.Format("{0} - MyLife News", news.Title);
            ViewData["News"] = Web.News.News.GetNews(10);
            return View("NewsDetails", MyLifeContext.Settings.Theme, news);
        }

        #endregion
    }
}