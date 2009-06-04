<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<MyLife.Web.Blogs.Post>>" %>

<h2>Các bài viết mới</h2>
<p>Các bài viết mới nhất của các thành viên trong <strong>MyLife</strong>, các bạn hãy thử ghé qua đọc các bài viết này và biết đâu bạn có thể tìm thấy điều bạn cần</p>
<ul>
    <% foreach (var post in Model) { %>
        <li><%= Html.Link(string.Format("{0}'s blog: {1}", post.CreatedBy, post.Title), post.ToUri()) %></li>
    <% } %>
</ul>