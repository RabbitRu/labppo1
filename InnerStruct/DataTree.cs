using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labppo1.InnerStruct
{
    class DataTree
    {
        private List<GroupInfo> groups;

        public void AddGroup(string groupname)
        {
            groups.Add(new GroupInfo(groupname));
        }


        public GroupInfo this[int index]
        {
            get
            {
                if (index >= 0 && index < groups.Count)
                    return groups[index];
                throw new IndexOutOfRangeException();
            }
        }

        public int Count
        {
            get
            {
                return groups.Count;
            }
        }

        public void Clear()
        {
            groups.Clear();
        }

        public DataTree()
        {
            groups = new List<GroupInfo>();
        }
    }
}
