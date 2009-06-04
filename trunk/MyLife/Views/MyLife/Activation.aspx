<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.metadata.pack.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using (Html.BeginForm("activation", "MyLife", System.Web.Mvc.FormMethod.Post, new { id = "fActivation" })){%>
            <fieldset>
                <legend>Kích hoạt tài khoản</legend>
                <p>
                    <label for="UserName">Tên đăng nhập</label>
                    <%= Html.TextBox("UserName", "", new { autocomplete = "off", @class = "text {required:true,messages:{required:'Bạn hãy nhập tên đăng nhập'}}" })%>
                </p>
                <p>
                    <label for="ActiveKey">Mã kích hoạt</label>
                    <%= Html.TextBox("ActiveKey", "", new { autocomplete = "off", @class = "text {required:true,messages:{required:'Bạn chưa nhập mã kích hoạt'}}" })%>
                </p>
                <p class="buttons">
                    <a id="btnActivation" class="button key">Kích hoạt</a>
                </p>
            </fieldset>
        <% } %>
        <script type="text/javascript">
            $("#btnActivation").click(function() {
                var self = $("#fActivation");
                if (!self.valid()) {
                    return false;
                }
                var button = $(this).attr('disabled', true);
                var action = self.attr('action');
                var data = self.serialize();
                $.post(action, data, function(result) {
                    button.removeAttr('disabled');
                    alert(result.Message);
                    if (result.Status) {
                        window.location = result.Data;
                    }
                }, "json");
                return false;
            });
        </script>
    </div>
</asp:Content>