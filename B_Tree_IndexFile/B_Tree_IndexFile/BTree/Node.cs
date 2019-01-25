using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class Node
    {
        BTreeRecord[] element;
        Node[] pointer;
        /// <summary>
        /// Maximal size of records in B Tree node.
        /// </summary>
        static int elementSize = 0;
        static int degree;
        static bool isElemtSizeSet = false;
        int elementCapacity;
        
        public int KeyCapacity
        {
            get { return elementCapacity; }
            set { elementCapacity = value; }
        }

        public static int ElementSize
        {
            get { return elementSize; }   
            set
            {
                int additionalSpace = 1;
                if (isElemtSizeSet == false)
                {
                    elementSize = value + additionalSpace;
                    isElemtSizeSet = true;
                }
                else
                {
                    System.Console.WriteLine("Cannot set Node.elementSize: it's already set!");
                }
            }     
        }       

        public Node()
        {
            int additionalSpace = 1;

            elementCapacity = 0;
            if (elementSize == 0)
            {
                System.Console.Out.WriteLine("Node: keySize is not set! Can not initialize Node object!");
            }
            degree = (elementSize - additionalSpace)/2;
            element = new BTreeRecord[elementSize];
            pointer = new Node[elementSize];
        }

        int maxElementsCapacity() { return 2 * degree; }

        int maxPointersCapacity() { return 2 * degree + 1; }

        public int getPointersCapacity() { return elementCapacity + 1; }

        public BTreeRecord getElement(int index)
        {
            if (index < elementCapacity)
            {
                return element[index];
            }
            Console.Out.WriteLine("Node.getElement: calling element out of array's size!");
            return null;
        }

        public int getElementIndex(BTreeRecord record)
        {
            for (int i = 0; i < elementCapacity; i++)
            {
                if (record == element[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public void setElement(BTreeRecord record, int index)
        {
            if (index < elementCapacity)
            {
                element[index] = record;
            }
            Console.Out.WriteLine("Node.setElement: calling element out of array's size!");
        }

        public void addElement(BTreeRecord record)
        {
            if (elementCapacity < maxElementsCapacity())
            {
                element[elementCapacity] = record;
                elementCapacity++;
            }            
        }

        void moveToLeftElementsByOne(int fromIndex)
        {
            for (int i = fromIndex; i < elementCapacity; i++)
            {
                element[i] = element[i+1];
                pointer[i + 1] = pointer[i + 2];
            }
            elementCapacity--;
        }

        void moveToRightElementsByOne(int fromIndex)
        {
            for (int i = elementCapacity; i >= fromIndex; i--)
            {
                element[i + 1] = element[i];
                pointer[i + 2] = pointer[i + 1];
            }
            elementCapacity++;
        }

        public void insertElement(NodeBufor oneRecord)
        {
            if (elementCapacity > maxElementsCapacity() )
            {
                // TODO: throw exception
            }
            int indexToInsert = 0;            
            for (int i = 0; i < elementCapacity; i++)
            {
                if (oneRecord.getElement(0).key < element[i].key)
                {
                    indexToInsert = i;
                    break;
                }
            }
            moveToRightElementsByOne(indexToInsert);
            element[indexToInsert] = oneRecord.getElement(0);
            pointer[indexToInsert + 1] = oneRecord.getPointer(0);
        }

        public Node getSonNode(int index)
        {
            if (index <= elementCapacity)
            {
                return pointer[index];
            }
            Console.Out.WriteLine("Node.getSonNode: calling element out of array's size!");
            return null;
        }

        public void setSonNode(Node node, int index)
        {
            if (index <= elementCapacity)
            {
                pointer[index] = node;
            }
            Console.Out.WriteLine("Node.setSonNode: calling element out of array's size!");
        }

        public int getLeftPointerIndex(int elementIndex)
        {
            if (elementIndex < elementCapacity)
            {
                return elementIndex;
            }
            return -1;
        }

        public int getRightPointerIndex(int elemenetIndex)
        {
            if (elemenetIndex < elementCapacity)
            {
                return elemenetIndex + 1;
            }
            return -1;
        }

        public bool isFull() { return elementCapacity == maxElementsCapacity(); }

        public bool isOverextended() { return elementCapacity > maxElementsCapacity(); }

        public bool isLeaf()
        {            
            int length = getPointersCapacity();
            for (int i = 0; i < length; i++)
            {
                if (pointer[i] != null )
                {
                    return false;
                }
            }
            return true;
        }

        public bool isMinimal() { return elementCapacity == degree; }

        public bool isLack() { return elementCapacity < degree; }
        
        public void nullElement(int i)
        {
            int lastRecordIndex = elementCapacity - 1;
            if (i >= elementCapacity)
            {
                // TODO: throw exception
            }
            setElement(null, i);
            if (i < lastRecordIndex)
            {
                moveToLeftElementsByOne(i);
            }
        }

        public void nullElements()
        {
            for (int i = 0; i < elementCapacity; i++)
            {                
                setElement(null, i);
            }
        }

        public void nullElementsFromTo(int from, int to)
        {
            int length = elementCapacity;
            if (to < elementCapacity)
            {
                length = to;
            }
            for (int i = from; i < length; i++)
            {
                setElement(null, i);
            }
        }
    }
}
