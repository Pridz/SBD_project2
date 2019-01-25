using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    class Bufor
    {
        Page[] page;
        int pageSize;
        int amountOfPages;

        public int PageSize
        {
            get { return pageSize; }
        }

        public int AmountOfPages
        {
            get { return amountOfPages; }
        }

        public Page this[int i]
        {
            get { return page[i]; }
            set { page[i] = value; }
        }

        public Record this[int i,int j]
        {
            get { return page[i][j]; }
            set { page[i][j] = value; }
        }

        public Bufor(int pageSize, int amountOfPages)
        {
            this.pageSize = pageSize;
            this.amountOfPages = amountOfPages;
            page = new Page[amountOfPages];
        }
    }
}
