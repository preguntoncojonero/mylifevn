<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Validation"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/Controllers/BaseController.js" type="text/javascript"></script>
    <script src="/Scripts/Controllers/MyLifeController.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script src="/Scripts/ControlsToolkit.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using(Html.BeginForm("resetpassword", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fResetPassword" })){%>
            <h2>Tìm lại mật khẩu</h2>
            <div>
                <label for="user_UserName">Tên đăng nhập<span class="required"> *</span></label>
                <input type="text" id="user_UserName" name="user.UserName" maxlength="255" />
            </div>
            <div class="buttons">
                <%= Html.MyLifeAntiForgeryToken() %>
                <a class="button mini-icons user" href="javascript:void(0)" id="btnResetPassword">Tìm lại mật khẩu</a>
            </div>
            <div id="divMessageBox"></div>
            <%= Html.ClientSideValidation("user", Validates.GetResetPasswordRuleSet())%>
        <% } %>
    </div>
    
    <script type="text/javascript">
        var msg;

        $(document).ready(function() {
            msg = new MessageBox("divMessageBox");
            msg.showInfo("Bạn hãy nhập tên đăng nhập để tìm lại mật khẩu");

            $("#user_UserName").focus();

            $("#btnResetPassword").click(function() {
                reset();
            });

            $("#fResetPassword").submit(function() {
                reset();
                return false;
            });
        });

        function reset() {
            var self = $("#fResetPassword");
            if (!self.validate().form()) {
                return false;
            }
            msg.showWait("Đang kết nối tới máy chủ, bạn hãy đợi trong giây lát");
            var action = self.attr("action");
            var data = self.serialize();
            $.post(action, data, function(result) {
                if (result.Status) {
                    msg.showInfo(result.Message);
                    window.location = result.RedirectUrl;
                } else {
                    msg.showError(result.Message);
                }
            }, "json");
        }
    </script>
</asp:Content>
