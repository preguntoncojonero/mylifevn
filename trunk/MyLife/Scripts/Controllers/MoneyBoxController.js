MoneyBoxController = function(baseUrl) {
    $.extend(this, new Controller());
    this.addView(new View("divRecords"));
    this.addView(new View("divStatistics", {
        onShow: function() {
            var el = $("#tblCharts");
            var offset = el.offset();
            var top = offset.top + el.height() + 20;
            $("#silverlightControlHostWrapper").css({
                top: top + "px",
                left: offset.left + "px"
            });
        },
        onHiding: function() {
            $("#silverlightControlHostWrapper").css("top", "-1000px");
        }
    }));
    this.addView(new View("divCategories"));
    this.addView(new View("divSettings"));
    this.baseUrl = baseUrl;
    this.currentIndex = 1;
    this.records = null;
    this.categories = null;
    this.earning = 0;
    this.expense = 0;
    this.overall = 0;
    this.recordsTpl = new XTemplate(
        '<tpl for=".">',
            '<tr>',
                '<td style="width: 80px;">{DateFormat}</td>',
                '<td>{CategoryName}</td>',
                '<tpl if="Amount &gt; 0">',
                    '<td style="color: #008000; font-weight: bold; text-align: right;">{Amount}</td>',
                '</tpl>',
                '<tpl if="Amount &lt; 0">',
                    '<td style="color: #DD0000; font-weight: bold; text-align: right;">{Amount}</td>',
                '</tpl>',
                '<td style="text-align: center;">',
                    '<a class="button edit" onclick="controller.editRecord({Id})">Sửa</a>',
                    '<a class="button delete" onclick="controller.deleteRecord({Id})">Xóa</a>',
                '</td>',
            '</tr>',
        '</tpl>'
    );
    this.categoriesTpl = new XTemplate(
        '<tpl for=".">',
            '<tr>',
                '<td>{Name}</td>',
                '<td style="text-align: center"><input class="colorSelector" readonly="readonly" style="background-color: #{ColorHex}" type="text" /></td>',
                '<td align="center">',
                    '<a class="button edit" onclick="controller.editCategory({Id})">Sửa</a>',
                    '<a class="button delete" onclick="controller.deleteCategory({Id})">Xóa</a>',
                '</td>',
            '</tr>',
        '</tpl>'
    );
};
MoneyBoxController.prototype.loadCategories = function() {
    var url = this.baseUrl + "/getcategories";
    var self = this;
    this.post(url, {}, function(obj) {
        if (obj.Status) {
            self.onCategoriesChanged(obj.Data);
        }
    }, "json");
};
MoneyBoxController.prototype.onCategoriesChanged = function(categories) {
    this.categories = categories;
    this.categoriesTpl.overwrite($("#tblCategories tbody"), categories);
    var dropDownList = ClearOptionsFast("record_CategoryId");
    $(categories).each(function(i, item) {
        dropDownList[i] = new Option(item.Name, item.Id);
    });
};
MoneyBoxController.prototype.addCategory = function() {
    $("#tblCategories").hide();
    $("#fAddOrEditCategory h2").html('Thêm danh mục mới');
    $("#category_Name").val("");
    $("#category_Id").val("0");
    $("#btnAddOrEditCategory").removeClass('edit').addClass('add').html('Thêm mới');
    $("#divAddOrEditCategory").show();
};
MoneyBoxController.prototype.editCategory = function(id) {
    var db = new TAFFY(this.categories);
    var category = db.first({ Id: id });
    $("#tblCategories").hide();
    $("#fAddOrEditCategory h2").html('Chỉnh sửa danh mục');
    $("#category_Name").val(category.Name);
    $("#category_ColorHex").val(category.ColorHex).css('background-color', '#' + category.ColorHex).css('color', '#' + category.ColorHex);
    $("#category_Id").val(category.Id);
    $("#btnAddOrEditCategory").removeClass('add').addClass('edit').html('Chỉnh sửa');
    $("#divAddOrEditCategory").show();
};
MoneyBoxController.prototype.deleteCategory = function(id) {
    if (!confirm("Bạn có muốn xóa danh mục này?")) return false;
    var action = this.baseUrl + "/deletecategory";
    var self = this;
    this.post(action, { Id: id }, function(obj) {
        if (obj.Status) {
            self.onCategoriesChanged(obj.Data);
        }
    }, "json");
};
MoneyBoxController.prototype.addOrEditCategory = function() {
    var self = $("#fAddOrEditCategory");
    if (!self.valid()) {
        return false;
    }
    var action = this.baseUrl + "/addoreditcategory";
    var data = self.serialize();
    var self = this;
    this.post(action, data, function(obj) {
        if (obj.Status) {
            self.onCategoriesChanged(obj.Data);
            self.cancelAddOrEditCategory();
        }
    }, "json");
};
MoneyBoxController.prototype.cancelAddOrEditCategory = function() {
    $("#divAddOrEditCategory").hide();
    $("#tblCategories").show();
};
MoneyBoxController.prototype.loadRecords = function() {
    var url = this.baseUrl + "/getrecords";
    var self = this;
    this.post(url, { IndexOfPage: self.currentIndex }, function(obj) {
        if (obj.Status) {
            self.onRecordsChanged(obj.Data);
        }
    }, "json");
};
MoneyBoxController.prototype.onRecordsChanged = function(obj) {
    this.records = obj.Items;
    this.recordsTpl.overwrite($("#tblRecords tbody"), this.records);
    $("#btnPrevious").toggle(obj.HasPrevPage);
    $("#btnNext").toggle(obj.HasNextPage);
    this.earning = obj.Earning;
    this.expense = obj.Expense;
    this.overall = obj.Overall;
    $("#txtEarning").html(obj.Earning);
    $("#txtExpense").html(obj.Expense);
    $("#txtOverall").html(obj.Overall);
};
MoneyBoxController.prototype.loadNextRecords = function() {
    this.currentIndex++;
    this.loadRecords();
    return false;
};
MoneyBoxController.prototype.loadPreviousRecords = function() {
    this.currentIndex--;
    this.loadRecords();
    return false;
};
MoneyBoxController.prototype.addRecord = function() {
    $("#tblRecords, #tblStatistics").hide();
    $("#fAddOrEditRecord h2").html('Thêm bản ghi mới');
    $("#record_Amount").val(1000);
    $("#record_Date").val($("#hdfServerDate").val());
    $("#record_Description").val("");
    $("#record_Id").val(0);
    $("#btnAddOrEditRecord").removeClass('edit').addClass('add').html("Thêm mới");
    $("#divAddOrEditRecord").show();
};
MoneyBoxController.prototype.editRecord = function(id) {
    var db = new TAFFY(this.records);
    var record = db.first({ Id: id });
    $("#tblRecords, #tblStatistics").hide();
    $("#fAddOrEditRecord h2").html('Chỉnh sửa bản ghi');
    $("#record_CategoryId").val(record.CategoryId);
    $("#record_Earning").attr('checked', record.Amount > 0);
    $("#record_Expense").attr('checked', record.Amount < 0);
    $("#record_Amount").val(Math.abs(record.Amount));
    $("#record_Date").val(record.DateFormat);
    $("#record_Description").val(record.Description);
    $("#record_Id").val(record.Id);
    $("#btnAddOrEditRecord").removeClass('add').addClass('edit').html("Chỉnh sửa");
    $("#divAddOrEditRecord").show();
};
MoneyBoxController.prototype.deleteRecord = function(id) {
    if (!confirm('Bạn có muốn xóa bản ghi này?')) return false;
    var url = this.baseUrl + "/deleterecord";
    var self = this;
    this.post(url, { Id: id }, function(obj) {
        if (obj.Status) {
            self.onRecordsChanged(obj.Data);
        }
    }, "json");
    return false;
};
MoneyBoxController.prototype.addOrEditRecord = function() {
    var self = $("#fAddOrEditRecord");
    if (!self.valid()) {
        return false;
    }
    var action = this.baseUrl + "/addoreditrecord";
    $("#record_CategoryName").val($("#record_CategoryId option:selected").text());
    var data = self.serialize();    
    var self = this;
    this.post(action, data, function(obj) {
        if (obj.Status) {
            self.onRecordsChanged(obj.Data);
            self.cancelAddOrEditRecord();
        }
    }, "json");
};
MoneyBoxController.prototype.cancelAddOrEditRecord = function() {
    $("#divAddOrEditRecord").hide();
    $("#tblRecords, #tblStatistics").show();
};
MoneyBoxController.prototype.chartEarningsByCategories = function() {
    var url = this.baseUrl + "/getearningbycategories";
    var self = this;
    this.post(url, {}, function(obj) {
        if (obj.Status) {
            var chartObj = self.getChartObject();
            var str = $.toJSON(obj.Data);
            chartObj.EarningByCategories(str);
        }
    }, "json");
};
MoneyBoxController.prototype.chartExpenseByCategories = function() {
    var url = this.baseUrl + "/getexpensebycategories";
    var self = this;
    this.post(url, {}, function(obj) {
        if (obj.Status) {
            var chartObj = self.getChartObject();
            var str = $.toJSON(obj.Data);
            chartObj.ExpenseByCategories(str);
        }
    }, "json");
};
MoneyBoxController.prototype.chartBalance = function() {
    var url = this.baseUrl + "/getbalance";
    var self = this;
    this.post(url, {}, function(obj) {
        if (obj.Status) {
            var chartObj = self.getChartObject();
            chartObj.Balance(obj.Data.Earning, Math.abs(obj.Data.Expense));
        }
    }, "json");
};
MoneyBoxController.prototype.getChartObject = function() {
    return document.getElementById("chartControl").Content.Bridge;
};
function ClearOptionsFast(id) {
    var selectObj = document.getElementById(id);
    var selectParentNode = selectObj.parentNode;
    var newSelectObj = selectObj.cloneNode(false);
    selectParentNode.replaceChild(newSelectObj, selectObj);
    return newSelectObj;
}