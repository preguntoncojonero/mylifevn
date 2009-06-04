<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<div class="blog-tools">
    <h2>blog của bạn</h2>
    <p>Bạn có thể quản lý blog của bạn tại đây</p>
    <ul>
        <li><%= Html.Link("Xem blog của bạn", string.Format("/{0}/blog", Model), new {target = "_blank"}) %></li>
        <li><%= Html.Link("Thiết lập blog", string.Format("/{0}/blog/settings", Model)) %></li>
        <li><%= Html.Link("Thêm bài viết mới", string.Format("/{0}/blog/addpost", Model)) %></li>
        <li><%= Html.Link("Chuyển đổi từ blog khác", string.Format("/{0}/blog/importrss", Model)) %></li>
    </ul>
</div>