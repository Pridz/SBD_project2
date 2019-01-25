using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class NodeBufor
    {
        BTreeRecord[] element;
        int index;
        int elementsSize;
        int elementsCapacity;
        int pointersCapacity;
        Node[] pointer;

        public int ElementsCapacity
        {
            get { return elementsCapacity; }
        }

        public int PointersCapacity
        {
            get { return pointersCapacity; }
        }

        public BTreeRecord getElement(int index)
        {
            if (index < elementsCapacity)
            {
                return element[index]; 
            }
            // TODO: throw exception
            return null;
        }

        public void setElement(BTreeRecord record, int index)
        {
            if (index < elementsCapacity)
            {
                element[index] = record;
            }
            // TODO: throw exception
        }

        public void addElement(BTreeRecord record)
        {
            if (elementsCapacity < elementsSize)
            {
                element[elementsCapacity] = record;
                elementsCapacity++;
            }
        }

        public void addPointerNode(Node node)
        {
            if (pointersCapacity < elementsSize + 1)
            {
                pointer[pointersCapacity] = node;
                pointersCapacity++;
            }
        }

        public Node getPointer(int index)
        {
            if (index >= pointersCapacity && index >= elementsSize + 1)
            {
                // TODO: throw an exception
            }
            return pointer[index];
        }

        public NodeBufor(int elementsSize)
        {
            this.elementsSize = elementsSize;
            element = new BTreeRecord[elementsSize];
            elementsCapacity = 0;
            pointer = new Node[elementsSize + 1];
            pointersCapacity = elementsSize + 1;
            index = 0;
        }

        public void addNodesElements(Node node)
        {
            for (int i = 0; i < node.KeyCapacity; i++)
            {
                addElement(node.getElement(i));                
            }
        }

        public void addPointerNodes(Node node)
        {
            int length = node.getPointersCapacity();
            for (int i = 0; i < length; i++)
            {
                addPointerNode(node.getSonNode(i));
            }
        }

        public int getPointerCapacity() { return elementsCapacity + 1; }

        public void setPointer(Node node, int index)
        {
            if (index < getPointerCapacity())
            {
                pointer[index] = node;
            }
        }

        public long changePointerToPositionInFile()
        {

        }
    }
}
