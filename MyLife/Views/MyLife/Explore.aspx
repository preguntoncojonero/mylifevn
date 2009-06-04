<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" Inherits="System.Web.Mvc.ViewPage<MembershipUserCollection>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Các thành viên của MyLife</h2>
    <ul class="explore">
    <% foreach (MembershipUser user in Model){ %>
         <li>
            <%= Html.Avatar(user.UserName) %>
            <p>
                <span><%= user.UserName %></span>
                <span>Gia nhập: <%= user.CreationDate.ToString(MyLifeContext.Settings.ShortDateFormat) %></span>
                <span><a href="/<%= user.UserName %>/blog"><%= user.UserName %>'s blog</a></span>
            </p>            
         </li>
    <% } %>
    </ul>
    
    <%= Html.PageNavigator((string)ViewData[Constants.ViewData.PageNavigator.BaseUrl], (int)ViewData[Constants.ViewData.PageNavigator.IndexOfPage],(int)ViewData[Constants.ViewData.PageNavigator.TotalPages]) %>
</asp:Content>