<%@ Page Language="C#" MasterPageFile="dfBlog.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<Post>" %>
<%@ Import Namespace="MyLife.Web.Blogs"%>
<%@ Import Namespace="MyLife.Web.Plugins.Emoticons"%>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="HeadContent" runat="server">    
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("PostDetails", Model); %>
</asp:Content>