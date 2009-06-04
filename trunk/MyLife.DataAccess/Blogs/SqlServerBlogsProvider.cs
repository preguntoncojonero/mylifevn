using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using MyLife.Web;
using MyLife.Web.Blogs;

namespace MyLife.DataAccess.Blogs
{
    public class SqlServerBlogsProvider : BlogsProvider
    {
        public override int InsertBlog(Blog blog)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Blogs();
            BizObject<Blog, int>.CopyToObject(blog, obj);
            context.AddTotblBlogs_Blogs(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override Blog GetBlogById(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Blogs.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        private static Blog Convert(tblBlogs_Blogs obj)
        {
            if (obj == null)
            {
                return null;
            }

            var blog = new Blog(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Blog, int>.CopyFromObject(blog, obj);
            return blog;
        }

        public override Blog GetBlogByName(string name)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Blogs.Where(item => item.CreatedBy == name).FirstOrDefault();
            return Convert(obj);
        }

        public override void UpdateBlog(Blog blog)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Blogs {Id = blog.Id};
            context.AttachTo("tblBlogs_Blogs", obj);
            blog.CopyToObject(obj);
            context.SaveChanges();
        }

        public override void DeleteBlog(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Blogs.Where(item => item.Id == id).FirstOrDefault();
            if (obj != null)
            {
                context.DeleteObject(obj);
                context.SaveChanges();
            }
        }

        public override int InsertPost(Post post)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Posts();
            post.CopyToObject(obj);

            if (post.Categories != null)
            {
                foreach (var category in post.Categories)
                {
                    var item = new tblBlogs_Categories {Id = category.Id};
                    context.AttachTo("tblBlogs_Categories", item);
                    obj.Categories.Add(item);
                }
            }

            context.AddTotblBlogs_Posts(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override Post GetPostById(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Posts.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        public override Post GetPostBySlug(int blogId, string slug)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Slug == slug).FirstOrDefault();
            return Convert(obj);
        }

        public override List<Post> GetPostsOfBlog(int blogId, PostOptions option)
        {
            var context = new BlogsEntities();
            List<tblBlogs_Posts> list;
            switch (option)
            {
                case PostOptions.Published:
                    list =
                        context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Published).OrderByDescending(
                            item => item.CreatedDate).ToList();
                    break;
                case PostOptions.NotPublished:
                    list =
                        context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Published == false).
                            OrderByDescending(item => item.CreatedDate).ToList();
                    break;
                default:
                    list =
                        context.tblBlogs_Posts.Where(item => item.BlogId == blogId).OrderByDescending(
                            item => item.CreatedDate).ToList();
                    break;
            }
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Post> GetPostsOfBlog(int blogId, PostOptions option, int indexOfPage, int sizeOfPage,
                                                  out int total)
        {
            var context = new BlogsEntities();
            List<tblBlogs_Posts> list;
            switch (option)
            {
                case PostOptions.Published:
                    total = context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Published).Count();
                    list =
                        context.tblBlogs_Posts.Where(
                            item => item.BlogId == blogId && item.Published).OrderByDescending(
                            item => item.CreatedDate).Skip(indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
                    break;
                case PostOptions.NotPublished:
                    total =
                        context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Published == false).Count();
                    list =
                        context.tblBlogs_Posts.Where(
                            item => item.BlogId == blogId && item.Published == false).
                            OrderByDescending(
                            item => item.CreatedDate).Skip(indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
                    break;
                default:
                    total = context.tblBlogs_Posts.Where(item => item.BlogId == blogId).Count();
                    list =
                        context.tblBlogs_Posts.Where(item => item.BlogId == blogId).
                            OrderByDescending(
                            item => item.CreatedDate).Skip(indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
                    break;
            }
            return list.ConvertAll(obj => Convert(obj));
        }

        private static Post Convert(tblBlogs_Posts obj)
        {
            if (obj == null)
            {
                return null;
            }

            var post = new Post(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Post, int>.CopyFromObject(post, obj);
            if (obj.Categories != null)
            {
                post.Categories = new List<tblBlogs_Categories>(obj.Categories).ConvertAll(category => Convert(category));
            }
            return post;
        }

        public override List<Post> GetPostsOfCategory(int categoryId, PostOptions option)
        {
            var context = new BlogsEntities();
            List<tblBlogs_Posts> list;
            switch (option)
            {
                case PostOptions.Published:
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published).OrderByDescending(item => item.CreatedDate).ToList();
                    break;
                case PostOptions.NotPublished:
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published == false).OrderByDescending(item => item.CreatedDate).ToList();
                    break;
                default:
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            OrderByDescending(item => item.CreatedDate).ToList();
                    break;
            }
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Post> GetPostsOfCategory(int categoryId, PostOptions option, int indexOfPage,
                                                      int sizeOfPage, out int total)
        {
            var context = new BlogsEntities();
            List<tblBlogs_Posts> list;
            switch (option)
            {
                case PostOptions.Published:
                    total =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published).Count();
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published).OrderByDescending(item => item.CreatedDate).Skip(indexOfPage*
                                                                                                           sizeOfPage).
                            Take(sizeOfPage).ToList();
                    break;
                case PostOptions.NotPublished:
                    total =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published == false).Count();
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Where(item => item.Published == false).OrderByDescending(item => item.CreatedDate).Skip(
                            indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
                    break;
                default:
                    total =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            Count();
                    list =
                        context.tblBlogs_Categories.Where(item => item.Id == categoryId).SelectMany(item => item.Posts).
                            OrderByDescending(item => item.CreatedDate).
                            Skip(indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
                    break;
            }
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Post> GetRecentPosts(int numberOfPosts)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Posts.Where(item => item.Published).OrderByDescending(
                    item => item.CreatedDate).Skip(0).Take(numberOfPosts).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Post> GetRecentPosts(int blogId, int numberOfPosts)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Posts.Where(item => item.BlogId == blogId && item.Published).OrderByDescending(
                    item => item.CreatedDate).Skip(0).Take(numberOfPosts).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override void UpdatePost(Post post)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Posts.Include("Categories").Where(item => item.Id == post.Id).FirstOrDefault();
            post.CopyToObject(obj);

            obj.Categories.Clear();
            if (post.Categories.Count > 0)
            {
                var categories =
                    context.tblBlogs_Categories.Where(
                        Utils.BuildContainsExpression<tblBlogs_Categories, int>(item => item.Id,
                                                                                post.Categories.Select(item => item.Id)));
                foreach (var category in categories)
                {
                    obj.Categories.Add(category);
                }
            }

            context.SaveChanges();
        }

