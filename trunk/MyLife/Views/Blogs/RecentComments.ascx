<%@ Import Namespace="MyLife.Web.Blogs"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Comment>>" %>

<h2>Các phản hồi mới</h2>
<%= Html.UnorderedLinks<Comment>(Model, comment => string.Format("<b>{0}</b>: {1}", comment.Name, comment.Content.RemoveHtmlTags().Ellipsis(60)), comment => comment.ToUri())%>