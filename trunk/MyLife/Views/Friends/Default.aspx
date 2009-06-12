<%@ Page Title="" Language="C#" MasterPageFile="Canvass.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Web.Friends" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/Controllers/BaseController.js" type="text/javascript"></script>
    <script src="/Scripts/Controllers/FriendsController.js" type="text/javascript"></script>
    <script src="/Scripts/XTemplate.js" type="text/javascript"></script>
    <script src="/Scripts/Format.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.pack.js" type="text/javascript"></script>
    <script src="/Scripts/xVal.jquery.validate.js" type="text/javascript"></script>
    <script src="/Scripts/taffy-min.js" type="text/javascript"></script>
    <script src="/Scripts/date.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="/Scripts/ControlsToolkit.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <ul class="groups" id="listGroups"></ul>
    
    <ul id="groupsContextMenu" class="contextMenu" style="width: 150px;">
        <li class="open"><a href="#open"><b>Open</b></a></li>
        <li class="rename"><a href="#rename">Đổi tên</a></li>
        <li class="delete"><a href="#delete">Xóa</a></li>
        <li class="mail"><a href="#mail">Gửi mail nhóm</a></li>
    </ul>
    <div class="clear"></div><br />
    <div id="divLetters" class="letters"></div><br />
    <div id="divButtons">
        <table>
            <tr>
                <td><a id="btnAddGroup" class="button mini-icons group-add">Thêm nhóm mới</a></td>
                <td><a id="btnAddFriend" class="button mini-icons friend-add">Thêm người bạn mới</a></td>
                <td><a id="btnBirthdays" class="button mini-icons birthday">Sắp sinh nhật</a></td>
            </tr>
        </table>
    </div><br />
    <div id="divAddOrEditGroup" class="cssform" style="display: none">
        <form action="#" id="fAddOrEditGroup">
            <h2>Thêm nhóm mới</h2>
            <div>
                <label for="group_Name">Tên nhóm</label>
                <input type="text" maxlength="255" name="group.Name" id="group_Name" />
            </div>
            <div class="buttons">
                <input type="hidden" id="group_Id" name="group.Id" />
                <table>
                    <tr>
                        <td><a class="button mini-icons" id="btnAddOrEditGroup" style="width: 100px;">Thêm mới</a></td>
                        <td><a class="button mini-icons cancel" id="btnCancelAddOrEditGroup" style="width: 100px;">Hủy bỏ</a></td>
                    </tr>
                </table>
            </div>
        </form>
        <%= Html.ClientSideValidation<MyLife.Web.Friends.Group>("group") %>
    </div>
    <div id="divAddOrEditFriend" class="cssform" style="display: none">
        <form action="#" id="fAddOrEditFriend">
            <h2></h2>
            <div>
                <label for="friend_FullName">Họ và tên<span class="required"> *</span></label>
                <input type="text" id="friend_FullName" name="friend.FullName" maxlength="255" />
            </div>
            <div>
                <label for="friend_Letter">Chữ cái đại diện<span class="required"> *</span></label>
                <input type="text" id="friend_Letter" name="friend.Letter" maxlength="1" style="text-transform: uppercase" /><br />
                <span class="notes">Ký tự đại diện cho tên, ví dụ: Đại Nghĩa = N</span>
            </div>
            <div>
                <label for="friend_NickName">Biệt hiệu</label>
                <input type="text" maxlength="255" id="friend_NickName" name="friend.NickName" />
            </div>
            <div>
                <label>Giới tính</label>
                <input type="radio" id="friend_Gender_Boy" name="friend.Gender" value="true" /> Con trai
                <input type="radio" id="friend_Gender_Girl" name="friend.Gender" value="false" /> Con gái
            </div>
            <div>
                <label for="friend_Birthday">Ngày sinh nhật</label>
                <input type="text" id="friend_Birthday" name="friend.Birthday" class="mini-icons birthday" />
                <span class="notes">Định dạng: ngày/tháng/năm, ví dụ: 17/04/1987</span>
            </div>
            <div>
                <label for="friend_PhoneNumber">Số điện thoại cố định</label>
                <input type="text" id="friend_PhoneNumber" name="friend.PhoneNumber" />
            </div>
            <div>
                <label for="friend_MobileNumber">Số điện thoại di động</label>
                <input type="text" id="friend_MobileNumber" name="friend.MobileNumber" />
            </div>
            <div>
                <label for="friend_Email">Địa chỉ email</label>
                <input type="text" id="friend_Email" name="friend.Email" maxlength="255" />
            </div>
            <div>
                <label for="friend_Website">Trang web cá nhân</label>
                <input type="text" id="friend_Website" name="friend.Website" maxlength="255" />
                <span class="notes">Định dạng: http://www.mylifevn.com</span>
            </div>
            <div>
                <label for="friend_AvatarUrl">Đường dẫn avatar</label>
                <input type="text" id="friend_AvatarUrl" name="friend.AvatarUrl" maxlength="255" disabled="disabled" />
            </div>
            <div>
                <label>&nbsp;</label>
                <input type="checkbox" id="friend_Gravatar" name="friend.Gravatar" checked="checked" />
                Sử dụng <a href="http://gravatar.com/">Gravatar</a> làm avatar
            </div>
            <div>
                <label>IM nicks</label>
                <input type="text" id="friend_YahooNick" name="friend.YahooNick" class="yahoo" />
            </div>
            <div>
                <label>&nbsp;</label>
                <input type="text" id="friend_SkypeNick" name="friend.SkypeNick" class="skype" />
            </div>
            <div>
                <label for="friend_Address">Địa chỉ</label>
                <textarea id="friend_Address" name="friend.Address" cols="60" rows="10"></textarea>
            </div>
            <div>
                <label for="friend_City">Thành phố</label>
                <%= Html.DropDownList("friend.City", (IEnumerable<SelectListItem>)ViewData["Cities"])%>
            </div>
            <div>
                <label for="friend_Notes" style="width: auto">Ghi chú các thông tin khác về người bạn này</label>
            </div>
            <div>
                <textarea id="friend_Notes" name="friend.Notes" cols="60" rows="10" style="width: 100%; height: 100px;"></textarea>
            </div>
            <div>
                <label>Nhóm bạn</label>                    
                <ul class="checkboxlist" id="listFriendGroups"></ul>
            </div>
            <div class="buttons">
                <input type="hidden" id="friend_Id" name="friend.Id" />
                <table>
                    <tr>
                        <td><a class="button mini-icons" style="width: 100px;" id="btnAddOrEditFriend">Thêm mới</a></td>
                        <td><a class="button mini-icons cancel" style="width: 100px;" id="btnCancelAddOrEditFriend">Hủy bỏ</a></td>
                    </tr>
                </table>
            </div>
        </form>
        <%= Html.ClientSideValidation<Friend>("friend") %>
    </div>
    <div id="divShowFriend" class="cssform" style="display: none">
        <form action="#" id="fShowFriend">
            <fieldset>
                <legend></legend>
                <div><img id="iFriend_Avatar" src="" alt="" /></div>
                <div>
                    <label>Họ và tên:</label>
                    <span id="lblFriend_FullName" class="content"></span>
                </div>
                <div>
                    <label>Chữ cái đại diện:</label>
                    <span class="content" id="lblFriend_Letter"></span>
                </div>
                <div>
                    <label>Biệt hiệu:</label>
                    <span id="lblFriend_NickName" class="content"></span>
                </div>
                <div>
                    <label>Giới tính:</label>
                    <span id="lblFriend_Gender" class="content"></span>
                </div>
                <div>
                    <label>Ngày sinh nhật:</label>
                    <span id="lblFriend_Birthday" class="content birthday"></span>
                </div>
                <div>
                    <label>Số điện thoại cố định:</label>
                    <span id="lblFriend_PhoneNumber" class="content"></span>
                </div>
                <div>
                    <label>Số điện thoại di động:</label>
                    <span id="lblFriend_MobileNumber" class="content"></span>
                </div>
                <div>
                    <label>Địa chỉ email:</label>
                    <span id="lblFriend_Email" class="content"></span>
                </div>
                <div>
                    <label>Trang web cá nhân:</label>
                    <span id="lblFriend_Website" class="content"></span>
                </div>
                <div>
                    <label>Đường dẫn avatar:</label>
                    <span class="content"><a id="lnkFriend_AvatarUrl" target="_blank">Click here</a></span>
                </div>
                <div>
                    <label>IM nicks:</label>
                    <input type="text" id="lblFriend_YahooNick" class="yahoo noborder" readonly="readonly" />
                </div>
                <div>
                    <label>&nbsp;</label>
                    <input type="text" id="lblFriend_SkypeNick" class="skype noborder" readonly="readonly" />
                </div>
                <div>
                    <label>Địa chỉ:</label>
                    <span id="lblFriend_Address" class="content"></span>
                </div>
                <div>
                    <label>Thành phố:</label>
                    <span id="lblFriend_City" class="content"></span>
                </div>
                <div>
                    <label style="width: auto">Ghi chú các thông tin khác về người bạn này:</label>
                </div>
                <div>
                    <span id="lblFriend_Notes" class="content"></span>
                </div>                
                <div>
                    <label>Nhóm bạn:</label>
                    <ul class="checkboxlist" id="listFriendGroupsShow"></ul>
                </div>
                <div class="buttons">
                    <table>
                        <tr>
                            <td><a class="button edit" style="width: 100px" id="btnEditFriend">Chỉnh sửa</a></td>
                            <td><a class="button delete" style="width: 100px" id="btnDeleteFriend">Xóa</a></td>
                            <td><a class="button cancel" style="width: 100px" id="btnCancelShowFriend">Hủy bỏ</a></td>
                        </tr>
                    </table>
                </div>
            </fieldset>
        </form>
    </div>
    <div id="divFriends" style="display: none">
        <ul class="friends" id="listFriends"></ul>        
        <ul id="friendsContextMenu" class="contextMenu">
            <li class="open"><a href="#open"><b>Open</b></a></li>
            <li class="edit"><a href="#edit">Chỉnh sửa</a></li>
            <li class="delete"><a href="#delete">Xóa</a></li>
            <li class="mail"><a href="#mail">Gửi mail</a></li>
        </ul>
    </div>
    <div id="divSendMail" class="cssform" style="display: none">
        <form action="#" id="fSendMail">
            <fieldset>
                <legend></legend>
                <p>
                    <label for="email_Subject">Tiêu đề thư</label>
                    <input type="text" id="email_Subject" name="email.Subject" />
                </p>
                <p>
                    <label for="email_Tos">Người nhận thư</label><br />
                    <textarea id="email_Tos" name="email.Tos" cols="60" rows="10" style="width: 100%; height: 50px; white-space: normal"></textarea>
                </p>
                <p>
                    <label for="email_Content">Nội dung thư</label><br />
                    <textarea id="email_Content" name="email.Content" cols="60" rows="10" style="width: 100%; height: 150px; white-space: normal"></textarea>
                </p>
                <p class="buttons">
                    <a class="button send-mail" id="btnSendMail">Gửi thư</a>
                    <a class="button cancel" id="btnCancelSendMail">Hủy thư</a>
                </p>
            </fieldset>
        </form>
    </div>
    
    <div id="divBirthdays">
        <div id="divTodayBirthdays">
            <h3>Sinh nhật trong ngày hôm nay</h3>
            <ul id="listTodayBirthdays" class="friends"></ul>
        </div>
        <div class="clear"></div><br />
        <div id="divWeekBirthdays">
            <h3>Sinh nhật trong 7 ngày tới</h3>
            <ul id="listWeekBirthdays" class="friends"></ul>
        </div>
        <div class="clear"></div><br />
        <div id="divMonthBidthdays">
            <h3>Sinh nhật trong 30 ngày tới</h3>
            <ul id="listMonthBidthdays" class="friends"></ul>
        </div>
        <div class="clear"></div>
    </div>
    
    <div id="divAjaxLoading" style="width: 200px; z-index: 9999;">
        Đang kết nối với máy chủ<br />
        Nhấn <b style="color: Red">F5</b> nếu chờ quá lâu.
    </div>
    
    <script type="text/javascript">
        var controller = null;

        $(document).ready(function() {
            controller = new FriendsController('/<%= User.Identity.Name %>/friends');
            controller.loadLetters();

            new AjaxLoading("divAjaxLoading");

            $("#btnAddGroup").click(function() {
                controller.addGroup();
            });

            $("#btnAddOrEditGroup").click(function() {
                controller.addOrEditGroup();
            });

            $("#btnCancelAddOrEditGroup").click(function() {
                controller.back();
            });

            $("#btnAddFriend").click(function() {
                controller.addFriend();
            });

            $("#btnAddOrEditFriend").click(function() {
                controller.addOrEditFriend();
            });

            $("#btnCancelAddOrEditFriend").click(function() {
                controller.back();
            });

            $("#friend_Gravatar").click(function() {
                if (this.checked) {
                    $("#friend_AvatarUrl").attr('disabled', true);
                }
                else {
                    $("#friend_AvatarUrl").removeAttr('disabled');
                }
            });

            $("#btnCancelShowFriend").click(function() {
                controller.back();
            });

            $("#btnSendMail").click(function() {
                controller.sendMail(); ;
            });

            $("#btnCancelSendMail").click(function() {
                controller.back();
            });

            $("#btnBirthdays").click(function() {
                controller.showViewById("divBirthdays");
            });

            controller.loadGroups();
            controller.loadFriends();
        });
    </script>
</asp:Content>
