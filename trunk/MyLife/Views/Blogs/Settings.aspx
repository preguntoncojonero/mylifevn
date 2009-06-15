<%@ Page Title="" Language="C#" MasterPageFile="dfBlog.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<Blog>" %>
<%@ Import Namespace="MyLife.Web.DynamicData"%>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.validate.pack.js"></script>
    <script type="text/javascript" src="/Scripts/xVal.jquery.validate.js"></script>
    <script type="text/javascript" src="/Scripts/Controllers/BaseController.js"></script>
    <script type="text/javascript" src="/Scripts/Controllers/BlogsController.js"></script>
    <script type="text/javascript" src="/Scripts/ControlsToolkit.js"></script>
    <script type="text/javascript" src="/Scripts/XTemplate.js"></script>
    <script type="text/javascript" src="/Scripts/Format.js"></script>
    <script type="text/javascript" src="/Scripts/taffy-min.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">    
    <ul class="cssmenu">
        <li><a href="#settings" class="current">Thiết lập</a></li>
        <li><a href="#categories">Các chủ đề</a></li>
        <li><a href="#posts">Các bài viết</a></li>
        <li><a href="#blogrolls">Các liên kết</a></li>
        <li><a href="#comments">Các phản hồi</a></li>
    </ul>
    <div class="clear"></div>
    <div class="cssform" id="divSettings">
        <h2>Thiết lập giao diện của blog</h2>
        <% using(Html.BeginForm("settings", "Blogs", System.Web.Mvc.FormMethod.Post, new { id = "fBlogSettings" })){ %>
        <div>
            <label for="blog_Name">Tên của blog</label>
            <%= Html.TextBox("blog.Name", Model.Name, new { maxlength = 255})%>
        </div>
        <div>
            <label for="blog_Description">Khẩu hiệu</label>
            <%= Html.TextBox("blog.Description", Model.Description, new { maxlength = 255 })%>
        </div>
        <div>
            <label for="blog_CommentsEnabled">Cho phép phản hồi</label>
            <%= Html.CheckBox("blog.CommentsEnabled", Model.CommentsEnabled)%>
        </div>
        <div>
            <label for="blog_EnableCommentsModeration">Quản lý các phản hồi</label>
            <%= Html.CheckBox("blog.EnableCommentsModeration", Model.ModerationCommentEnable)%>
        </div>
        <div>
            <label for="blog_AnonymousCommentEnabled">Cho phép phản hồi nặc danh</label>
            <%= Html.CheckBox("blog.AnonymousCommentEnabled", Model.AnonymousCommentEnabled)%>
        </div>
        <div>
            <label for="blog_DaysCommentEnabled">Giới hạn số ngày phản hồi</label>
            <%= Html.TextBox("blog.DaysCommentEnabled", Model.DaysCommentEnabled.ToString())%>
        </div>
        <div>
            <label for="blog_NumberOfRecentPosts">Số bài viết gần đây</label>
            <%= Html.TextBox("blog.NumberOfRecentPosts", Model.NumberOfRecentPosts.ToString())%>
        </div>
        <div>
            <label for="blog_PostsPerFeed">Số bài viết trên feed</label>
            <%= Html.TextBox("blog.PostsPerFeed", Model.PostsPerFeed.ToString())%>
        </div>
        <div>
            <label for="blog_PostsPerPage">Số bài viết trên trang</label>
            <%= Html.TextBox("blog.PostsPerPage", Model.PostsPerPage.ToString())%>
        </div>
        <div>
            <label for="blog_Theme">Giao diện</label>
            <%= Html.DropDownList("blog.Theme", (IEnumerable<SelectListItem>)ViewData[Constants.ViewData.Blogs.Themes]) %>
        </div>
        <div>
            <label>&nbsp;</label>
            <input id="btnSettings" type="button" class="button" value="Thiết lập" />
        </div>
    <% } %>
    <%= Html.ClientSideValidation<Blog>("blog") %>
    </div>
    <div id="divCategories" style="display: none">
        <table class="csstable" id="tblCategories">
            <thead>
                <tr>
                    <th>Chủ đề</th>
                    <th>Slug</th>
                    <th style="width: 100px">&nbsp;</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot>
                <tr>
                    <td colspan="2">&nbsp;</td>
                    <td style="text-align: center">
                        <a onclick="controller.addCategory()" class="button add">Thêm mới</a>
                    </td>
                </tr>
            </tfoot>
        </table>
        <div class="cssform" id="divAddCategory" style="display: none">
            <h2>Thêm chủ đề mới</h2>
            <% using (Html.DynamicForm(typeof(Category))) { } %>
        </div>
    </div>
    <div id="divPosts" style="display: none">
        <table class="csstable" id="tblPosts">
            <thead>
                <tr>
                    <th>Bài viết</th>
                    <th style="width: 100px">&nbsp;</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot>
                <tr>
                    <td>&nbsp;</td>
                    <td style="text-align: center">
                        <a onclick="controller.addPost()" class="button add">Thêm mới</a>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <div id="divBlogrolls" style="display: none">
        <table class="csstable" id="tblBlogrolls">
            <thead>
                <tr>
                    <th>Blogroll</th>
                    <th style="width: 100px">&nbsp;</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot>
                <tr>
                    <td>&nbsp;</td>
                    <td style="text-align: center">
                        <a onclick="blogsController.addBlogroll()" class="button add">Thêm mới</a>
                    </td>
                </tr>
            </tfoot>
        </table>
        <div class="cssform" id="divAddBlogroll" style="display: none">
            <h2>Thêm liên kết mới</h2>
            <% using (Html.DynamicForm(typeof(Blogroll))) { } %>
        </div>
    </div>
    
    <div id="divAjaxLoading" style="width: 200px">
        Đang kết nối với máy chủ<br />
        Nhấn <b style="color: Red">F5</b> nếu chờ quá lâu.
    </div>
    
    <script type="text/javascript">
        var controller;

        $(document).ready(function() {
            controller = new BlogsController("/<%= Model.CreatedBy %>/blog");
            new AjaxLoading("divAjaxLoading");

            $("ul.cssmenu a").click(function() {
                var href = $(this).attr('href');
                switch (href) {
                    case "#settings":
                        controller.showViewById("divSettings", true);
                        break;
                    case "#categories":
                        controller.showViewById("divCategories", true);
                        controller.loadCategories();
                        break;
                    case "#posts":
                        controller.showViewById("divPosts", true);
                        controller.loadPosts();
                        break;
                    case "#blogrolls":
                        controller.showViewById("divBlogrolls", true);
                        controller.loadBlogrolls();
                        break;
                    case "#comments":
                        controller.showViewById("divBlogrolls", true);
                        break;
                }
                $("ul.cssmenu a").removeClass("current");
                $(this).addClass("current");
                return false;
            });

            $("#btnSettings").click(function() {
                controller.updateSettings();
                return false;
            });

            $("#btnCancelAddOrEditCategory").click(function() {
                controller.cancelAddOrEditCategory();
                return false;
            });

            $("#btnAddOrEditCategory").click(function() {
                controller.addOrEditCategory();
                return false;
            });

            $("#btnAddOrEditBlogroll").click(function() {
                controller.addOrEditBlogroll();
                return false;
            });

            $("#btnCancelAddOrEditBlogroll").click(function() {
                controller.cancelAddOrEditBlogroll();
                return false;
            });
        });
    </script>
</asp:Content>
