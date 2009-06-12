<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Validation"%>
<%@ Import Namespace="MyLife"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script type="text/javascript" src="/Scripts/ControlsToolkit.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
    <% using (Html.BeginForm("register", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fRegister" })){%>
        <h2>Đăng ký tài khoản</h2>
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
        <div id="divMessageBox"></div>
    <% } %>    
    <%= Html.ClientSideValidation(null, Validates.GetRegisterRuleSet())%>
    </div>
    
    <script type="text/javascript">
        var msg;
        $(document).ready(function() {
            msg = new MessageBox("divMessageBox");

            $("#btnRegister").click(function() {
                register();
                return false;
            });

            msg.showInfo("Bạn hãy nhập các thông tin cần thiết để đăng ký");
        });

        function register() {
            var self = $("#fRegister");
            if (!self.valid()) {
                return false;
            }
            $("#fRegister div.buttons").hide();
            msg.showWait("Đang kết nối tới máy chủ, bạn hãy đợi trong giây lát");
            var action = self.attr('action');
            var data = self.serialize();
            $.post(action, data, function(result) {
                if (result.Status) {
                    msg.showInfo(result.Message);
                    window.location = result.RedirectUrl;
                } else {
                    $("#fRegister div.buttons").show();
                    msg.showError(result.Message);
                }
            }, "json");
        }
    </script>
</asp:Content>
