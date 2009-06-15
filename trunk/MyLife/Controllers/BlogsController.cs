using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MyLife.Models;
using MyLife.Web;
using MyLife.Web.Blogs;
using MyLife.Web.Rss;
using Messages=MyLife.Web.Blogs.Messages;

namespace MyLife.Controllers
{
    [HandleError]
    public class BlogsController : BaseController
    {
        public string CurrentWeb
        {
            get { return RouteData.Values[Constants.RouteData.User].ToString().ToLowerInvariant(); }
        }

        public ActionResult Default()
        {
            return Posts(1);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetComments()
        {
            var pageIndex = Request.Form["PageIndex"];
            var pageSize = Request.Form["PageSize"];
            if (!string.IsNullOrEmpty(pageIndex) && !string.IsNullOrEmpty(pageSize))
            {
                int total;
                var index = Convert.ToInt32(pageIndex);
                var size = Convert.ToInt32(pageSize);
                var comments = ServiceManager.GetService<BlogsProvider>().GetCommentsOfBlog(1, index - 1, size,
                                                                                            out total);
                return Json(new {Status = true, Data = comments});
            }
            throw new NotImplementedException();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditComment()
        {
            var id = Convert.ToInt32(Request.Form["comment.Id"]);
            var comment = id == 0 ? new Comment() : Comment.GetCommentById(id);
            UpdateModel(comment, "comment");
            comment.Save();
            return Json(new AjaxModel {Status = true, Message = Messages.AddCommentSuccess});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteCategory()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var category = Web.Blogs.Category.GetCategoryById(id);
            category.Delete();
            var categories = Web.Blogs.Category.GetCategoriesOfBlog(category.BlogId);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteComment()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var comment = Comment.GetCommentById(id);
            comment.Delete();
            return Json(new AjaxModel {Status = true});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditBlogroll()
        {
            var id = Convert.ToInt32(Request.Form["blogroll.Id"]);
            var blogroll = id == 0 ? new Blogroll() : Blogroll.GetBlogrollById(id);
            UpdateModel(blogroll, "blogroll");
            if (blogroll.IsNew)
            {
                var blog = Blog.GetBlogByName(User.Identity.Name);
                blogroll.BlogId = blog.Id;
            }
            blogroll.Save();

            var blogrolls = Blogroll.GetBlogrolls(blogroll.BlogId);
            return Json(new AjaxModel {Status = true, Data = blogrolls});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteBlogroll()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var blogroll = Blogroll.GetBlogrollById(id);
            blogroll.Delete();

            var blogrolls = Blogroll.GetBlogrolls(blogroll.BlogId);
            return Json(new AjaxModel {Status = true, Data = blogrolls});
        }

        public ActionResult Posts(int indexOfPage)
        {
            if (indexOfPage < 1)
            {
                indexOfPage = 1;
            }
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }

            int total;
            var posts = Post.GetPostsOfBlog(blog.Id, indexOfPage, blog.PostsPerPage, out total);

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog", blog.CreatedBy);
            ViewData[Constants.ViewData.Blogs.Header] = "Các bài viết mới nhất";
            ViewData[Constants.ViewData.Blogs.IsPostList] = true;

            ViewData[Constants.ViewData.PageNavigator.IndexOfPage] = indexOfPage;
            ViewData[Constants.ViewData.PageNavigator.TotalPages] = Utils.CalcTotalPages(total, blog.PostsPerPage);
            ViewData[Constants.ViewData.PageNavigator.BaseUrl] = string.Format("/{0}/blog/posts", blog.CreatedBy);

            CommonTasks(blog);
            return View("Posts", blog.Theme, posts);
        }

        public ActionResult PostBySlug(string slug)
        {
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }

            var post = Post.GetPostBySlug(blog.Id, slug);
            if (post == null)
            {
                throw new HttpException(404, Messages.PostNotExist);
            }

            post.IncreaseViewCount();

            ViewData[Constants.ViewData.Title] = string.Format("{0} - {1}'s blog", post.Title, post.CreatedBy);
            ViewData[Constants.ViewData.Blogs.IsPostList] = false;
            CommonTasks(blog);

            if (Comment.VerifyCommentsEnabled(blog, post, false))
            {
                var comment = new Comment {PostId = post.Id};
                if (User.Identity.IsAuthenticated)
                {
                    comment.Name = User.Identity.Name;
                    comment.Website = string.Format("{0}{1}/blog", MyLifeContext.AbsoluteWebRoot, User.Identity.Name);
                }
                ViewData[Constants.ViewData.Blogs.Comment] = comment;
            }

            return View("Post", blog.Theme, post);
        }

        public ActionResult Category(string slug, int indexOfPage)
        {
            if (indexOfPage < 1)
            {
                indexOfPage = 1;
            }
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }

            var category = Web.Blogs.Category.GetCategoryBySlug(blog.Id, slug);
            if (category == null)
            {
                throw new HttpException(404, Messages.CategoryNotExist);
            }

            int total;
            var posts = Post.GetPostsOfCategory(category.Id, indexOfPage, blog.PostsPerPage, out total);

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog - {1}", blog.CreatedBy, category.Name);
            ViewData[Constants.ViewData.Blogs.IsPostList] = true;
            ViewData[Constants.ViewData.Blogs.Header] = "Chủ đề: " + category.Name;

            ViewData[Constants.ViewData.PageNavigator.IndexOfPage] = indexOfPage;
            ViewData[Constants.ViewData.PageNavigator.TotalPages] = Utils.CalcTotalPages(total, blog.PostsPerPage);
            ViewData[Constants.ViewData.PageNavigator.BaseUrl] = string.Format("/{0}/blog/category/{1}", blog.CreatedBy,
                                                                               category.Slug);

            CommonTasks(blog);

            return View("Posts", blog.Theme, posts);
        }

