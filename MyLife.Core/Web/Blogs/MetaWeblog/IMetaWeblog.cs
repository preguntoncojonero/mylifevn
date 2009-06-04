using CookComputing.XmlRpc;

namespace MyLife.Web.Blogs.MetaWeblog
{
    public interface IMetaWeblog
    {
        [XmlRpcMethod("metaWeblog.editPost",
            Description = "Updates and existing post to a designated blog using the metaWeblog API. Returns true if completed.")]
        bool editPost(string postid, string username, string password, XmlRpcStruct rpcstruct, bool publish);

        [XmlRpcMethod("metaWeblog.getCategories",
            Description = "Retrieves a list of valid categories for a blog using the metaWeblog API. Returns the metaWeblog categories struct collection.")]
        XmlRpcStruct[] getCategories(string blogid, string username, string password);

        [XmlRpcMethod("metaWeblog.getPost",
            Description = "Retrieves an existing post using the metaWeblog API. Returns the metaWeblog struct.")]
        XmlRpcStruct getPost(string postid, string username, string password);

        [XmlRpcMethod("metaWeblog.getRecentPosts",
            Description = "Retrieves a list of the most recent existing post using the metaWeblog API. Returns the metaWeblog struct collection.")]
        XmlRpcStruct[] getRecentPosts(string blogid, string username, string password, int numberOfPosts);

        [XmlRpcMethod("metaWeblog.newPost",
            Description = "Makes a new post to a designated blog using the metaWeblog API. Returns postid as a string.")]
        string newPost(string blogid, string username, string password, XmlRpcStruct post, bool publish);

        [XmlRpcMethod("metaWeblog.newMediaObject",
            Description = "Makes a new file to a designated blog using the metaWeblog API. Returns url as a string of a struct.")]
        XmlRpcStruct newMediaObject(string blogid, string username, string password, XmlRpcStruct rpcstruct);

        [XmlRpcMethod("blogger.deletePost")]
        [return: XmlRpcReturnValue(Description = "Returns true.")]
        bool DeletePost(string key, string postid, string username, string password, bool publish);

        [XmlRpcMethod("blogger.getUsersBlogs")]
        XmlRpcStruct[] GetUsersBlogs(string key, string username, string password);

        [XmlRpcMethod("blogger.getUserInfo")]
        XmlRpcStruct GetUserInfo(string key, string username, string password);
    }
}