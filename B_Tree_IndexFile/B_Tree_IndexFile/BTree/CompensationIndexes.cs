using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class CompensationIndexes
    {
        public int parentElementIndex { get; set; }
        public int parentNodePointerIndex { get; set; }

        public CompensationIndexes(int parentElementIndex, int parentNodePointerIndex)
        {
            this.parentElementIndex = parentElementIndex;
            this.parentNodePointerIndex = parentNodePointerIndex;
        }
    }
}
