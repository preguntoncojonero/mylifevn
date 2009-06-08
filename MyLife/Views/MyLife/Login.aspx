<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Web.Mvc"%>
<%@ Import Namespace="MyLife.Validation"%>
<%@ Import Namespace="MyLife"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using (Html.BeginForm("login", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fLogin" })){%>
            <h2>Đăng nhập hệ thống</h2>
            <div>
                <label for="UserName">Tên đăng nhập</label>
                <%= Html.TextBox("UserName", "", new { maxlength = 255 })%>
            </div>
            <div>
                <label for="Password">Mật khẩu</label>
                <%= Html.Password("Password", "", new { autocomplete = "off", maxlength = 255 })%>
            </div>
            <div class="buttons">
                <%= Html.MyLifeAntiForgeryToken() %>
                <%= Html.Hidden("ReturnUrl", Request.QueryString["ReturnUrl"])%>
                <a class="button user" id="btnLogin">Đăng nhập</a>
            </div>
            <div>
                <a href="/resetpassword">Bạn quên mật khẩu?</a><br />
                <a href="/register">Bạn chưa đăng ký tài khoản?</a>
            </div>
        <% } %>
        <%= Html.ClientSideValidation(null, Validates.GetLoginRuleSet())%>
    </div>
    
    <script type="text/javascript">
        var controller;
        $(document).ready(function() {
            $("#btnLogin").click(function() {
                login();
                return false;
            });

            $("#Password").keydown(function(event) {
                if (event.keyCode == 13) {
                    login();
                }
            });
        });

        function login() {
            var self = $("#fLogin");
            if (!self.valid()) {
                return false;
            }
            var action = self.attr('action');
            var data = self.serialize();
            $.post(action, data, function(result) {
                if (result.Status) {
                    window.location = result.RedirectUrl;
                } else {
                    alert(result.Message);
                }
            }, "json");        
        }
    </script>
</asp:Content>
