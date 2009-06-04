<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Validation"%>
<%@ Import Namespace="MyLife"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Controllers/BaseController.js"></script>
    <script type="text/javascript" src="/Scripts/Controllers/MyLifeController.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script type="text/javascript" src="/Scripts/ControlsToolkit.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
    <% using (Html.BeginForm("register", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fRegister" })){%>
        <fieldset>
            <legend>Đăng ký tài khoản</legend>
            <div>
                <label for="UserName">Tên đăng nhập</label>
                <%= Html.TextBox("UserName", "", new { autocomplete = "off" })%>
            </div>
            <div>
                <label for="Password">Mật khẩu</label>
                <%= Html.Password("Password", "", new { autocomplete = "off" })%>
            </div>
            <div>
                <label for="ConfirmPassword">Xác nhận mật khẩu</label>
                <%= Html.Password("ConfirmPassword", "", new { autocomplete = "off" })%>
            </div>
            <div>
                <label for="Email">Email</label>
                <%= Html.TextBox("Email", "", new { autocomplete = "off" })%>
            </div>
            <div class="buttons">
                <%= Html.MyLifeAntiForgeryToken() %>
                <a class="button user" id="btnRegister">Đăng ký tài khoản</a>
            </div>
        </fieldset>
    <% } %>    
    <%= Html.ClientSideValidation(null, Validates.GetRegisterRuleSet())%>
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

            $("#btnRegister").click(function() {
                controller.register();
                return false;
            });
        });
    </script>
</asp:Content>
