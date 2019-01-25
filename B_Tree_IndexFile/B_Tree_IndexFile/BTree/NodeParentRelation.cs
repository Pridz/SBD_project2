using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class NodeParentRelation
    {
        Node father;
        Node son;

        public Node Father
        {
            get { return father; }
        }

        public Node Son
        {
            get { return son; }
        }

        public NodeParentRelation(Node father, Node son)
        {
            this.father = father;
            this.son = son;
        }
    }
}
