<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MyLife.Web.MoneyBox"%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Scripts/Controllers/BaseController.js" type="text/javascript"></script>
    <script src="/Scripts/Controllers/MoneyBoxController.js" type="text/javascript"></script>
    <script src="/Scripts/colorpicker.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.pack.js" type="text/javascript"></script>
    <script src="/Scripts/xVal.jquery.validate.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.json-1.3.min.js" type="text/javascript"></script>
    <script src="/Scripts/ControlsToolkit.js" type="text/javascript"></script>    
    <script src="/Scripts/XTemplate.js" type="text/javascript"></script>    
    <script src="/Scripts/format.js" type="text/javascript"></script>
    <script src="/Scripts/taffy-min.js" type="text/javascript"></script>
    <link href="/Content/Themes/colorpicker.css" rel="stylesheet" type="text/css" media="screen" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <ul class="cssmenu">
        <li><a href="#records" class="current">Tiền của bạn</a></li>
        <li><a href="#statistics">Thống kê</a></li>
        <li><a href="#categories">Các danh mục</a></li>
        <li><a href="#settings">Thiết lập</a></li>        
    </ul>
    <div class="clear"></div>
    <div id="divRecords">
        <table class="csstable" id="tblRecords">
            <thead>
                <tr>
                    <th style="width: 80px;">Ngày</th>
                    <th>Danh mục</th>
                    <th>Số tiền</th>
                    <th style="width: 120px;">&nbsp;</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot>
                <tr>
                    <td colspan="3" align="center">
                        <div>
                            <a class="button previous" id="btnPrevious" onclick="controller.loadPreviousRecords()">Trước</a>&nbsp;&nbsp;&nbsp;
                            <a class="button next" id="btnNext" onclick="controller.loadNextRecords()">Sau</a>
                        </div>
                    </td>
                    <td align="center"><a class="button add" id="btnAddRecord" onclick="controller.addRecord()">Thêm mới</a></td>
                </tr>
            </tfoot>
        </table>
        <table id="tblStatistics" class="csstable" style="margin-top: 20px;">
            <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th style="text-align: center;">Thu nhập</th>
                    <th style="text-align: center;">Chi tiêu</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>&nbsp;</td>
                    <td id="txtEarning" style="text-align: center; color: #008000; font-weight: bold"></td>
                    <td id="txtExpense" style="text-align: center; color: #DD0000; font-weight: bold"></td>
                </tr>
                <tr>
                    <td style="font-weight: bold; text-align: center">Tổng cộng</td>
                    <td id="txtOverall" colspan="2" style="font-weight: bold; text-align: center"></td>
                </tr>
            </tbody>
        </table>
        <div id="divAddOrEditRecord" style="display: none" class="cssform">
            <form action="#" id="fAddOrEditRecord">
                <h2></h2>
                <div>
                    <label for="record_CategoryId">Danh mục</label>
                    <select id="record_CategoryId" name="record.CategoryId"></select>
                    <input type="hidden" id="record_CategoryName" name="record.CategoryName" />
                </div>
                <div>
                    <label for="record_Amount">Số tiền</label>
                    <input type="radio" id="record_Earning" name="record.Earning" value="true" checked="checked" /> Thu nhập&nbsp;&nbsp;&nbsp;
                    <input type="radio" id="record_Expense" name="record.Earning" value="false" /> Chi tiêu
                </div>
                <div>
                    <label>&nbsp;</label>
                    <input type="text" id="record_Amount" name="record.Amount" maxlength="9" value="0" />
                </div>
                <div>
                    <label for="record_Date">Ngày tháng</label>
                    <input type="text" id="record_Date" name="record.Date" maxlength="10" />
                </div>
                <div>
                    <label for="record_Description">Mô tả</label>
                    <input type="text" id="record_Description" name="record.Description" maxlength="255" />
                </div>
                <div class="buttons">
                    <a class="button" id="btnAddOrEditRecord" onclick="controller.addOrEditRecord()">Thêm mới</a>&nbsp;&nbsp;&nbsp;
                    <a class="button cancel" onclick="controller.cancelAddOrEditRecord()">Hủy bỏ</a>
                </div>
                <input type="hidden" id="record_Id" name="record.Id" value="0" />
            </form>
            <%= Html.ClientSideValidation<Record>("record")%>
        </div>
    </div>
    <div id="divStatistics" style="display: none;">        
        <table id="tblCharts" cellspacing="10">
            <tr>
                <td><a class="button pie" onclick="controller.chartEarningsByCategories()">Thu nhập theo danh mục</a></td>
                <td><a class="button pie" onclick="controller.chartExpenseByCategories()">Chi tiêu theo danh mục</a></td>
                <td><a class="button balance" onclick="controller.chartBalance()">Cân bằng tài chính</a></td>
            </tr>
        </table>
        <div style="height: 350px;">&nbsp;</div>
    </div>
    <div id="divCategories" style="display: none">
        <table class="csstable" id="tblCategories">
            <thead>
                <tr>
                    <th>Danh mục</th>
                    <th style="width: 80px">Màu sắc</th>
                    <th style="width: 120px">&nbsp;</th>
                </tr>
            </thead>
            <tbody></tbody>
            <tfoot>
                <tr>
                    <td colspan="2">&nbsp;</td>
                    <td style="text-align: center">
                        <a class="button add" id="btnAddCategory" onclick="controller.addCategory()">Thêm mới</a>
                        <%= Html.Hidden("hdfCategories", Utils.GetJsonString(Model)) %>
                    </td>
                </tr>
            </tfoot>
        </table>
        <div id="divAddOrEditCategory" class="cssform" style="display: none">
            <form action="#" id="fAddOrEditCategory">
                <h2></h2>
                <div>
                    <label for="category_Name">Danh mục</label>
                    <input type="text" id="category_Name" name="category.Name" maxlength="255" /><br />
                    <span class="notes">Ví dụ: Ăn uống, Thu nhập, Mua sắm, Điện thoại, Xăng xe ...</span>
                </div>
                <div>
                    <label>Màu sắc</label>                            
                    <input type="text" id="category_ColorHex" readonly="readonly" name="category.ColorHex" class="colorSelector" value="0a0a0a" /><br />
                    <span class="notes">Các màu sắc sẽ hiển thị trong các biểu đồ thống kê</span>
                </div>
                <div class="buttons">
                    <a class="button" id="btnAddOrEditCategory" onclick="controller.addOrEditCategory()">Thêm mới</a>&nbsp;&nbsp;&nbsp;
                    <a class="button cancel" onclick="controller.cancelAddOrEditCategory()">Hủy bỏ</a>
                </div>
                <input type="hidden" id="category_Id" name="category.Id" value="0" />
            </form>
            <%= Html.ClientSideValidation<Category>("category") %>
        </div>
    </div>
    <div id="divSettings" style="display: none">
    </div>
    
    <div id="silverlightControlHostWrapper" style="position: absolute; width: 500px; height: 300px; top: -1000px; z-index: -1000">
	    <object id="chartControl" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		    <param name="source" value="/ClientBin/MoneyBoxCharts.xap"/>
		    <param name="onload" value="OnLoaded" />
		    <param name="background" value="white" />
		    <param name="minRuntimeVersion" value="2.0.31005.0" />
		    <param name="autoUpgrade" value="true" />
		    <a href="http://go.microsoft.com/fwlink/?LinkID=124807" style="text-decoration: none; z-index: 1">
 			    <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style: none"/><br />
 			    Bạn cần cài đặt Microsoft Silverlight để chạy được ứng dụng này
		    </a>
	    </object>
	    <iframe style='visibility:hidden;height:0;width:0;border:0px'></iframe>
    </div>
    
    <div id="divAjaxLoading" style="width: 200px; z-index: 9999">
        Đang kết nối với máy chủ<br />
        Nhấn <b style="color: Red">F5</b> nếu chờ quá lâu.
    </div>
    
    <%= Html.Hidden("hdfServerDate", DateTime.UtcNow.AddHours(7).ToString("dd/MM/yyyy")) %>
    
    <script type="text/javascript">
        var controller = null;
        $(document).ready(function() {
            controller = new MoneyBoxController("/<%= User.Identity.Name %>/moneybox");

            $("ul.cssmenu a").click(function() {
                var href = $(this).attr('href');
                switch (href) {
                    case "#records":
                        controller.showViewById("divRecords", true);
                        break;
                    case "#statistics":
                        controller.showViewById("divStatistics", true);
                        break;
                    case "#categories":
                        controller.showViewById("divCategories", true);
                        break;
                    case "#settings":
                        controller.showViewById("divSettings", true);
                        break;
                }
                $("ul.cssmenu a").removeClass("current");
                $(this).addClass("current");
                return false;
            });

            $("#category_ColorHex").ColorPicker({
                onSubmit: function(hsb, hex, rgb) {
                    $("#category_ColorHex").val(hex);
                    $("#category_ColorHex").css('background-color', '#' + hex).css('color', '#' + hex);
                }, onBeforeShow: function() {
                    $(this).ColorPickerSetColor(this.value);
                }
            });

            new AjaxLoading("divAjaxLoading");

            controller.loadRecords();
            controller.loadCategories();
        });

        function OnLoaded(sender, args) {
        }
    </script>
</asp:Content>