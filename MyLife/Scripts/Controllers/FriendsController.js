FriendsController = function(baseUrl) {
    $.extend(this, new Controller());
    this.addView(new View("divAddOrEditGroup"));
    this.addView(new View("divAddOrEditFriend"));
    this.addView(new View("divShowFriend"));
    this.addView(new View("divFriends"));
    this.addView(new View("divSendMail"));
    this.addView(new View("divBirthdays"));
    this.baseUrl = baseUrl;
    this.groups = null;
    this.friends = null;
    this.tplGroups = new XTemplate(
        '<tpl for=".">',
            '<li ref="{Id}"><a onclick="controller.changeGroup({Id})">{Name}</a></li>',
        '</tpl>'
    );
    this.tplFriends = new XTemplate(
        '<tpl for=".">',
            '<li ref="{Id}" onclick="controller.showFriend({Id})" style="background: url({AvatarUrl})">',
                '<span>{FullName:htmlEncode}</span>',
            '</li>',
        '</tpl>'
    );
    this.tplFriendGroups = new XTemplate(
        '<tpl for=".">',
            '<li><input type="checkbox" name="friend.Groups" value="{Id}" />{Name}</li>',
        '</tpl>'
    );
    this.tplBirthdays = new XTemplate(
        '<tpl for=".">',
            '<li ref="{Id}" onclick="controller.showFriend({Id})" style="background: url({AvatarUrl})">',
                '<span>{FullName:htmlEncode}<br/><b>(còn {DaysOfBirthday} ngày)</b></span>',
            '</li>',
        '</tpl>'
    );
    this.initialize();
};
FriendsController.ViewMode = {
    ViewByGroup: 0,
    ViewByLetter: 1,
    ViewByBirthdays: 2
};
FriendsController.prototype.initialize = function() {
    this.viewmode = FriendsController.ViewMode.ViewByBirthdays;
    this.currentGroupId = null;
    this.currentLetter = null;
};
FriendsController.prototype.loadGroups = function() {    
    var url = this.baseUrl + "/getgroups";
    var self = this;
    this.post(url, {}, function(result) {
        if (result.Status) {
            self.onGroupsChanged(result.Data);
        }
    }, "json");
};
FriendsController.prototype.onGroupsChanged = function(groups) {
    this.groups = new TAFFY(groups);
    this.tplGroups.overwrite($("#listGroups"), groups);
    $("#listGroups li:first").addClass('disabled');
    var self = this;
    $("#listGroups li").contextMenu({
        menu: 'groupsContextMenu'
    }, function(action, el, pos) {
        self.groupsContextMenuEvent(action, el, pos);
    });
};
FriendsController.prototype.onFriendsChanged = function(friends) {
    this.friends = new TAFFY(friends);
    this.renderFriends();
};
FriendsController.prototype.loadLetters = function() {
    for (var i = 65; i <= 90; i++) {
        var character = String.fromCharCode(i);
        $("#divLetters").append($("<a href=\"#\" href=\"javascript:void(0)\"></a>").html(character));
    }

    var self = this;
    $("#divLetters a").click(function() {
        var letter = $(this).html();
        self.viewmode = FriendsController.ViewMode.ViewByLetter;
        self.currentLetter = letter;
        self.renderFriends();
        self.showViewById("divFriends");
    });
};
FriendsController.prototype.groupsContextMenuEvent = function(action, el, pos) {
    var id = parseInt(el.attr('ref'));
    switch (action) {
        case "open":
            this.changeGroup(id);
            break;
        case "rename":
            this.editGroup(id);
            break;
        case "delete":
            this.deleteGroup(id);
            break;
        case "mail":
            this.sendmailGroup(id);
            break;
    }
};
FriendsController.prototype.friendsContextMenuEvent = function(action, el, pos) {
    var id = parseInt(el.attr('ref'));
    switch (action) {
        case "open":
            this.showFriend(id);
            break;
        case "edit":
            this.editFriend(id);
            break;
        case "delete":
            this.deleteFriend(id);
            break;
        case "mail":
            this.sendmailFriend(id);
            break;
    }
};
FriendsController.prototype.sendmailGroup = function(id) {
    var group = this.groups.first({ Id: id });
    var friends = this.friends.get({ Groups: { has: { Id: id} }, Email: { length: { gt: 0}} });
    $("#fSendMail legend").html('Gửi thư cho nhóm bạn: ' + group.Name);
    var tos = "";
    $(friends).each(function(i, item) {
        tos += item.Email + ";";
    });
    $("#email_Tos").val(tos);
    $("#email_Content").val('');
    this.showViewById("divSendMail");
};
FriendsController.prototype.sendmailFriend = function(id) {
    var friend = this.friends.first({ Id: id });
    $("#fSendMail legend").html('Gửi thư cho một người bạn: ' + friend.FullName);
    $("#email_Tos").val(friend.Email);
    $("#email_Content").val('');
    this.showViewById("divSendMail");
};
FriendsController.prototype.sendMail = function() {
    var value = $.trim($("#email_Subject").val());
    if (value == "") {
        alert("Bạn chưa nhập tiêu đề thư");
        return false;
    }
    else {
        $("#email_Subject").val(value);
    }

    value = $.trim($("#email_Tos").val());
    if (value == "") {
        alert("Bạn không thể gửi bức thư này do không có người nhận");
        return false;
    }
    else {
        $("#email_Tos").val(value);
    }

    value = $.trim($("#email_Content").val());
    if (value == "") {
        alert("Bạn không thể gửi bức thư này do không có nội dung");
        return false;
    }
    else {
        $("#email_Content").val(value);
    }

    var self = this;
    var url = this.baseUrl + "/sendmail";
    var data = $("#fSendMail").serialize();
    $.post(url, data, function(result) {
        alert(result.Message);
        if (result.Status) {
            self.back();
        }
    }, "json");
};
FriendsController.prototype.changeGroup = function(id) {
    this.viewmode = FriendsController.ViewMode.ViewByGroup;
    this.currentGroupId = id;
    this.renderFriends();
    this.showViewById("divFriends");
};
FriendsController.prototype.addGroup = function() {
    $("#group_Id").val(0);
    $("#group_Name").val("");
    $("#fAddOrEditGroup h2").html("Thêm nhóm mới");
    $("#btnAddOrEditGroup").html("Thêm mới").removeClass("edit").addClass("add");
    this.showViewById("divAddOrEditGroup");
};
FriendsController.prototype.editGroup = function(id) {
    var group = this.groups.first({ Id: id });
    $("#group_Id").val(group.Id);
    $("#group_Name").val(group.Name);
    $("#fAddOrEditGroup h2").html("Đổi tên nhóm");
    $("#btnAddOrEditGroup").html("Chỉnh sửa").removeClass("add").addClass("edit");
    this.showViewById("divAddOrEditGroup");
};
FriendsController.prototype.deleteGroup = function(id) {
    if (!confirm('Bạn có muốn xóa nhóm này?')) {
        return false;
    }
    var url = this.baseUrl + "/deletegroup";
    var self = this;
    $.post(url, { Id: id }, function(result) {
        if (result.Status) {
            self.onGroupsChanged(result.Data);
        } else {
            alert(result.Message);
        }
    }, "json");
};
FriendsController.prototype.addOrEditGroup = function() {
    var self = $("#fAddOrEditGroup");
    if (!self.valid()) {
        return false;
    }
    var action = this.baseUrl + "/addoreditgroup";
    var data = self.serialize();
    var controller = this;
    $.post(action, data, function(result) {
        if (result.Status) {
            controller.onGroupsChanged(result.Data);
            controller.back();
        }
        else {
            alert(result.Message);
        }
    }, "json");
    return false;
}
FriendsController.prototype.loadFriends = function() {
    var url = this.baseUrl + "/getfriends";
    var self = this;
    this.post(url, {}, function(result) {
        if (result.Status) {
            self.onFriendsChanged(result.Data);
        }
    }, "json");
};
FriendsController.prototype.renderFriends = function() {
    if (this.viewmode == FriendsController.ViewMode.ViewByBirthdays) {
        var todayBirthdays = this.friends.get({ DaysOfBirthday: 0 });
        if (todayBirthdays.length > 0) {
            this.tplBirthdays.overwrite($("#listTodayBirthdays"), todayBirthdays);
            $("#divTodayBirthdays").show();
        } else {
            $("#divTodayBirthdays").hide();
        }

        var weekBirthdays = [];
        this.friends.forEach(function(f, n) {
            if (f.DaysOfBirthday <= 7 && f.DaysOfBirthday > 0) {
                weekBirthdays.push(f);
            }
        });
        if (weekBirthdays.length > 0) {
            this.tplBirthdays.overwrite($("#listWeekBirthdays"), weekBirthdays);
            $("#divWeekBirthdays").show();
        } else {
            $("#divWeekBirthdays").hide();
        }

        var monthBirthdays = [];
        this.friends.forEach(function(f, n) {
            if (f.DaysOfBirthday <= 30 && f.DaysOfBirthday > 7) {
                monthBirthdays.push(f);
            }
        });
        if (monthBirthdays.length > 0) {
            this.tplBirthdays.overwrite($("#listMonthBidthdays"), monthBirthdays);
            $("#divMonthBidthdays").show();
        } else {
            $("#divMonthBidthdays").hide();
        }
    }
    else {
        var friends;
        if (this.viewmode == FriendsController.ViewMode.ViewByGroup) {
            var id = this.currentGroupId;
            if (id > 0) {
                friends = this.friends.get({ Groups: { has: { Id: id}} });
            } else {
                friends = this.friends.get({});
            }
        }
        else if (this.viewmode == FriendsController.ViewMode.ViewByLetter) {
            var letter = this.currentLetter;
            friends = this.friends.get({ Letter: letter });
        }
        else {

        }

        this.tplFriends.overwrite($("#listFriends"), friends);
        var self = this;
        $("#listFriends li").contextMenu({
            menu: 'friendsContextMenu'
        }, function(action, el, pos) {
            self.friendsContextMenuEvent(action, el, pos);
        });
    }
};
FriendsController.prototype.addFriend = function() {
    $("#fAddOrEditFriend h2").html('Thêm một người bạn mới');
    $("#friend_FullName").val('');
    $("#friend_Letter").val('');
    $("#friend_NickName").val('');
    $("#friend_Gender_Girl").attr('checked', true);
    $("#friend_Birthday").val('');
    $("#friend_PhoneNumber").val('');
    $("#friend_MobileNumber").val('');
    $("#friend_Email").val('');
    $("#friend_Website").val('');
    $("#friend_AvatarUrl").val('').attr('disabled', true);
    $("#friend_Gravatar").attr('checked', true);
    $("#friend_YahooNick").val('');
    $("#friend_SkypeNick").val('');
    $("#friend_Address").val('');
    $("#friend_Notes").val('');
    $("#friend_Id").val('0');

    var groups = this.groups.get({});
    this.tplFriendGroups.overwrite($("#listFriendGroups"), groups);
    $("#listFriendGroups li:first").remove();

    $("#btnAddOrEditFriend").html('Thêm mới').removeClass('edit').addClass('add');
    this.showViewById("divAddOrEditFriend");
};
FriendsController.prototype.editFriend = function(id) {
    var friend = this.friends.first({ Id: id });
    $("#fAddOrEditFriend h2").html('Chỉnh sửa thông tin của một người bạn');
    $("#friend_FullName").val(friend.FullName);
    $("#friend_Letter").val(friend.Letter);
    $("#friend_NickName").val(friend.NickName);
    $("#friend_Gender_Girl").attr('checked', !friend.Gender);
    $("#friend_Gender_Boy").attr('checked', friend.Gender);
    var birthday = new Date(parseInt(friend.Birthday.slice(6, 18)));
    $("#friend_Birthday").val(formatDate(birthday, 'dd/MM/yyyy'));
    $("#friend_PhoneNumber").val(friend.PhoneNumber);
    $("#friend_MobileNumber").val(friend.MobileNumber);
    $("#friend_Email").val(friend.Email);
    $("#friend_Website").val(friend.Website);
    $("#friend_AvatarUrl").val(friend.AvatarUrl).removeAttr('disabled');
    $("#friend_Gravatar").removeAttr('checked');
    $("#friend_YahooNick").val(friend.YahooNick);
    $("#friend_SkypeNick").val(friend.SkypeNick);
    $("#friend_Address").val(friend.Address);
    $("#friend_City").val(friend.City);
    $("#friend_Notes").val(friend.Notes);
    $("#friend_Id").val(friend.Id);

    this.tplFriendGroups.overwrite($("#listFriendGroups"), this.groups.get({}));
    $("#listFriendGroups li:first").remove();

    $(friend.Groups).each(function(i, item) {
        $("#listFriendGroups input[value=" + item.Id + "]").attr('checked', true);
    });

    $("#btnAddOrEditFriend").html('Chỉnh sửa').removeClass('add').addClass('edit');
    this.showViewById("divAddOrEditFriend");
};
FriendsController.prototype.addOrEditFriend = function() {
    var self = $("#fAddOrEditFriend");
    if (!self.valid()) {
        $('html,body').animate({ scrollTop: 0 }, 1000);
        return false;
    }
    var action = this.baseUrl + "/addoreditfriend";
    var data = self.serialize();
    var controller = this;
    $.post(action, data, function(result) {
        if (result.Status) {
            controller.onFriendsChanged(result.Data);
            controller.back();
        }
        else {
            alert(result.Message);
        }
    }, "json");
    return false;
};
FriendsController.prototype.showFriend = function(id) {
    var friend = this.friends.first({ Id: id });
    $("#fShowFriend h2").html(Format.htmlEncode(friend.FullName));
    $("#iFriend_Avatar").attr('src', friend.AvatarUrl).attr('alt', friend.FullName);
    $("#lblFriend_FullName").html(Format.htmlEncode(friend.FullName));
    $("#lblFriend_Letter").html(friend.Letter);
    $("#lblFriend_NickName").html(Format.htmlEncode(friend.NickName));
    if (friend.Gender) {
        $("#lblFriend_Gender").html("Con trai").removeClass('female').addClass('male');
    } else {
        $("#lblFriend_Gender").html("Con gái").removeClass('male').addClass('female');
    }
    var birthday = new Date(parseInt(friend.Birthday.slice(6, 18)));
    $("#lblFriend_Birthday").html(formatDate(birthday, 'dd/MM/yyyy'));
    $("#lblFriend_PhoneNumber").html(Format.htmlEncode(friend.PhoneNumber));
    $("#lblFriend_MobileNumber").html(Format.htmlEncode(friend.MobileNumber));
    $("#lblFriend_Email").html(Format.htmlEncode(friend.Email));
    $("#lblFriend_Website").html(Format.htmlEncode(friend.Website));
    $("#lnkFriend_AvatarUrl").attr('href', friend.AvatarUrl);
    $("#lblFriend_YahooNick").val(Format.htmlEncode(friend.YahooNick));
    $("#lblFriend_SkypeNick").val(Format.htmlEncode(friend.SkypeNick));
    $("#lblFriend_Address").html(Format.htmlEncode(friend.Address));
    $("#lblFriend_City").html(Format.htmlEncode(friend.City));
    $("#lblFriend_Notes").html(Format.htmlEncode(friend.Notes));

    this.tplFriendGroups.overwrite($("#listFriendGroupsShow"), friend.Groups);
    $("#listFriendGroupsShow input").remove();

    var self = this;
    $("#btnEditFriend").unbind('click').click(function() {
        self.editFriend(friend.Id);
    });

    $("#btnDeleteFriend").unbind('click').click(function() {
        self.deleteFriend(friend.Id);
    });

    this.showViewById("divShowFriend");
};
FriendsController.prototype.deleteFriend = function(id) {
    if (!confirm('Bạn muốn xóa người bạn này?')) {
        return false;
    }

    var self = this;
    var url = this.baseUrl + "/deletefriend";
    $.post(url, { Id: id }, function(result) {
        if (result.Status) {
            self.onFriendsChanged(result.Data);
            self.back();
        } else {
            alert(result.Message);
        }
    }, "json");
};
FriendsController.prototype.back = function() {
    switch (this.viewmode) {
        case FriendsController.ViewMode.ViewByBirthdays:
            this.showViewById("divBirthdays");
            break;
        case FriendsController.ViewMode.ViewByGroup:
        case FriendsController.ViewMode.ViewByLetter:
            this.showViewById("divFriends");
            break;
    }
};