using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using CookComputing.XmlRpc;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MyLife.Linq;

namespace MyLife.Web.Blogs.MetaWeblog
{
    public class MetaWebLogApi : XmlRpcService, IMetaWeblog
    {
        #region IMetaWeblog Members

        public bool editPost(string postid, string username, string password, XmlRpcStruct rpcstruct, bool publish)
        {
            ValidateUser(username, password);
            var post = Post.GetPostById(Convert.ToInt32(postid));
            if (post == null)
            {
                throw new XmlRpcException();
            }
            post.Title = rpcstruct["title"].ToString();
            post.Content = rpcstruct["description"].ToString();
            post.Published = publish;

            try
            {
                post.Save();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                throw new XmlRpcException(ex.Message);
            }
        }

        public XmlRpcStruct[] getCategories(string blogid, string username, string password)
        {
            ValidateUser(username, password);
            var categories = Category.GetCategoriesOfBlog(Convert.ToInt32(blogid));
            var retval = new List<XmlRpcStruct>();
            if (categories != null)
            {
                var rootUrl = MyLifeContext.AbsoluteWebRoot;
                username = username.ToLowerInvariant();
                foreach (var category in categories)
                {
                    retval.Add(new XmlRpcStruct
                                   {
                                       {"categoryid", category.Id.ToString()},
                                       {"description", category.Name},
                                       {"htmlUrl", rootUrl + username + "/blog/category/" + category.Slug},
                                       {"rssUrl", ""},
                                       {"title", category.Name}
                                   });
                }
            }
            return retval.ToArray();
        }

        public XmlRpcStruct getPost(string postid, string username, string password)
        {
            ValidateUser(username, password);
            var post = Post.GetPostById(Convert.ToInt32(postid));
            if (post == null)
            {
                throw new XmlRpcException();
            }
            var rootUrl = MyLifeContext.AbsoluteWebRoot;
            username = username.ToLowerInvariant();
            var categories = Category.GetCategoriesOfPost(post.Id);
            return new XmlRpcStruct
                       {
                           {"dateCreated", post.CreatedDate},
                           {"description", post.Content},
                           {"title", post.Title},
                           {"link", rootUrl + username + "/blog/post/" + post.Slug},
                           {"permalink", rootUrl + username + "/blog/post/permalink/" + post.Id},
                           {"postid", post.Id},
                           {"publish", post.Published},
                           {"categories", categories.ToArray(item => item.Name)}
                       };
        }

        public XmlRpcStruct[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            int total;
            var posts = Post.GetPostsOfBlog(Convert.ToInt32(blogid), 1, numberOfPosts, out total);
            var retval = new List<XmlRpcStruct>();
            if (posts != null)
            {
                foreach (var post in posts)
                {
                    retval.Add(new XmlRpcStruct
                                   {
                                       {"dateCreated", post.CreatedDate},
                                       {"description", post.Content},
                                       {"title", post.Title},
                                       {"postid", post.Id},
                                       {"publish", post.Published},
                                   });
                }
            }
            return retval.ToArray();
        }

        public string newPost(string blogid, string username, string password, XmlRpcStruct rpcstruct, bool publish)
        {
            ValidateUser(username, password);
            var blogId = Convert.ToInt32(blogid);
            var post = new Post
                           {
                               Title = rpcstruct["title"].ToString(),
                               Content = rpcstruct["description"].ToString(),
                               Published = publish,
                               BlogId = blogId,
                               CommentsEnabled = true
                           };

            try
            {
                post.Save();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                throw new XmlRpcException(ex.Message);
            }

            return post.Id.ToString();
        }

        public XmlRpcStruct newMediaObject(string blogid, string username, string password, XmlRpcStruct rpcstruct)
        {
            throw new XmlRpcException();
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            ValidateUser(username, password);
            var post = Post.GetPostById(Convert.ToInt32(postid));
            try
            {
                post.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public XmlRpcStruct[] GetUsersBlogs(string key, string username, string password)
        {
            ValidateUser(username, password);
            var blog = Blog.GetBlogByName(username);
            if (blog == null)
            {
                throw new XmlRpcException(Messages.BlogNotExist);
            }

            return new[]
                       {
                           new XmlRpcStruct
                               {
                                   {"blogid", blog.Id.ToString()},
                                   {"blogName", blog.Name},
                                   {"url", MyLifeContext.AbsoluteWebRoot + username + "/blog"},
                               }
                       };
        }

        public XmlRpcStruct GetUserInfo(string key, string username, string password)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static void ValidateUser(string username, string password)
        {
            var validate = Membership.ValidateUser(username, password);
            if (!validate)
            {
                throw new XmlRpcException();
            }
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(username), null);
        }
    }
}