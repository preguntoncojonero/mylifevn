<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" Inherits="System.Web.Mvc.ViewPage" %>
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
    <% using (Html.BeginForm("contact", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fContact" })){ %>
        <fieldset>
            <legend>Liên hệ</legend>
            <div>
                Cảm ơn bạn đã gửi thông tin đến cho chúng tôi. Mọi đóng góp ý kiến của bạn sẽ tham khảo giúp cho <strong>MyLife</strong> ngày càng phát triển hơn.<br />
                Bạn có thể điền các thông tin dưới đây hoặc là gửi email về địa chỉ <strong>Nguyen.dainghia@gmail.com</strong>
            </div>
            <div>
                <label for="contact_Name">Họ và tên của bạn</label>
                <input type="text" maxlength="50" name="Name" id="contact_Name" />
            </div>
            <div>
                <label for="contact_Email">Địa chỉ email</label>
                <input type="text" maxlength="255" name="Email" id="contact_Email" />
            </div>
            <div>
                <label for="contact_Content">Nội dung phản hồi</label>
                <textarea cols="60" rows="10" name="Content" id="contact_Content"></textarea>
            </div>
            <div class="buttons">
                <a id="btnSubmit" class="button send-mail">Phản hồi</a>
            </div>
        </fieldset>
    <% } %>
    <%= Html.ClientSideValidation("contact", Validates.GetContactRuleSet()) %>
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

            $("#btnSubmit").click(function() {
                controller.sendContact();
                return false;
            });
        });
    </script>
</asp:Content>