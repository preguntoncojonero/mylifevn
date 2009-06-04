<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Blogs/iNove.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Models"%>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/jquery.validate.pack.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.metadata.pack.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
        <% using(Html.BeginForm("importrss", "Blogs", System.Web.Mvc.FormMethod.Post, new { id = "fImportRss" })){ %>
            <p class="notes">Bạn có thể dễ dàng chuyển đổi một rss feed các bài viết trong blog của bạn, bạn hãy nhập đường dẫn của feed rss cần chuyển đổi, sau đó nhấn nút <b>Import Rss</b>.
                Nội dung của rss feed sẽ được đưa vào thành các bài viết của bạn</p>
            <div>
                <label for="txtUrl">Đường dẫn rss feed</label>
                <input type="text" id="txtUrl" name="Url" validate="{required:true,url:true,messages:{required:'Bạn hãy nhập rss feed url', url:'Rss feed url không hợp lệ'}}" />
            </div>
            <div class="buttons">
                <a class="button rss-import" id="btnImportRss">Import Rss</a>
            </div>
        <% } %>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            $.metadata.setType('attr', 'validate');

            $("#btnImportRss").click(function() {
                var self = $("#fImportRss");
                if (!self.valid()) {
                    return false;
                }
                
                var url = $("#txtUrl").val();
                $("#btnImportRss").attr("disabled", true);
                var action = $("#fImportRss").attr('action');
                $.post(action, { Url: url }, function(result) {
                    alert(result.Message);
                    $("#btnImportRss").removeAttr("disabled");
                }, "json");
                return false;
            });
        });
    </script>
</asp:Content>
