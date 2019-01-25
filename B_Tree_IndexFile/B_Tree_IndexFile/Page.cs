using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    class Page
    {
        Record[] page;
        int size;
        int capacity;

        public Record this[int i]
        {
            get { return page[i]; }
            set { page[i] = value; }
        }

        public Page(int size)
        {
            page = new Record[size];
            this.size = size;
            capacity = 0;
        }
    }
}
