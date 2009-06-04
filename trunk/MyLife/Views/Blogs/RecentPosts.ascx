<%@ Import Namespace="MyLife.Web.Blogs"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Post>>" %>

<h2>Các bài viết mới</h2>
<%= Html.UnorderedLinks<Post>(Model, t => t.Title, l => l.ToUri()) %>