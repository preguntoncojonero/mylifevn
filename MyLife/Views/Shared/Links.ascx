<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IPrincipal>" %>
<%@ Import Namespace="System.Security.Principal"%>

<h2 class="bg1">my life</h2>
<ul>
    <li><a href="/">Trang chủ</a></li>
    <% if(Model.Identity.IsAuthenticated){ %>
        <li><%= Html.Link("Trang nhà của bạn", "/" + Model.Identity.Name) %></li>
        <li><a href="/logout">Đăng xuất</a></li>
    <% } %>
    <% else { %>
        <li><a href="/login">Đăng nhập</a></li>
        <li><a href="/register">Đăng ký mới</a></li>
    <% } %>
    <li><a href="/explore">Các thành viên</a></li>
    <li><a href="/contact">Liên hệ</a></li>
</ul>