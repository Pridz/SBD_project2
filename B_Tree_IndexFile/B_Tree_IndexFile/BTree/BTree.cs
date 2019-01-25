using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class BTree
    {
        Node root;
        int degree;

        public BTree(int degree)
        {
            this.degree = degree;
            Node.ElementSize = 2*degree;            
            root = new Node();
        }

        public int Degree
        {
            get { return degree; }
        }

        public Node Root
        {
            get { return root; }
            set { root = value; }
        }
    }
}
