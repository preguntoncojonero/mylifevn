<%@ Page Language="C#" MasterPageFile="dfBlog.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IList<Post>>" %>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <% foreach(var post in Model){ %>
        <% Html.RenderPartial("PostDetails", post); %>
    <% } %>
    <%= Html.PageNavigator((string)ViewData[Constants.ViewData.PageNavigator.BaseUrl], (int)ViewData[Constants.ViewData.PageNavigator.IndexOfPage],(int)ViewData[Constants.ViewData.PageNavigator.TotalPages]) %>
</asp:Content>