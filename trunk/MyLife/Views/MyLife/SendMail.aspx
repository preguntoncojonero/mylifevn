<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<string>" %>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.metadata.pack.js"></script>
</asp:Content>

<asp:Content ID="divDetailsContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using(Html.BeginForm("sendmail", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fSendMail" })){ %>
            <fieldset>
                <legend>Gửi email cho các thành viên</legend>
                <p>
                    <label for="Subject">Chủ đề<span class="required"> *</span></label>
                    <%= Html.TextBox("Subject", "", new { @class = "{required:true,messages:{required:'Bạn chưa nhập chủ đề'}}" })%>
                </p>
                <p>
                    <label for="Tos">Người nhận<span class="required"> *</span></label>
                    <%= Html.TextArea("Tos", ViewData.Model, new { @class = "{required:true,messages:{required:'Bạn chưa nhập địa chỉ những người nhận'}}" })%>
                </p>
                <p>
                    <label for="Body">Nội dung<span class="required"> *</span></label>
                    <%= Html.TextArea("Body", "", new { @class = "{required:true,messages:{required:'Bạn chưa nhập nội dung email'}}" })%>
                </p>
                <p class="buttons">
                    <a id="btnSend" class="button">Gửi đi</a>
                </p>
            </fieldset>
        <% } %>
        
        <script type="text/javascript">
            $("#btnSend").click(function() {
                var self = $("#fSendMail");
                if (!self.valid()) {
                    return;
                }
                var button = $(this).attr('disabled', true);
                var action = self.attr('action');
                var data = self.serialize();
                $.post(action, data, function(result) {
                    button.removeAttr('disabled');
                    if (result.Status) {
                        $("#Body", self).val('');
                    }
                    else {
                        alert(result.Message);
                    }
                }, "json");
                return false;
            });
        </script>
    </div>
</asp:Content>
