<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Validation"%>
<%@ Import Namespace="MyLife"%>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script src="/Scripts/Controllers/BaseController.js" type="text/javascript"></script>
    <script src="/Scripts/Controllers/MyLifeController.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script src="/Scripts/ControlsToolkit.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <form action="/changepassword" method="post" id="fChangePassword">
            <fieldset>
                <legend>Thay đổi mật khẩu</legend>
                <div>
                    <label for="OldPassword">Mật khẩu cũ</label>
                    <%= Html.Password("OldPassword", "", new { autocomplete = "off" })%>
                </div>
                <div>
                    <label for="NewPassword">Mật khẩu mới</label>
                    <%= Html.Password("NewPassword", "", new { autocomplete = "off" })%>
                </div>
                <div>
                    <label for="ConfirmNewPassword">Xác nhận mật khẩu</label>
                    <%= Html.Password("ConfirmNewPassword", "", new { autocomplete = "off" })%>
                </div>
                <div class="buttons">
                    <%= Html.MyLifeAntiForgeryToken() %>
                    <a class="button user" id="btnChangePassword">Thay đổi mật khẩu</a>
                </div>
            </fieldset>
        </form>
        <%= Html.ClientSideValidation(null, Validates.GetChangePasswordRuleSet())%>
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

            $("#btnChangePassword").click(function() {
                controller.changePassword();
                return false;
            });
        });
    </script>
</asp:Content>