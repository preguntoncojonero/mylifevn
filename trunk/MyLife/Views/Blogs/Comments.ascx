<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Comment>>" %>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<div id="comments" class="post-comments">
    <h2>Có tất cả <%= Model.Count %> phản hồi trong bài viết này</h2>
    <ul>
    <% foreach(var comment in Model){ %>
        <li class="comment">
            <%= Html.Gravatar(comment.Email) %>
            <span class="post-comment-author">
                <%= Html.Link(comment.Name, comment.Website, new { rel = "external nofollow" })%>
                <span class="post-comment-date"><%= comment.CreatedDate.ToString(MyLifeContext.Settings.LongDateTimeFormat) %></span>
            </span>
            <p><%= comment.Content %></p>
        </li>
    <% } %>
    </ul>
</div>