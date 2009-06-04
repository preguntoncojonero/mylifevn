using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.Blogs
{
    public abstract class BlogsProvider: ProviderBase
    {
        #region Blogs

        public abstract int InsertBlog(Blog blog);
        public abstract Blog GetBlogById(int id);
        public abstract Blog GetBlogByName(string name);
        public abstract void UpdateBlog(Blog blog);
        public abstract void DeleteBlog(int id);
        public abstract IList<Blog> GetNewesUsers(int numberOfUsers);

        #endregion

        #region Posts

        public abstract int InsertPost(Post post);
        public abstract Post GetPostById(int id);
        public abstract Post GetPostBySlug(int blogId, string slug);
        public abstract List<Post> GetPostsOfBlog(int blogId, PostOptions option);
        public abstract List<Post> GetPostsOfBlog(int blogId, PostOptions option, int indexOfPage, int sizeOfPage, out int total);
        public abstract List<Post> GetPostsOfCategory(int categoryId, PostOptions option);
        public abstract List<Post> GetPostsOfCategory(int categoryId, PostOptions option, int indexOfPage, int sizeOfPage, out int total);
        public abstract List<Post> GetRecentPosts(int numberOfPosts);
        public abstract List<Post> GetRecentPosts(int blogId, int numberOfPosts);
        public abstract IList<Post> GetAllPosts(int blogId);
        public abstract IList<Post> Search(int blogId, string keyword);
        public abstract void UpdatePost(Post post);
        public abstract void DeletePost(int id);

        #endregion

        #region Categories

        public abstract int InsertCategory(Category category);
        public abstract Category GetCategoryById(int id);
        public abstract Category GetCategoryBySlug(int blogId, string slug);
        public abstract List<Category> GetCategoriesOfBlog(int blogId);
        public abstract List<Category> GetCategoriesOfPost(int postId);
        public abstract void UpdateCategory(Category category);
        public abstract void DeleteCategory(int id);

        #endregion

        #region Comments

        public abstract int InsertComment(Comment comment);
        public abstract Comment GetCommentById(int id);
        public abstract List<Comment> GetCommentsOfBlog(int blogId);
        public abstract List<Comment> GetCommentsOfBlog(int blogId, int indexOfPage, int sizeOfPage, out int total);
        public abstract List<Comment> GetCommentsOfPost(int postId);
        public abstract List<Comment> GetCommentsOfPost(int postId, int indexOfPage, int sizeOfPage, out int total);
        public abstract List<Comment> GetRecentComments(int blogId, int numberOfComments);
        public abstract void UpdateComment(Comment comment);
        public abstract void DeleteComment(int id);

        #endregion

        #region Blogrolls

        public abstract int InsertBlogroll(Blogroll blogroll);
        public abstract Blogroll GetBlogrollById(int id);
        public abstract List<Blogroll> GetBlogrolls(int blogId);
        public abstract void UpdateBlogroll(Blogroll blogroll);
        public abstract void DeleteBlogroll(int id);

        #endregion
    }
}