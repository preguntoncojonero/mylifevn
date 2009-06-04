<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MyLife.master" AutoEventWireup="true"
    CodeBehind="Settings.aspx.cs" Inherits="MyLife.Views.MyLife.Settings" %>
<%@ Import Namespace="MyLife"%>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="divHead" runat="server">
</asp:Content>

<asp:Content ID="divDetailsContent" ContentPlaceHolderID="divDetails" runat="server">
    <% var user = MyLifeContext.Current.Site; %>
    <table style="width: 100%;" cellpadding="15" cellspacing="0" class="mylife-settings">
        <tr>
            <td style="width: 50%; vertical-align: top">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/profile"><img src="/Content/Images/icon-profile.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/settings/profile">Hồ sơ cá nhân</a></h2>
                            <p>Tùy chỉnh các thông tin cá nhân của bạn</p>                        
                            <ul>
                                <li><a href="/avatar">Chỉnh sửa avatar</a></li>                                
                            </ul>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 50%">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/blog/start"><img src="/Content/Images/icon-blog.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/blog/start">Blog</a></h2>
                            <p>Chia sẻ những cảm xúc của bạn với mọi người</p>                        
                            <ul>
                                <li><a href="/<%= user %>/blog/settings">Thiết lập blog</a></li>
                                <li><a href="/<%= user %>/blog/addpost">Thêm bài viết</a></li>
                                <li><a href="/<%= user %>/blog/categories">Quản lý các chủ đề</a></li>
                                <li><a href="/<%= user %>/blog/blogrolls">Quản lý các blogroll</a></li>
                                <li><a href="/<%= user %>/blog/comments">Quản lý các phản hồi</a></li>
                            </ul>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/friends/start"><img src="/Content/Images/icon-friends.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/friends/start">Friends</a></h2>
                            <p>Những người bạn sẽ cho ta những niềm vui</p>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/photos/start"><img src="/Content/Images/icon-photos.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/photos/start">Photos</a></h2>
                            <p>Những khoảnh khắc của cuộc sống quanh ta</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>        
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/events/start"><img src="/Content/Images/icon-events.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/events/start">Events</a></h2>
                            <p></p>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/changepassword"><img src="/Content/Images/icon-password.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/changepassword">Mật khẩu</a></h2>
                            <p>Thay đổi mật khẩu của bạn</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/links"><img src="/Content/Images/icon-sitemap.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/links">Liên kết trang web</a></h2>
                            <p>Các liên kết trong trang web của bạn</p>
                        </td>
                    </tr>
                </table>
            </td>
            <td></td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="divSideBarContent" ContentPlaceHolderID="divSideBar" runat="server">
</asp:Content>
