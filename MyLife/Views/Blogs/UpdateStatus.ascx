<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<div class="updateStatus">
    <form id="fUpdateStatus" action="/<%= Model %>/blog/updatestatus">
    <h3>Bạn đang làm gì?<span>160</span></h3>
    <textarea id="txtStatus" name="Status" cols="60" rows="3"></textarea>
    <a id="btnUpdateStatus" class="button edit">Tôi đang làm</a>
    </form>
</div>

<script type="text/javascript">
    $(document).ready(function() {
        $("#txtStatus").keyup(function() {
            var text = $(this).val();
            var textlength = text.length;

            if (textlength > 160) {
                $("#fUpdateStatus h3 span").html(0);
                $(this).val(text.substr(0, 160));
                return false;
            } else {
                $("#fUpdateStatus h3 span").html(160 - textlength);
                return true;
            }
        });

        $("#btnUpdateStatus").click(function() {
            var self = $("#fUpdateStatus");
            var action = self.attr("action");
            var data = self.serialize();
            $.post(action, data, function(result) {
                if (result.Status) {

                } else {
                    alert(result.Message);
                }
            }, "json");
            return false;
        });
    });
</script>

