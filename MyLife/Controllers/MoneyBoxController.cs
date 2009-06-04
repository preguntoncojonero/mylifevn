using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyLife.Models;
using MyLife.Web.MoneyBox;
using MyLife.Web.Mvc;

namespace MyLife.Controllers
{
    [HandleError]
    public class MoneyBoxController : BaseController
    {
        [Authorize]
        [CompressedFilter]
        public ActionResult Default()
        {
            ViewData[Constants.ViewData.Title] = "Money Box - Quản lý tài chính cá nhân";
            return View("Default", MyLifeContext.Settings.Theme);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult AddOrEditCategory()
        {
            var id = Convert.ToInt32(Request.Form["category.Id"]);
            var category = id == 0 ? new Category() : Category.GetCategoryById(id);
            UpdateModel(category, "category");
            category.Save();

            var categories = Category.GetCategories(category.CreatedBy);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult DeleteCategory()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var category = Category.GetCategoryById(id);
            category.Delete();

            var categories = Category.GetCategories(category.CreatedBy);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult DeleteRecord()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var record = Record.GetRecordById(id);
            record.Delete();

            return GetRecords(1);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult AddOrEditRecord()
        {
            var id = Convert.ToInt32(Request.Form["record.Id"]);
            var record = id == 0 ? new Record() : Record.GetRecordById(id);
            TryUpdateModel(record, "record");
            DateTime date;
            DateTime.TryParseExact(Request.Form["record.Date"], "d/M/yyyy", null, DateTimeStyles.None, out date);
            record.Date = date;
            if (!Convert.ToBoolean(Request.Form["record.Earning"]))
            {
                record.Amount = record.Amount*-1;
            }
            record.Save();

            return GetRecords(1);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetRecordById()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var record = Record.GetRecordById(id);
            if (record == null)
            {
                throw new HttpException(404, "Bản ghi này không tồn tại.");
            }

            if (!record.CreatedBy.Equals(User.Identity.Name))
            {
                throw new HttpException(401, "Bạn không có quyền đối với bản ghi này");
            }

            return Json(record);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetCategoryById()
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            var category = Category.GetCategoryById(id);
            if (category == null)
            {
                throw new HttpException(404, "Danh mục này không tồn tại.");
            }

            if (!category.CreatedBy.Equals(User.Identity.Name))
            {
                throw new HttpException(401, "Bạn không có quyền đối với danh mục này");
            }

            return Json(category);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetRecords()
        {
            var indexOfPage = Convert.ToInt32(Request.Form["IndexOfPage"]);
            return GetRecords(indexOfPage);
        }

        private ActionResult GetRecords(int indexOfPage)
        {
            int total;
            var records = Record.GetRecords(User.Identity.Name, indexOfPage, 15, out total);
            var obj = new MoneyBoxModel(records, indexOfPage, 15, total)
                          {
                              Earning = Record.GetEarning(User.Identity.Name),
                              Expense = Record.GetExpense(User.Identity.Name)
                          };
            obj.Overall = obj.Earning + obj.Expense;
            return Json(new AjaxModel {Status = true, Data = obj});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetCategories()
        {
            var categories = Category.GetCategories(User.Identity.Name);
            return Json(new AjaxModel {Status = true, Data = categories});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetEarningByCategories()
        {
            var records = Record.GetRecords(User.Identity.Name);
            var obj = from record in records
                      where record.Amount > 0
                      group record by record.CategoryName
                      into record select new {Name = record.Key, Earnings = record.Sum(item => item.Amount)};
            return Json(new AjaxModel {Status = true, Data = obj});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetExpenseByCategories()
        {
            var records = Record.GetRecords(User.Identity.Name);
            var obj = from record in records
                      where record.Amount < 0
                      group record by record.CategoryName
                      into record
                          select new {Name = record.Key, Expenses = record.Sum(item => item.Amount)};
            return Json(new AjaxModel {Status = true, Data = obj});
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [CompressedFilter]
        public ActionResult GetBalance()
        {
            var records = Record.GetRecords(User.Identity.Name);
            var Earning = records.Where(item => item.Amount > 0).Sum(item => item.Amount);
            var Expense = records.Where(item => item.Amount < 0).Sum(item => item.Amount);
            return Json(new AjaxModel {Status = true, Data = new {Earning, Expense}});
        }
    }
}