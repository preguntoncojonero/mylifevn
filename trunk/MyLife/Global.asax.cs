using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MyLife.Web;
using MyLife.Web.Blogs;
using MyLife.Web.Friends;
using MyLife.Web.Links;
using MyLife.Web.MoneyBox;
using MyLife.Web.News;
using MyLife.Web.Schedulers;
using MyLife.Web.Storage;

namespace MyLife
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*rest}", new {rest = "^Content\\/.*"});

            #region Blog Controller

            routes.MapRoute("Blog-Posts", "{user}/blog/posts/{indexOfPage}",
                            new {controller = "Blogs", action = "Posts", indexOfPage = 1},
                            new {user = Constants.Regulars.User, indexOfPage = Constants.Regulars.IndexOfPage});

            routes.MapRoute("Blog-Category", "{user}/blog/category/{slug}/{indexOfPage}",
                            new {controller = "Blogs", action = "Category", indexOfPage = 1},
                            new
                                {
                                    user = Constants.Regulars.User,
                                    slug = Constants.Regulars.Slug,
                                    indexOfPage = Constants.Regulars.IndexOfPage
                                });

            routes.MapRoute("Blog-Post-Slug", "{user}/blog/post/{slug}",
                            new {controller = "Blogs", action = "PostBySlug"},
                            new {user = Constants.Regulars.User, slug = Constants.Regulars.Slug});

            routes.MapRoute("Blog-Post-Permalink", "{user}/blog/post/permalink/{id}",
                            new { controller = "Blogs", action = "Permalink" },
                            new { user = Constants.Regulars.User, id = Constants.Regulars.Id });

            routes.MapRoute("Blog-Default", "{user}/blog",
                            new {controller = "Blogs", action = "Default"},
                            new {user = Constants.Regulars.User});

            routes.MapRoute("Blog-Action-Id", "{user}/blog/{action}/{id}", new {controller = "Blogs"},
                            new {user = Constants.Regulars.User, id = Constants.Regulars.Id});

            routes.MapRoute("Blog", "{user}/blog/{action}", new {controller = "Blogs"},
                            new {user = Constants.Regulars.User});

            #endregion

            #region Friends Controller

            routes.MapRoute("Friends-Default", "{user}/friends",
                            new {controller = "Friends", action = "Default"},
                            new {user = Constants.Regulars.User});

            routes.MapRoute("Friends-Action", "{user}/friends/{action}", new {controller = "Friends"},
                            new {user = Constants.Regulars.User});

            #endregion

            #region MoneyBox Controller

            routes.MapRoute("MoneyBox-Default", "{user}/moneybox",
                            new { controller = "MoneyBox", action = "Default" },
                            new { user = Constants.Regulars.User });

            routes.MapRoute("MoneyBox-Action", "{user}/moneybox/{action}", new { controller = "MoneyBox" },
                            new { user = Constants.Regulars.User });

            #endregion

            #region MyLife Controller

            routes.MapRoute("MyLife-Default", "", new {controller = "MyLife", action = "Home"});

            routes.MapRoute("MyLife-Home", "home", new {controller = "MyLife", action = "Home"});

            routes.MapRoute("MyLife-Login", "login", new {controller = "MyLife", action = "Login"});

            routes.MapRoute("MyLife-Logout", "logout", new {controller = "MyLife", action = "Logout"});

            routes.MapRoute("MyLife-Register", "register", new {controller = "MyLife", action = "Register"});

            routes.MapRoute("MyLife-ResetPassword", "resetpassword",
                            new {controller = "MyLife", action = "ResetPassword"});

            routes.MapRoute("MyLife-ChangePassword", "changepassword",
                            new {controller = "MyLife", action = "ChangePassword"});

            routes.MapRoute("MyLife-SiteMap", "sitemap", new {controller = "MyLife", action = "SiteMap"});

            routes.MapRoute("MyLife-Contact", "contact", new {controller = "MyLife", action = "Contact"});
            
            routes.MapRoute("MyLife-SendMail", "sendmail", new {controller = "MyLife", action = "SendMail"});

            routes.MapRoute("MyLife-Explore", "explore/{indexOfPage}", new { controller = "MyLife", action = "Explore", indexOfPage = 1 });

            routes.MapRoute("MyLife-News", "news/{slug}", new {controller = "MyLife", action = "News"},
                            new {slug = Constants.Regulars.Slug});

            routes.MapRoute("MyLife-Avatar", "avatar/{user}", new {controller = "MyLife", action = "Avatar"},
                            new {user = Constants.Regulars.User});

            routes.MapRoute("MyLife-Emoticon", "emoticon/{id}", new {controller = "MyLife", action = "Emoticon"},
                            new {id = Constants.Regulars.Id});

            routes.MapRoute("MyLife", "{user}", new {controller = "MyLife", action = "Default"},
                            new {user = Constants.Regulars.User});

            routes.MapRoute("MyLife-Action", "{user}/{action}", new {controller = "MyLife"},
                            new {user = Constants.Regulars.User});

            #endregion
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            #region Register Services

            // Blogs service
            var blogsProvider = MyLifeContext.Settings.Blogs.Providers[MyLifeContext.Settings.Blogs.DefaultProvider];
            ServiceManager.RegisterService<BlogsProvider>(Type.GetType(blogsProvider.Type));

            // Storage service
            var storageProvider =
                MyLifeContext.Settings.Storage.Providers[MyLifeContext.Settings.Storage.DefaultProvider];
            ServiceManager.RegisterService<StorageProvider>(Type.GetType(storageProvider.Type));

            // Links service
            var linkProvider = MyLifeContext.Settings.Links.Providers[MyLifeContext.Settings.Links.DefaultProvider];
            ServiceManager.RegisterService<LinksProvider>(Type.GetType(linkProvider.Type));

            // Friends service
            var themesProvider =
                MyLifeContext.Settings.Friends.Providers[MyLifeContext.Settings.Friends.DefaultProvider];
            ServiceManager.RegisterService<FriendsProvider>(Type.GetType(themesProvider.Type));

            // MoneyBox service
            var moneyBoxProvider =
                MyLifeContext.Settings.MoneyBox.Providers[MyLifeContext.Settings.MoneyBox.DefaultProvider];
            ServiceManager.RegisterService<MoneyBoxProvider>(Type.GetType(moneyBoxProvider.Type));

            // News service
            var newsProvider = MyLifeContext.Settings.News.Providers[MyLifeContext.Settings.News.DefaultProvider];
            ServiceManager.RegisterService<NewsProvider>(Type.GetType(newsProvider.Type));

            #endregion

            #region Schedulers

            //var clearTemporaryFilesTask = new ClearTemporaryFilesTask();
            //TaskScheduler.AddTask(clearTemporaryFilesTask);
            TaskScheduler.StartTasks();

            #endregion
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += Environment.NewLine + ex.InnerException.Message;
            }
            message += Environment.NewLine + string.Format("Raw url: {0}", Request.RawUrl);
            Logger.Write(message);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            TaskScheduler.StopTasks();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = MyLifeContext.Current.CultureInfo;
        }
    }
}