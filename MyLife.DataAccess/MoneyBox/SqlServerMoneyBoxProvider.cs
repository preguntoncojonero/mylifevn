using System.Collections.Generic;
using System.Linq;
using MyLife.Web.MoneyBox;

namespace MyLife.DataAccess.MoneyBox
{
    public class SqlServerMoneyBoxProvider : MoneyBoxProvider
    {
        public override int InsertCategory(Category category)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Categories();
            category.CopyToObject(obj);
            context.AddTotblMoneyBox_Categories(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateCategory(Category category)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Categories {Id = category.Id};
            context.AttachTo("tblMoneyBox_Categories", obj);
            category.CopyToObject(obj);
            context.SaveChanges();
        }

        public override void DeleteCategory(int id)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Categories {Id = id};
            context.AttachTo("tblMoneyBox_Categories", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override Category GetCategoryById(int id)
        {
            var context = new MoneyBoxEntities();
            var obj = context.tblMoneyBox_Categories.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        public override IList<Category> GetCategories(string user)
        {
            var context = new MoneyBoxEntities();
            var list =
                context.tblMoneyBox_Categories.Where(item => item.CreatedBy == user).OrderBy(item => item.Name).ToList();
            return Convert(list);
        }

        private static IList<Category> Convert(List<tblMoneyBox_Categories> list)
        {
            var retval = new List<Category>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        private static Category Convert(tblMoneyBox_Categories obj)
        {
            if (obj == null)
            {
                return null;
            }

            var category = new Category(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            category.CopyFromObject(obj);
            return category;
        }

        public override int InsertRecord(Record record)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Records();
            record.CopyToObject(obj);
            context.AddTotblMoneyBox_Records(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateRecord(Record record)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Records {Id = record.Id};
            context.AttachTo("tblMoneyBox_Records", obj);
            record.CopyToObject(obj);
            context.SaveChanges();
        }

        public override void DeleteRecord(int id)
        {
            var context = new MoneyBoxEntities();
            var obj = new tblMoneyBox_Records {Id = id};
            context.AttachTo("tblMoneyBox_Records", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override Record GetRecordById(int id)
        {
            var context = new MoneyBoxEntities();
            var obj = context.tblMoneyBox_Records.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        private static Record Convert(tblMoneyBox_Records obj)
        {
            if (obj == null)
            {
                return null;
            }

            var money = new Record(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            money.CopyFromObject(obj);
            return money;
        }

        public override IList<Record> GetRecords(string user)
        {
            var context = new MoneyBoxEntities();
            var list =
                context.tblMoneyBox_Records.Where(item => item.CreatedBy == user).OrderByDescending(item => item.Date).
                    ToList();
            return Convert(list);
        }

        public override IList<Record> GetRecords(int categoryId)
        {
            var context = new MoneyBoxEntities();
            var list =
                context.tblMoneyBox_Records.Where(item => item.CategoryId == categoryId).OrderByDescending(item => item.Date).
                    ToList();
            return Convert(list);
        }

        public override IList<Record> GetRecords(string user, int indexOfPage, int sizeOfPage, out int total)
        {
            var context = new MoneyBoxEntities();
            total = context.tblMoneyBox_Records.Where(item => item.CreatedBy == user).Count();
            var list =
                context.tblMoneyBox_Records.Where(item => item.CreatedBy == user).OrderByDescending(item => item.Date).
                    Skip(indexOfPage*sizeOfPage).Take(sizeOfPage).ToList();
            return Convert(list);
        }

        private static IList<Record> Convert(List<tblMoneyBox_Records> list)
        {
            var retval = new List<Record>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }
    }
}