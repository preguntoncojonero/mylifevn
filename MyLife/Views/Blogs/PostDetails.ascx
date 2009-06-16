<%@ Import Namespace="MyLife.Web.Blogs"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Post>" %>

<div class="post">
    <h2><%= Html.Link(Model.Title, Model.RelativeUrl, new { rel = "bookmark", title = Model.Title })%></h2>
    <div class="post-meta">
        <span class="date"><%= Model.CreatedDate.ToString(MyLifeContext.Settings.LongDateFormat)%></span>
    </div>
    <div class="post-content">
        <%= Model.Content %>
    </div>
    
    <div class="post-meta">
        <span class="post-categories">
            <%= Model.Categories.FormatAsLinks(item => item.Name, item => item.RelativeUrl, ", ") %>
        </span>
    </div>
    
    <asp:TextBox runat="server"></asp:TextBox>
</div>