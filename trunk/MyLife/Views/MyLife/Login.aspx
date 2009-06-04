<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Web.Mvc"%>
<%@ Import Namespace="MyLife.Validation"%>
<%@ Import Namespace="MyLife"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/Controllers/BaseController.js" type="text/javascript"></script>
    <script src="/Scripts/Controllers/MyLifeController.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script src="/Scripts/ControlsToolkit.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using (Html.BeginForm("login", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fLogin" })){%>
            <fieldset>
                <legend>Đăng nhập hệ thống</legend>
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
            </fieldset>
        <% } %>
        <%= Html.ClientSideValidation(null, Validates.GetLoginRuleSet())%>
    </div>
    
    <div id="divAjaxLoading" style="width: 200px">
        Đang kết nối với máy chủ<br />
        Nhấn <b style="color: Red">F5</b> nếu chờ quá lâu.
    </div>
    
    <script type="text/javascript">
        var controller;
        $(document).ready(function() {
            controller = new MyLifeController();

            new AjaxLoading("divAjaxLoading");

            $("#btnLogin").click(function() {
                controller.login();
                return false;
            });

            $("#Password").keydown(function(event) {
                if (event.keyCode == 13) {
                    controller.login();
                }
            });
        });
    </script>
</asp:Content>
