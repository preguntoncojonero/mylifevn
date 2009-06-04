<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<News>>" %>
<%@ Import Namespace="MyLife.Web.News"%>

<h2>Tin tức mới</h2>
<ul>
    <% foreach (var news in Model) { %>
        <li><%= news.CreatedDate.ToString("dd/MM") %>&nbsp;<%= Html.Link(news.Title, news.ToUri()) %></li>
    <% } %>
</ul>