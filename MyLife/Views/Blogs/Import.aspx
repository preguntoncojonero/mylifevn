<%@ Page Title="" Language="C#" MasterPageFile="dfBlog.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Models"%>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/jquery.validate.pack.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.metadata.pack.js" type="text/javascript"></script>
    <script src="/Scripts/ajaxupload.3.2.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center; padding: 10px 40px 10px 40px;">
        <div class="cssform" style="text-align: left">
            <h2>Chuyển đổi từ RSS 2.0</h2>
            <% using(Html.BeginForm("import", "Blogs", System.Web.Mvc.FormMethod.Post, new { id = "fImportRss" })){ %>
                <p class="notes">Bạn có thể dễ dàng chuyển đổi một rss feed các bài viết trong blog của bạn, bạn hãy nhập đường dẫn của feed rss cần chuyển đổi, sau đó nhấn nút <b>Import Rss</b>.
                    Nội dung của rss feed sẽ được đưa vào thành các bài viết của bạn</p>
                <div>
                    <label for="txtUrl">Đường dẫn rss feed</label>
                    <input type="text" id="txtUrl" name="Url" validate="{required:true,url:true,messages:{required:'Bạn hãy nhập rss feed url', url:'Rss feed url không hợp lệ'}}" />
                </div>
                <div class="buttons">
                    <a class="button mini-icons rss" id="btnImportRss">Import Rss</a>
                </div>
            <% } %>
        </div>
        <br /><br /><br />
        <div class="cssform" style="text-align: left">
            <h2>Chuyển đổi từ Yahoo 360 Archive</h2>
            <p>Bạn hãy upload file Yahoo 360 Archive để chuyển đổi thành bài viết. Nếu bạn chưa biết cách tạo file này, hãy click <a href="http://www.mylifevn.com/xox/blog">vào đây</a></p>
            <div style="text-align: center">
                <a href="javascript:void(0)" id="btnYahoo360Archive" class="button mini-icons upload">Upload Yahoo 360 Archive</a>
            </div>
            <%= Html.Hidden("hdfImportYahoo360Archive", "/" + User.Identity.Name + "/blog/importyahoo360archive")%>
        </div>
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

            var url = $("#hdfImportYahoo360Archive").val();
            new AjaxUpload("#btnYahoo360Archive", {
            action: url,
                responseType: false,
                onSubmit: function(file, ext) {
                    if (ext != "txt") {
                        alert("Please upload text file only");
                        return false;
                    }
                },
                onComplete: function(file, response) {
                    alert(response);
                }
            });
        });
    </script>
</asp:Content>
