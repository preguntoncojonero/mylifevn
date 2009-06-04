<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MyLife.master" AutoEventWireup="true" CodeBehind="MyLifeSettings.aspx.cs" Inherits="MyLife.Views.MyLife.MyLifeSettings" %>
<%@ Import Namespace="MyLife"%>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="divHead" runat="server">
</asp:Content>

<asp:Content ID="divDetailsContent" ContentPlaceHolderID="divDetails" runat="server">
    <% var user = MyLifeContext.Current.Site; %>
    <table style="width: 100%;" cellpadding="15" cellspacing="0" class="mylife-settings">
        <tr>            
            <td style="width: 50%">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/<%= user %>/blog/start"><img src="/Content/Images/icon-blog.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a>Cấu hình hệ thống</a></h2>
                            <p>Quản lý các cấu hình của hệ thống</p>
                            <ul>
                                <li><a href="/settings/themes">Giao diện người dùng</a></li>
                            </ul>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 50%; vertical-align: top">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top"><a href="/settings/users"><img src="/Content/Images/icon-users.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/settings/users">Quản lý thành viên</a></h2>
                            <p>Quản lý thông tin các thành viên</p>                        
                            <ul>
                                <li><a href="/settings/sendmail">Gửi email cho mọi người</a></li>
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
                        <td style="vertical-align: top"><a href="/<%= user %>/sitemaps"><img src="/Content/Images/icon-sitemap.png" width="64" height="64" alt="" /></a></td>
                        <td style="vertical-align: top">
                            <h2><a href="/<%= user %>/sitemaps">Bản đồ trang web</a></h2>
                            <p>Bản đồ trang web của bạn</p>
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