        public override void DeletePost(int id)
        {
            var context = new BlogsEntities();
            using (var transaction = new TransactionScope())
            {
                var post = context.tblBlogs_Posts.Include("Categories").Where(item => item.Id == id).FirstOrDefault();
                post.Categories.Clear();
                context.SaveChanges();
                var comments = context.tblBlogs_Comments.Where(item => item.PostId == post.Id);
                foreach (var comment in comments)
                {
                    context.DeleteObject(comment);
                }

                context.DeleteObject(post);
                context.SaveChanges();
                transaction.Complete();
            }
        }

        public override int InsertCategory(Category category)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Categories();
            BizObject<Category, int>.CopyToObject(category, obj);
            context.AddTotblBlogs_Categories(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override Category GetCategoryById(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Categories.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        public override Category GetCategoryBySlug(int blogId, string slug)
        {
            var context = new BlogsEntities();
            var obj =
                context.tblBlogs_Categories.Where(item => item.BlogId == blogId && item.Slug == slug).FirstOrDefault();
            return Convert(obj);
        }

        public override List<Category> GetCategoriesOfBlog(int blogId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Categories.Where(item => item.BlogId == blogId).OrderBy(item => item.Name).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        private static Category Convert(tblBlogs_Categories obj)
        {
            if (obj == null)
            {
                return null;
            }
            var category = new Category(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Category, int>.CopyFromObject(category, obj);
            return category;
        }

        public override List<Category> GetCategoriesOfPost(int postId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Posts.Where(item => item.Id == postId).SelectMany(item => item.Categories).OrderBy(
                    item => item.Name).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override void UpdateCategory(Category category)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Categories {Id = category.Id};
            context.AttachTo("tblBlogs_Categories", obj);
            BizObject<Category, int>.CopyToObject(category, obj);
            context.SaveChanges();
        }

        public override void DeleteCategory(int id)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Categories {Id = id};
            context.AttachTo("tblBlogs_Categories", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override int InsertComment(Comment comment)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Comments();
            BizObject<Comment, int>.CopyToObject(comment, obj);
            context.AddTotblBlogs_Comments(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override Comment GetCommentById(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Comments.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        public override List<Comment> GetCommentsOfBlog(int blogId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Comments.Where(item => item.BlogId == blogId).OrderBy(item => item.CreatedDate).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Comment> GetCommentsOfBlog(int blogId, int indexOfPage, int sizeOfPage, out int total)
        {
            var context = new BlogsEntities();
            indexOfPage--;
            total = context.tblBlogs_Comments.Where(item => item.BlogId == blogId).Count();
            var list =
                context.tblBlogs_Comments.Where(item => item.BlogId == blogId).OrderBy(item => item.CreatedDate).Skip(
                    indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Comment> GetCommentsOfPost(int postId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Comments.Where(item => item.PostId == postId && item.IsApproved).OrderBy(
                    item => item.CreatedDate).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Comment> GetCommentsOfPost(int postId, int indexOfPage, int sizeOfPage, out int total)
        {
            var context = new BlogsEntities();
            indexOfPage--;
            total = context.tblBlogs_Comments.Where(item => item.PostId == postId).Count();
            var list =
                context.tblBlogs_Comments.Where(item => item.PostId == postId && item.IsApproved).OrderBy(
                    item => item.CreatedDate).Skip(
                    indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override List<Comment> GetRecentComments(int blogId, int numberOfComments)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Comments.Where(item => item.BlogId == blogId && item.IsApproved).OrderByDescending(
                    item => item.CreatedDate).Skip(0).Take(numberOfComments).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        private static Comment Convert(tblBlogs_Comments obj)
        {
            if (obj == null)
            {
                return null;
            }
            var comment = new Comment(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Comment, int>.CopyFromObject(comment, obj);
            return comment;
        }

        public override void UpdateComment(Comment comment)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Comments {Id = comment.Id};
            context.AttachTo("tblBlogs_Comments", obj);
            BizObject<Comment, int>.CopyToObject(comment, obj);
            context.SaveChanges();
        }

        public override void DeleteComment(int id)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Comments {Id = id};
            context.AttachTo("tblBlogs_Comments", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override int InsertBlogroll(Blogroll blogroll)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Blogrolls();
            BizObject<Blogroll, int>.CopyToObject(blogroll, obj);
            context.AddTotblBlogs_Blogrolls(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override Blogroll GetBlogrollById(int id)
        {
            var context = new BlogsEntities();
            var obj = context.tblBlogs_Blogrolls.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        public override List<Blogroll> GetBlogrolls(int blogId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Blogrolls.Where(item => item.BlogId == blogId).OrderBy(item => item.Name).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        private static Blogroll Convert(tblBlogs_Blogrolls obj)
        {
            if (obj == null)
            {
                return null;
            }
            var blogroll = new Blogroll(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Blogroll, int>.CopyFromObject(blogroll, obj);
            return blogroll;
        }

        public override void UpdateBlogroll(Blogroll blogroll)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Blogrolls {Id = blogroll.Id};
            context.AttachTo("tblBlogs_Blogrolls", obj);
            BizObject<Blogroll, int>.CopyToObject(blogroll, obj);
            context.SaveChanges();
        }

        public override void DeleteBlogroll(int id)
        {
            var context = new BlogsEntities();
            var obj = new tblBlogs_Blogrolls {Id = id};
            context.AttachTo("tblBlogs_Blogrolls", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override IList<Post> GetAllPosts(int blogId)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Posts.Where(item => item.BlogId == blogId).OrderByDescending(item => item.CreatedDate).
                    ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override IList<Blog> GetNewesUsers(int numberOfUsers)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Blogs.OrderByDescending(item => item.CreatedDate).Skip(0).Take(numberOfUsers).ToList();
            return list.ConvertAll(obj => Convert(obj));
        }

        public override IList<Post> Search(int blogId, string keyword)
        {
            var context = new BlogsEntities();
            var list =
                context.tblBlogs_Posts.Where(
                    item => item.BlogId == blogId && (item.Title.Contains(keyword) || item.Content.Contains(keyword))).
                    ToList();
            return list.ConvertAll(obj => Convert(obj));
        }
    }
}