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
            <fieldset>
                <legend>Tìm lại mật khẩu</legend>
                <div>
                    <label for="user_UserName">Tên đăng nhập<span class="required"> *</span></label>
                    <input type="text" id="user_UserName" name="user.UserName" maxlength="255" />
                </div>
                <div class="buttons">
                    <a class="button user" id="btnResetPassword">Tìm lại mật khẩu</a>
                </div>
            </fieldset>
            <%= Html.ClientSideValidation("user", Validates.GetResetPasswordRuleSet())%>
        <% } %>
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

            $("#btnResetPassword").click(function() {
                controller.resetPassword();
                return false;
            });
        });
    </script>
</asp:Content>
