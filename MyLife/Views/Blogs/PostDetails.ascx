<%@ Import Namespace="MyLife.Web.Blogs"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Post>" %>

<div class="post">
    <h2><%= Html.Link(Model.Title, Model.ToUri(), new { rel = "bookmark", title = Model.Title })%></h2>
    <div class="post-meta">
        <span class="date"><%= Model.CreatedDate.ToString(MyLifeContext.Settings.LongDateFormat)%></span>
    </div>
    <div class="post-content">
        <%= Html.If((bool)ViewData[Constants.ViewData.Blogs.IsPostList], html => Model.ShortContent)
        .Else(html => Model.Content) %>
    </div>
    
    <% if(!(bool) ViewData[Constants.ViewData.Blogs.IsPostList]){ %>
        <div class="post-meta">
            <span class="post-categories">
                <%= Model.Categories.FormatAsLinks(item => item.Name, item => item.ToUri(), ", ") %>
            </span>
        </div>
        <% Html.RenderPartial("Comments", Model.Comments); %>
        <% Html.RenderPartialIf(ViewData[Constants.ViewData.Blogs.Comment] != null, "AddComment", ViewData[Constants.ViewData.Blogs.Comment]); %>
    <% } %>    
</div>