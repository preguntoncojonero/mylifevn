<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Comment>" %>
<%@ Import Namespace="MyLife.Web.Blogs"%>

<div id="respond" class="cssform">
    <h3>Phản hồi bài viết</h3>
    <% using (Html.BeginForm("addoreditcomment", "blogs",System.Web.Mvc.FormMethod.Post, new { id = "fAddComment" })){%>
        <fieldset>
            <p> 
                <label for="comment_Name">Tên của bạn<span class="required"> *</span></label>
                <%= Html.TextBox("comment.Name", Model.Name)%>           
            </p>
            <p>
                <label for="comment_Email">Địa chỉ email<span class="required"> *</span></label>
                <%= Html.TextBox("comment.Email", Model.Email)%>
            </p>
            <p>
                <label for="comment_Website">Trang web của bạn</label>
                <%= Html.TextBox("comment.Website", Model.Website)%>
            </p>
            <p>
                <%= Html.TextArea("comment.Content", Model.Content, new { rows = 10 })%>
            </p>
            <p class="buttons">
                <%= Html.Hidden("comment.Id", Model.Id.ToString())%>
                <%= Html.Hidden("comment.PostId", Model.PostId.ToString())%>
                <input id="btnAddComment" type="submit" value="Submit Comment" class="button" />
            </p>
        </fieldset>        
    <% } %>
    <%= Html.ClientSideValidation<Comment>("comment") %>
    
    <script type="text/javascript">
        $(document).ready(function() {
            $("#fAddComment").submit(function() {
                var self = $(this);
                if (!self.valid()) {
                    return false;
                }
                var action = self.attr('action');
                var data = self.serialize();
                var button = $("#btnAddComment").attr("disabled", true);
                $.post(action, data, function(result) {
                    button.removeAttr("disabled"); ;
                    if (result.Status) {
                        $("textarea", self).val('');
                    }
                    alert(result.Message);
                }, "json");
                return false;
            });
        });        
    </script>
</div>