        public ActionResult XmlRpc()
        {
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }
            var rootUrl = MyLifeContext.AbsoluteWebRoot;
            XNamespace ns = "http://archipelago.phrasewise.com/rsd";
            var rsdDocument =
                new XDocument(new XElement(ns + "rsd", new XAttribute("version", "1.0"),
                                           new XElement(ns + "service",
                                                        new XElement(ns + "engineName", "MyLifeVn"),
                                                        new XElement(ns + "engineLink", rootUrl),
                                                        new XElement(ns + "homePageLink",
                                                                     rootUrl + blog.CreatedBy + "/blog"),
                                                        new XElement(ns + "apis",
                                                                     new XElement(ns + "api",
                                                                                  new XAttribute("name", "MetaWeblog"),
                                                                                  new XAttribute("preferred", "true"),
                                                                                  new XAttribute("apiLink",
                                                                                                 rootUrl +
                                                                                                 "metaweblog.axd"),
                                                                                  new XAttribute("blogID", blog.Id))
                                                            ))));
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + rsdDocument, "text/xml");
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditPost()
        {
            var id = Convert.ToInt32(Request.Form["post.Id"]);
            var post = id == 0 ? new Post() : Post.GetPostById(id);
            TryUpdateModel(post, "post");

            post.Categories.Clear();
            var categories = Request.Form["post.Categories"];
            if (categories != null)
            {
                var ids = categories.Split(',');
                foreach (var categoryId in ids)
                {
                    var category = Web.Blogs.Category.GetCategoryById(Convert.ToInt32(categoryId));
                    if (category != null)
                    {
                        post.Categories.Add(category);
                    }
                }
            }

            if (post.IsNew)
            {
                var blog = Blog.GetBlogByName(User.Identity.Name);
                post.BlogId = blog.Id;
            }

            post.Save();

            return Json(new AjaxModel
                            {
                                Status = true,
                                Data = post.RelativeUrl,
                                Message = "Bài viết của bạn đã được xuất bản"
                            });
        }

        [Authorize]
        public ActionResult AddPost()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog - {1}", blog.CreatedBy, "Thêm bài viết mới");
            ViewData[Constants.ViewData.Blogs.Header] = "Thêm bài viết mới";
            CommonTasks(blog);

