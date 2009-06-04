<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<MyLife.Web.Blogs.Blog>>" %>

<h2>Thành viên mới</h2>
<p>Chào mừng các bạn đã tham gia <strong>MyLife</strong>, hi vọng các bạn có những giây phút vui vẻ trong không gian này</p>
<ul>
    <% foreach (var blog in Model) { %>
        <li>
            <%= Html.Avatar(blog.CreatedBy)%>
            <p>
                <%= blog.CreatedBy %>
                <span><%= blog.CreatedDate.ToString(MyLifeContext.Settings.ShortDateFormat) %></span>
                <span><a href="/<%= blog.CreatedBy %>/blog"><%= blog.CreatedBy %>'s blog</a></span>
            </p>
        </li>        
    <% } %>
</ul>