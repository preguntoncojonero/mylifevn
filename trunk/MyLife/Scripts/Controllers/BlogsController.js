BlogsController = function(baseUrl) {
    $.extend(this, new Controller());
    this.addView(new View("divSettings"));
    this.addView(new View("divCategories"));
    this.addView(new View("divPosts"));
    this.addView(new View("divBlogrolls"));
    this.baseUrl = baseUrl;
    this.categories = null;
    this.blogrolls = null;
    this.comments = null;
    this.tplBlogrolls = new XTemplate(
        '<tpl for=".">',
            '<tr>',
                '<td><a href="{Link}" target="_blank">{Name}</a></td>',
                '<td style="text-align: center">',
                    '<input type="image" src="/Icons/edit.png" onclick="controller.editBlogroll({Id})" />',
                    '<input type="image" src="/Icons/delete.png" onclick="controller.deleteBlogroll({Id})" />',
                '</td>',
            '</tr>',
        '</tpl>'
    );
    this.tplCategories = new XTemplate(
        '<tpl for=".">',
            '<tr>',
                '<td>{Name}</td>',
                '<td>{Slug}</td>',
                '<td style="text-align: center">',
                    '<input type="image" src="/Icons/edit.png" onclick="controller.editCategory({Id})" />',
                    '<input type="image" src="/Icons/delete.png" onclick="controller.deleteCategory({Id})" />',
                '</td>',
            '</tr>',
        '</tpl>'
    );
    this.tplPosts = new XTemplate(
        '<tpl for=".">',
            '<tr>',
                '<td><a href="{RelativeUrl}" target="_blank">{Title}</a></td>',
                '<td style="text-align: center">',
                    '<input type="image" src="/Icons/edit.png" onclick="controller.editPost({Id})" />',
                    '<input type="image" src="/Icons/delete.png" onclick="controller.deletePost({Id})" />',
                '</td>',
            '</tr>',
        '</tpl>'
    );
};
BlogsController.prototype.updateSettings = function() {
    var self = $("#fBlogSettings");
    if (!self.valid()) {
        return false;
    }
    var action = self.attr('action');
    var data = self.serialize();
    this.post(action, data, function(result) {
    
    }, "json");
};
BlogsController.prototype.loadCategories = function() {
    if (this.categories != null) return;
    var url = this.baseUrl + "/getcategories";
    var self = this;
    this.post(url, {}, function(result) {
        if (result.Status) {
            self.onCategoriesChanged(result.Data);
        }
    }, "json");
};
BlogsController.prototype.onCategoriesChanged = function(categories) {
    this.categories = new TAFFY(categories);
    this.tplCategories.overwrite($("#tblCategories tbody"), categories);
};
BlogsController.prototype.loadPosts = function() {
    if (this.posts != null) return;
    var url = this.baseUrl + "/getposts";
    var self = this;
    this.post(url, {}, function(result) {
        if (result.Status) {
            self.onPostsChanged(result.Data);
        }
    }, "json");
};
BlogsController.prototype.onPostsChanged = function(posts) {
    this.posts = new TAFFY(posts);
    this.tplPosts.overwrite($("#tblPosts tbody"), posts);
};
BlogsController.prototype.loadBlogrolls = function() {
    if (this.posts != null) return;
    var url = this.baseUrl + "/getblogrolls";
    var self = this;
    this.post(url, {}, function(result) {
        if (result.Status) {
            self.onBlogrollsChanged(result.Data);
        }
    }, "json");
};
BlogsController.prototype.onBlogrollsChanged = function(blogrolls) {
    this.blogrolls = new TAFFY(blogrolls);
    this.tplBlogrolls.overwrite($("#tblBlogrolls tbody"), blogrolls);
};
BlogsController.prototype.addCategory = function() {
    var self = $("#fAddOrEditCategory");
    $("#category_Id", self).val("0");
    $("#category_Name", self).val("");
    $("#btnAddOrEditCategory", self).html("Thêm mới").removeClass("edit").addClass("add");
    $("#tblCategories").hide();
    $("#divAddCategory").show();
};
BlogsController.prototype.addOrEditCategory = function() {
    var self = $("#fAddOrEditCategory");
    if (!self.valid()) {
        return false;
    }
    var action = this.baseUrl + "/addoreditcategory";
    var data = self.serialize();
    var controller = this;
    this.post(action, data, function(html) {
        $("#tblCategories").wrap(html);
    }, "html");
    this.cancelAddOrEditCategory();
};
BlogsController.prototype.cancelAddOrEditCategory = function() {
    $("#divAddCategory").hide();
    $("#tblCategories").show();
};
BlogsController.prototype.editCategory = function(id) {
    var url = this.baseUrl + "/getcategorybyid";
    this.post(url, { Id: id }, function(obj) {
        var self = $("#fAddOrEditCategory");
        $("#category_Id", self).val(obj.Id);
        $("#category_Name", self).val(obj.Name);
        $("#btnAddOrEditCategory", self).html("Chỉnh sửa").removeClass("add").addClass("edit");
        $("#tblCategories").hide();
        $("#divAddCategory").show();
    }, "json");
};
BlogsController.prototype.deleteCategory = function(id) {
    if (!confirm('Bạn có muốn xóa chủ đề này?')) {
        return false;
    }
    var action = this.baseUrl + "/deletecategory";
    this.post(action, { Id: id }, function(html) {
        $("#tblCategories").wrap(html);
    }, "html");
};
BlogsController.prototype.addBlogroll = function() {
    var self = $("#fAddOrEditBlogroll");
    $("#blogroll_Id", self).val("0");
    $("#blogroll_Name", self).val("");
    $("#blogroll_Link", self).val("");
    $("#btnAddOrEditBlogroll", self).html("Thêm mới").removeClass("edit").addClass("add");
    $("#tblBlogrolls").hide();
    $("#divAddBlogroll").show();
};
BlogsController.prototype.editBlogroll = function(id) {
    var url = this.baseUrl + "/getblogrollbyid";
    this.post(url, { Id: id }, function(obj) {
        var self = $("#fAddOrEditBlogroll");
        $("#blogroll_Id", self).val(obj.Id);
        $("#blogroll_Name", self).val(obj.Name);
        $("#blogroll_Link", self).val(obj.Link);
        $("#btnAddOrEditBlogroll", self).html("Chỉnh sửa").removeClass("add").addClass("edit");
        $("#tblBlogrolls").hide();
        $("#divAddBlogroll").show();
    }, "json");
};
BlogsController.prototype.deleteBlogroll = function(id) {
    if (!confirm('Bạn có muốn xóa blogroll này?')) {
        return false;
    }
    var action = this.baseUrl + "/deleteblogroll";
    this.post(action, { Id: id }, function(html) {
        $("#tblBlogrolls").wrap(html);
    }, "html");
};
BlogsController.prototype.addOrEditBlogroll = function() {
    var self = $("#fAddOrEditBlogroll");
    if (!self.valid()) {
        return false;
    }
    var action = this.baseUrl + "/addoreditblogroll";
    var data = self.serialize();
    this.post(action, data, function(html) {
        $("#tblBlogrolls").wrap(html);
    }, "html");
    this.cancelAddOrEditBlogroll();
};
BlogsController.prototype.cancelAddOrEditBlogroll = function() {
    $("#divAddBlogroll").hide();
    $("#tblBlogrolls").show();
};
BlogsController.prototype.addPost = function() {
    window.location = this.baseUrl + "/addpost"
};
BlogsController.prototype.editPost = function(id) {
    var url = this.baseUrl + "/editpost/" + id;
    window.location = url;
};
BlogsController.prototype.deletePost = function(id) {
    if (!confirm('Bạn có muốn xóa bài viết này?')) {
        return false;
    }
    var action = this.baseUrl + "/deletepost";
    this.post(action, { Id: id }, function(html) {
        $("#tblPosts").wrap(html);
    }, "html");
};