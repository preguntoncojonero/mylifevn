<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<MyLifeProfile>" %>
<%@ Import Namespace="MyLife.Web.Security"%>
<%@ Import Namespace="MyLife.Web.Mvc"%>
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
        <% using (Html.BeginForm("profile", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fProfile" })){ %>
            <fieldset>
                <legend>Hồ sơ cá nhân</legend>
                <div>
                    <label for="profile_FullName">Họ và tên</label>
                    <%= Html.TextBox("profile.FullName", Model.FullName, new { maxlength = 255})%>
                </div>
                <div>
                    <label for="profile_Birthday">Ngày tháng năm sinh</label>
                    <%= Html.TextBox("profile.Birthday", Model.Birthday.ToString(MyLifeContext.Settings.ShortDateFormat))%>
                    <span class="notes">Định dạng: ngày/tháng/năm, ví dụ: 17/04/1987</span>
                </div>
                <div>
                    <label for="profile_Sex">Giới tính</label>
                    <%= Html.RadioButton("profile.Sex", true, Model.Sex) %> Con trai
                    <%= Html.RadioButton("profile.Sex", false, !Model.Sex) %> Con gái
                </div>
                <div>
                    <label for="profile_Address">Địa chỉ</label>
                    <%= Html.TextArea("profile.Address", Model.Address, new { maxlength = 255})%>
                </div>
                <div>
                    <label for="profile_City">Thành phố</label>
                    <%= Html.DropDownList("profile.City", (IEnumerable<SelectListItem>)ViewData["Cities"])%>
                </div>
                <div class="buttons">
                    <a class="button user" id="btnSave">Thay đổi</a>
                </div>
            </fieldset>
        <% } %>        
        <%= Html.ClientSideValidation<MyLifeProfile>("profile")%>        
    </div>    
    <br />
    <a href="/changepassword"><b>Thay đổi mật khẩu</b></a>
    
    <div id="divAjaxLoading" style="width: 200px">
        Đang kết nối với máy chủ<br />
        Nhấn <b style="color: Red">F5</b> nếu chờ quá lâu.
    </div>
    
    <script type="text/javascript">
        var controller;
        $(document).ready(function() {
            controller = new MyLifeController();

            new AjaxLoading("divAjaxLoading");

            $("#btnSave").click(function() {
                controller.updateProfile();
                return false;
            });
        });
    </script>
</asp:Content>
