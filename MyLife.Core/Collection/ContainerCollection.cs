using System.Collections.Generic;

namespace MyLife.Collection
{
    public class ContainerCollection<T>
    {
        public ContainerCollection()
        {
        }

        public ContainerCollection(IList<T> list, int total)
        {
            Total = total;
            List = list;
        }

        public int Total { get; set; }
        public IList<T> List { get; set; }
    }
}