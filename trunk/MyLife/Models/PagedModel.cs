using System;
using System.Collections.Generic;

namespace MyLife.Models
{
    [Serializable]
    public class PagedModel<T>
    {
        public PagedModel()
        {
        }

        public PagedModel(IList<T> items)
        {
            Items = items;
        }

        public PagedModel(IList<T> items, int indexOfPage, int sizeOfPage, int totalItems)
        {
            Items = items;
            IndexOfPage = indexOfPage;
            SizeOfPage = sizeOfPage;
            TotalItems = totalItems;
        }

        public IList<T> Items { get; set; }
        public int IndexOfPage { get; set; }
        public int SizeOfPage { get; set; }

        public bool HasNextPage
        {
            get { return IndexOfPage < TotalPages; }
        }

        public bool HasPrevPage
        {
            get { return IndexOfPage > 1; }
        }

        public int TotalPages
        {
            get { return Utils.CalcTotalPages(TotalItems, SizeOfPage); }
        }

        public int TotalItems { get; set; }
    }
}