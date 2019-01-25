using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class BTreeRecord
    {
        public int key    {get;set;}
        public long position { get; set; }
    }
}