            var post = new Post {Published = true, CommentsEnabled = true};
            return View("PostEditor", blog.Theme, post);
        }

        [Authorize]
        public ActionResult EditPost(int id)
        {
            var post = Post.GetPostById(id);
            if (post == null)
            {
                throw new HttpException(404, "Bài viết này không tồn tại");
            }

            var blog = Blog.GetBlogByName(post.CreatedBy);
            CommonTasks(blog);

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog - {1}", blog.CreatedBy, "Chỉnh sửa bài viết");
            ViewData[Constants.ViewData.Blogs.Header] = "Chỉnh sửa bài viết";
            return View("PostEditor", blog.Theme, post);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletePost(int id)
        {
            var post = Post.GetPostById(id);
            post.Delete();

            var posts = Post.GetAllPosts(post.BlogId);
            return Json(new AjaxModel {Status = true, Data = posts});
        }

        [Authorize]
        public ActionResult Settings()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);

            if (IsPostRequest)
            {
                UpdateModel(blog, "blog");
                blog.Save();
                return Json(new AjaxModel
                                {
                                    Status = true,
                                    Message = "Các thiết lập của bạn đã được ghi nhận"
                                });
            }

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s - Thiết lập hệ thống", blog.CreatedBy);
            ViewData[Constants.ViewData.Blogs.Themes] = new SelectList(Blog.GetThemes(), blog.Theme);
            ViewData[Constants.ViewData.Blogs.Header] = "Thiết lập blog của bạn";
            ViewData[Constants.ViewData.Blogs.Posts] = Post.GetAllPosts(blog.Id);
            CommonTasks(blog);

            return View("Settings", blog.Theme, blog);
        }

        public ActionResult Rss()
        {
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }

            int total;
            var posts = Post.GetPostsOfBlog(blog.Id, 1, blog.PostsPerFeed, out total);
            var baseUri = MyLifeContext.AbsoluteWebRoot;
            Response.ContentType = "text/xml";
            var rssWriter = new RssWriter(Response.OutputStream, Encoding.UTF8) {Version = RssVersion.RSS20};
            var rssChanel = new RssChannel
                                {
                                    Title = blog.Name,
                                    Description = blog.Description,
                                    Link = new Uri(baseUri, blog.CreatedBy + "/blog"),
                                    LastBuildDate = DateTime.UtcNow
                                };

            foreach (var post in posts)
            {
                rssChanel.Items.Add(new RssItem
                                        {
                                            Title = post.Title,
                                            Description = post.Content,
                                            Link = new Uri(baseUri, post.RelativeUrl),
                                            Author = post.CreatedBy,
                                            PubDate = post.CreatedDate
                                        });
            }

            rssWriter.Write(rssChanel);
            rssWriter.Close();
            Response.Flush();
            return new EmptyResult();
        }

        public ActionResult SiteMap()
        {
            var blog = Blog.GetBlogByName(CurrentWeb);
            if (blog == null)
            {
                throw new HttpException(404, Messages.BlogNotExist);
            }
            var rootUrl = MyLifeContext.AbsoluteWebRoot;
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            int total;
            var posts = Post.GetPostsOfBlog(blog.Id, 1, int.MaxValue, out total);
            var urlset = new XElement(ns + "urlset");
            foreach (var post in posts)
            {
                var url = new XElement(ns + "url");
                url.Add(new XElement(ns + "loc",
                                     string.Format("{0}{1}/blog/post/{2}", rootUrl, post.CreatedBy, post.Slug)));
                url.Add(new XElement(ns + "lastmod",
                                     post.ModifiedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                urlset.Add(url);
            }
            var document = new XDocument(urlset);
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + document, "text/xml");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult GetCategories()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);
            var categories = Web.Blogs.Category.GetCategoriesOfBlog(blog.Id);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult GetPosts()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);
            var posts = Post.GetAllPosts(blog.Id);
            return Json(new AjaxModel {Status = true, Data = posts});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult GetBlogrolls()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);
            var blogrolls = Blogroll.GetBlogrolls(blog.Id);
            return Json(new AjaxModel {Status = true, Data = blogrolls});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddOrEditCategory()
        {
            var id = Convert.ToInt32(Request.Form["category.Id"]);
            var category = id == 0 ? new Category() : Web.Blogs.Category.GetCategoryById(id);
            UpdateModel(category, "category");
            if (category.IsNew)
            {
                var blog = Blog.GetBlogByName(User.Identity.Name);
                category.BlogId = blog.Id;
            }

            category.Save();

            var categories = Web.Blogs.Category.GetCategoriesOfBlog(category.BlogId);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        public ActionResult Search()
        {
            var blog = Blog.GetBlogByName(CurrentWeb);
            var keyword = Request.QueryString["keyword"];
            var posts = Post.Search(blog.Id, keyword);

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog", blog.CreatedBy);
            ViewData[Constants.ViewData.Blogs.Header] = "Kết quả tìm kiếm cho từ khóa: " + keyword;
            ViewData[Constants.ViewData.Blogs.IsPostList] = true;

            ViewData[Constants.ViewData.PageNavigator.IndexOfPage] = 0;
            ViewData[Constants.ViewData.PageNavigator.TotalPages] = 1;

            CommonTasks(blog);

            return View("Posts", blog.Theme, posts);
        }

        [Authorize]
        public ActionResult Import()
        {
            var blog = Blog.GetBlogByName(User.Identity.Name);

            if (IsPostRequest)
            {
                var feed = RssFeed.Read(Request.Form["Url"]);
                var errors = 0;
                foreach (RssItem item in feed.Channels[0].Items)
                {
                    var post = new Post(0, item.PubDate, blog.CreatedBy, item.PubDate, blog.CreatedBy)
                                   {
                                       Title = item.Title,
                                       BlogId = blog.Id,
                                       Content = item.Description,
                                       Published = true,
                                       CommentsEnabled = true
                                   };

                    try
                    {
                        post.Save();
                    }
                    catch (Exception)
                    {
                        errors++;
                    }
                }

                var message = errors == 0
                                  ? string.Format("Đã chuyển thành công {0} bài viết vào blog của bạn",
                                                  feed.Channels[0].Items.Count)
                                  : string.Format(
                                        "Đã chuyển thành công {0} bài viết, thất bại {1} bài viết vào blog của bạn.",
                                        feed.Channels[0].Items.Count - errors, errors);

                return Json(new AjaxModel
                                {
                                    Status = true,
                                    Message = message
                                });
            }

            ViewData[Constants.ViewData.Title] = string.Format("{0}'s blog - Chuyển đổi rss feed thành bài viết",
                                                               blog.CreatedBy);
            ViewData[Constants.ViewData.Blogs.Header] = "Chuyển đổi các bài viết từ các blog khác";
            CommonTasks(blog);
            return View("Import", blog.Theme);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportYahoo360Archive()
        {
            var success = 0;
            var error = 0;
            var total = 0;

            if (Request.Files.Count > 0)
            {
                var content = new StreamReader(Request.Files[0].InputStream).ReadToEnd();

                var posts =
                    Regex.Split(content, "\n-----\n--------\n").Where(str => !string.IsNullOrEmpty(str)).ToArray();

                var blog = Blog.GetBlogByName(User.Identity.Name);

                StringReader reader;
                foreach (var str in posts)
                {
                    var post = new Post();
                    reader = new StringReader(str);

                    // Abort AUTHOR:
                    reader.ReadLine();

                    // Read TITLE:
                    post.Title = reader.ReadLine().Substring(7);

                    // Read DATE:
                    post.PublishedDate = DateTime.ParseExact(reader.ReadLine().Substring(6, 10), "MM/dd/yyyy", null);

                    // Read STATUS:
                    post.Published = reader.ReadLine().Substring(8) == "publish";

                    // Read BODY:
                    reader.ReadLine();
                    post.Content = reader.ReadLine();

                    post.BlogId = blog.Id;

                    try
                    {
                        post.Save();
                    }
                    catch
                    {
                        error++;
                    }
                }

                total = posts.Length;
                success = total - error;
            }

            return Content(string.Format("Đã chuyển đổi thành công {0} trong tổng số {1} bài viết", success, total));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult UpdateStatus()
        {
            var status = Request.Form["Status"].Trim();
            var blog = Blog.GetBlogByName(User.Identity.Name);
            blog.Status = status;
            blog.Save();
            return Json(new {Status = true});
        }

        private void CommonTasks(Blog blog)
        {
            ViewData[Constants.ViewData.Blogs.Name] = blog.Name;
            ViewData[Constants.ViewData.Blogs.Slogan] = blog.Description;
            ViewData[Constants.ViewData.Blogs.Categories] = Web.Blogs.Category.GetCategoriesOfBlog(blog.Id);
            ViewData[Constants.ViewData.Blogs.RecentPosts] = Post.GetRecentPosts(blog.Id, blog.NumberOfRecentPosts);
            ViewData[Constants.ViewData.Blogs.RecentComments] = Comment.GetRecentComments(blog.Id,
                                                                                          blog.NumberOfRecentComments);
            ViewData[Constants.ViewData.Blogs.Blogrolls] = Blogroll.GetBlogrolls(blog.Id);
            ViewData[Constants.ViewData.RsdLink] =
                string.Format(
                    "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{0}{1}/blog/xmlrpc\" />",
                    MyLifeContext.AbsoluteWebRoot, blog.CreatedBy);
            ViewData[Constants.ViewData.FeedLink] =
                string.Format(
                    "<link type=\"application/rss+xml\" rel=\"alternate\" title=\"{2}\" href=\"{0}{1}/blog/rss\" />",
                    MyLifeContext.AbsoluteWebRoot, blog.CreatedBy, blog.Name);
        }
    }
}