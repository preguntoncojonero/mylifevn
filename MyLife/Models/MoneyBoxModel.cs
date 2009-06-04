using System.Collections.Generic;
using MyLife.Web.MoneyBox;

namespace MyLife.Models
{
    public class MoneyBoxModel : PagedModel<Record>
    {
        public MoneyBoxModel()
        {
        }

        public MoneyBoxModel(IList<Record> items) : base(items)
        {
        }

        public MoneyBoxModel(IList<Record> items, int indexOfPage, int sizeOfPage, int totalItems)
            : base(items, indexOfPage, sizeOfPage, totalItems)
        {
        }

        public int Earning { get; set; }

        public int Expense { get; set; }

        public int Overall { get; set; }
    }
}