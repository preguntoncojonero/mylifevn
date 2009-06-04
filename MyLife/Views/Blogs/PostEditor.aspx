<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Blogs/iNove.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<Post>" %>
<%@ Import Namespace="MyLife.Web"%>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script type="text/javascript" src="/TinyMCE/tiny_mce_gzip.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="cssform">
    <% using(Html.BeginForm("addoreditpost", "Blogs", System.Web.Mvc.FormMethod.Post, new { id = "fAddOrEditPost" })){ %>
        <div>
            <label for="post_Title">Tiêu đề bài viết</label>
            <%= Html.TextBox("post.Title", Model.Title, new { maxlength = 255 }) %>
        </div>
        <div>
            <label for="post_ShortContent">Mô tả</label>
            <%= Html.TextArea("post.ShortContent", Model.ShortContent)%>
        </div>
        <div>
            <label>Các lựa chọn</label>
            <ul class="checkboxlist">
                <li><%= Html.CheckBox("post.Published", Model.Published)%>Xuất bản</li>
                <li><%= Html.CheckBox("post.CommentsEnabled", Model.CommentsEnabled)%>Cho phép phản hồi</li>
            </ul>
        </div>
        <div>
            <label>Các chủ đề</label>
            <%= Html.CheckBoxList("post.Categories", ViewData[Constants.ViewData.Blogs.Categories] as IEnumerable<Category>, Model.Categories, item => item.Name, item => item.Id, 2)%>
        </div>
        <div>
            <%= Html.TextArea("post.Content", Model.Content, new { style = "width: 100%; height: 300px" })%>
        </div>
        <div>
            <%= Html.Hidden("post.Id", Model.Id.ToString())%>
            <input type="button" class="button" id="btnSave" value="Xuất bản bài viết" />
        </div>
    <% } %>
        <%= Html.ClientSideValidation<Post>("post") %>
        <script type="text/javascript">
            tinyMCE_GZ.init({
            plugins: 'pagebreak,style,layer,table,save,advhr,inlinepopups,media,contextmenu,paste,fullscreen',
                themes: 'advanced',
                languages: 'en',
                disk_cache: true,
                debug: false
            });
        </script>
        
        <script type="text/javascript">
            tinyMCE.init({
                mode: 'exact',
                elements: 'post_Content',
                theme: 'advanced',
                skin: 'o2k7',
                relative_urls: false,
                plugins: 'pagebreak,style,layer,table,save,advhr,inlinepopups,media,contextmenu,paste,fullscreen',
                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons2: "cut,copy,paste,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,image,media,cleanup,code,|,forecolor,backcolor",
                theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,advhr,|,fullscreen",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,
                extended_valid_elements: "iframe[src|width|height|name|align|style|scrolling|frameborder],object[width|height|data=data:application/x-silverlight-2|type=application/x-silverlight-2],param[name|value]"
            });
        </script>
        
        <script type="text/javascript">
            $("#btnSave").click(function() {
                var self = $("#fAddOrEditPost");
                if (!self.valid()) {
                    $('html,body').animate({ scrollTop: 0 }, 10);
                    return false;
                }
                var button = $(this).attr('disabled', true);
                tinyMCE.triggerSave(true, true);
                var action = self.attr("action");
                var data = self.serialize();
                $.post(action, data, function(result) {
                    alert(result.Message);
                    if (result.Status) {
                        window.location = result.Data;
                    } else {
                        button.removeAttr('disabled');
                    }
                }, "json");
                return false;
            });
        </script>
    </div>
</asp:Content>