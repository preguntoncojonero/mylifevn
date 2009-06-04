using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.MoneyBox
{
    public abstract class MoneyBoxProvider : ProviderBase
    {
        public abstract int InsertCategory(Category category);
        public abstract void UpdateCategory(Category category);
        public abstract void DeleteCategory(int id);
        public abstract Category GetCategoryById(int id);
        public abstract IList<Category> GetCategories(string user);

        public abstract int InsertRecord(Record money);
        public abstract void UpdateRecord(Record money);
        public abstract void DeleteRecord(int id);
        public abstract Record GetRecordById(int id);
        public abstract IList<Record> GetRecords(string user);
        public abstract IList<Record> GetRecords(int categoryId);
        public abstract IList<Record> GetRecords(string user, int indexOfPage, int sizeOfPage, out int total);
    }
}