using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using MyLife.Collection;
using MyLife.Web.Security;
using xVal.ServerSide;

namespace MyLife.Web.MoneyBox
{
    public class Record : BizObject<Record, int>
    {
        private static readonly ICacheManager CacheManager;
        private int amount;
        private bool expense;

        static Record()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.MoneyBox.CacheProvider);
        }

        public Record()
        {
        }

        public Record(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn hãy thêm các danh mục mới")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Bạn hãy nhập số tiền")]
        [Range(1000, int.MaxValue, ErrorMessage = "Số tiền phải bắt đầu từ 1000đ")]
        public int Amount
        {
            get
            {
                if (amount == 0 && !string.IsNullOrEmpty(EncryptedAmount))
                {
                    amount =
                        Convert.ToInt32(
                            Cryptographer.DecryptSymmetric(MyLifeContext.Settings.MoneyBox.SymmetricCryptoProvider,
                                                           EncryptedAmount));
                }

                return amount;
            }
            set { amount = value; }
        }

        [ScriptIgnore]
        public string EncryptedAmount { get; set; }

        [ScriptIgnore]
        [Required(ErrorMessage = "Bạn hãy nhập ngày tháng phát sinh")]
        [RegularExpression(Constants.Regulars.Date, ErrorMessage = "Giá trị không đúng định dạng ngày/tháng/năm")]
        public DateTime Date { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string DateFormat
        {
            get { return Date.ToString("dd/MM/yyyy"); }
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<MoneyBoxProvider>().InsertRecord(this);
        }

        protected override void OnDataInserted()
        {
            base.OnDataInserted();
            CacheManager.Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<MoneyBoxProvider>().UpdateRecord(this);
        }

        protected override void OnDataUpdated()
        {
            base.OnDataUpdated();
            CacheManager.Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<MoneyBoxProvider>().DeleteRecord(Id);
        }

        protected override void OnDataDeleted()
        {
            base.OnDataDeleted();
            CacheManager.Flush();
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            EncryptedAmount = Cryptographer.EncryptSymmetric(MyLifeContext.Settings.MoneyBox.SymmetricCryptoProvider,
                                                             Amount.ToString());
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new MyLifeSecurityException();
            }

            if (!IsNew)
            {
                if (!MyLifeContext.Current.User.Identity.Name.Equals(CreatedBy))
                {
                    throw new MyLifeSecurityException();
                }
            }
        }

        public override void Validate()
        {
            expense = Amount < 0;
            Amount = Math.Abs(Amount);
            try
            {
                base.Validate();
            }
            catch (RulesException ex)
            {
                if (ex.Errors.Count() != 1 || ex.Errors.ElementAt(0).PropertyName != "Date")
                {
                    throw;
                }
            }

            if (expense)
            {
                Amount = Amount*-1;
            }
        }

        public static Record GetRecordById(int id)
        {
            var key = "MoneyBox_GetRecordById_" + id;
            if (CacheManager.Contains(key))
            {
                return (Record) CacheManager[key];
            }
            var money = ServiceManager.GetService<MoneyBoxProvider>().GetRecordById(id);
            if (money != null)
            {
                CacheManager.Add(key, money);
            }
            return money;
        }

        public static IList<Record> GetRecords(string user)
        {
            var key = "MoneyBox_GetRecords_" + user;
            if (CacheManager.Contains(key))
            {
                return (IList<Record>) CacheManager[key];
            }
            var records = ServiceManager.GetService<MoneyBoxProvider>().GetRecords(user);
            CacheManager.Add(key, records);
            return records;
        }

        public static IList<Record> GetRecords(int categoryId)
        {
            var key = "MoneyBox_GetRecordsByCategoryId_" + categoryId;
            if (CacheManager.Contains(key))
            {
                return (IList<Record>) CacheManager[key];
            }
            var records = ServiceManager.GetService<MoneyBoxProvider>().GetRecords(categoryId);
            CacheManager.Add(key, records);
            return records;
        }

        public static IList<Record> GetRecords(string user, int indexOfPage, int sizeOfPage, out int total)
        {
            var key = string.Format("MoneyBox_GetRecords_{0}_{1}_{2}", user, indexOfPage, sizeOfPage);
            if (CacheManager.Contains(key))
            {
                var container = (ContainerCollection<Record>) CacheManager[key];
                total = container.Total;
                return container.List;
            }

            var records = ServiceManager.GetService<MoneyBoxProvider>().GetRecords(user, indexOfPage - 1, sizeOfPage,
                                                                                   out total);
            CacheManager.Add(key, new ContainerCollection<Record> {List = records, Total = total});
            return records;
        }

        public static int GetEarning(string user)
        {
            var key = "MoneyBox_GetEarning_" + user;
            if (CacheManager.Contains(key))
            {
                return (int) CacheManager[key];
            }
            var records = GetRecords(user);
            var earning = records.Where(item => item.Amount > 0).Sum(item => item.Amount);
            CacheManager.Add(key, earning);
            return earning;
        }

        public static int GetEarning(int categoryId)
        {
            if (categoryId == 0)
            {
                return 0;
            }

            var key = "MoneyBox_GetEarningByCategoryId_" + categoryId;
            if (CacheManager.Contains(key))
            {
                return (int) CacheManager[key];
            }
            var records = GetRecords(categoryId);
            var earning = records.Where(item => item.Amount > 0).Sum(item => item.Amount);
            CacheManager.Add(key, earning);
            return earning;
        }

        public static int GetExpense(string user)
        {
            var key = "MoneyBox_GetExpense_" + user;
            if (CacheManager.Contains(key))
            {
                return (int) CacheManager[key];
            }
            var records = GetRecords(user);
            var expense = records.Where(item => item.Amount < 0).Sum(item => item.Amount);
            CacheManager.Add(key, expense);
            return expense;
        }

        public static int GetExpense(int categoryId)
        {
            if (categoryId == 0)
            {
                return 0;
            }

            var key = "MoneyBox_GetExpenseByCategoryId_" + categoryId;
            if (CacheManager.Contains(key))
            {
                return (int) CacheManager[key];
            }
            var records = GetRecords(categoryId);
            var expense = records.Where(item => item.Amount < 0).Sum(item => item.Amount);
            CacheManager.Add(key, expense);
            return expense;
        }
    }
}