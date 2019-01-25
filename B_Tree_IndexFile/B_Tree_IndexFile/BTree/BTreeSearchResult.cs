using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B_Tree_IndexFile;

namespace B_Tree_IndexFile.BTree
{   

    class BTreeSearchResult
    {
        public Result result { get; set; }
        public BTreeRecord element { get; set; }
        public Node page { get; set; }
        public Node parent { get; set; }
        public List<Node> pageSequence;        

        public BTreeSearchResult()
        {
            result = Result.NotFound;
            element = null;
            page = null;
            parent = null;
            pageSequence = new List<Node>();
        }

        public BTreeSearchResult(NodeParentRelation relation)
        {
            result = Result.NotFound;
            parent = relation.Father;
            page = relation.Son;
        }

        public BTreeSearchResult(BTreeRecord element, NodeParentRelation relation)
        {
            result = Result.Found;
            this.element = element;
            parent = relation.Father;
            page = relation.Son;            
        }

        public void setRelation(NodeParentRelation relation)
        {
            parent = relation.Father;
            page = relation.Son;
            pageSequence.Add(page);
        }

        public int getPageIndexFromSearchingPath(Node page)
        {
            for (int i = 0; i < pageSequence.Count; i++)
            {
                if (pageSequence[i].Equals(page))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
