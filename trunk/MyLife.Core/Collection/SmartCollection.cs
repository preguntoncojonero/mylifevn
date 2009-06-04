using System.Collections;
using System.Collections.Generic;

namespace MyLife.Collection
{
    public class SmartCollection<T> : IEnumerator, IEnumerable
    {
        private readonly List<T> currents;
        private readonly List<T> deletes = new List<T>();
        private readonly List<T> news = new List<T>();
        private int position = -1;

        public SmartCollection()
            : this(new List<T>())
        {
        }

        public SmartCollection(List<T> list)
        {
            currents = list ?? new List<T>();
        }

        public int Count
        {
            get { return currents.Count + news.Count; }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            var retval = new List<T>();
            retval.AddRange(currents);
            retval.AddRange(news);
            return retval.GetEnumerator();
        }

        #endregion

        #region IEnumerator Members

        public object Current
        {
            get
            {
                if (position > currents.Count - 1)
                {
                    return news[position - currents.Count];
                }
                return currents[position];
            }
        }

        public bool MoveNext()
        {
            if (position < (currents.Count + news.Count) - 1)
            {
                position++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            position = -1;
        }

        #endregion

        public void Add(T item)
        {
            news.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            news.AddRange(items);
        }

        public bool Contains(T item)
        {
            return currents.Contains(item) || news.Contains(item);
        }

        public void Delete(T item)
        {
            currents.Remove(item);
            news.Remove(item);
            deletes.Add(item);
        }

        public List<T> GetCurrentItems()
        {
            return currents;
        }

        public List<T> GetNewItems()
        {
            return news;
        }

        public List<T> GetDeleteItems()
        {
            return deletes;
        }
    }
}