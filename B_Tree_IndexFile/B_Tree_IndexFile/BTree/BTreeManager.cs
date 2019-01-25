using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile.BTree
{
    class BTreeManager
    {
        BTree root;
        int maxSizeOfElementsInPage;
        int divider;

        public delegate bool isTrue();

        BTreeSearchResult findRecordInBtreePage(NodeParentRelation relation, int key)
        {
            Node son;
            for (int i = 0; i < maxSizeOfElementsInPage; i++)
            {
                if (relation.Son.getElement(i).key == key)
                {
                    return new BTreeSearchResult(relation.Son.getElement(i), relation);
                }
                if (relation.Son.getElement(i).key < key)
                {
                    son = relation.Son.getSonNode(i);
                    if (son == null)
                    {
                        return new BTreeSearchResult(relation);
                    }
                    return findRecordInBtreePage( new NodeParentRelation( relation.Son, relation.Son.getSonNode(i) ), key );
                }
            }
            return findRecordInBtreePage( new NodeParentRelation( relation.Son, relation.Son.getSonNode( maxSizeOfElementsInPage ) ), key );
        }

        BTreeSearchResult findRecordInBtreePage(BTreeSearchResult searchResult, int key)
        {
            Node son;
            for (int i = 0; i < maxSizeOfElementsInPage; i++)
            {
                if (searchResult.page.getElement(i).key == key)
                {
                    searchResult.element = searchResult.page.getElement(i);
                    searchResult.result = Result.Found;
                    return searchResult;
                }
                if (searchResult.page.getElement(i).key < key)
                {
                    son = searchResult.page.getSonNode(i);
                    if (son == null)
                    {
                        return  searchResult;
                    }
                    searchResult.setRelation(new NodeParentRelation(searchResult.page, searchResult.page.getSonNode(i)));
                    return findRecordInBtreePage(searchResult, key);
                }
            }
            searchResult.setRelation(new NodeParentRelation(searchResult.page, searchResult.page.getSonNode(maxSizeOfElementsInPage)));
            return findRecordInBtreePage(searchResult, key);
        }

        BTreeRecord findMaximumRecord(Node node)
        {
            int indexOfMaxRecord = node.KeyCapacity - 1;
            Node sonNode = node.getSonNode(node.getRightPointerIndex(indexOfMaxRecord));
            BTreeRecord maxRecord = node.getElement(indexOfMaxRecord);
            if (sonNode != null)
            {
                return findMaximumRecord(sonNode);
            }
            return maxRecord;
        }

        BTreeSearchResult findMaximumRecord(BTreeSearchResult searchResult)
        {
            int indexOfMaxRecord = searchResult.page.KeyCapacity - 1;
            Node sonNode = searchResult.page.getSonNode(searchResult.page.getRightPointerIndex(indexOfMaxRecord));
            BTreeRecord maxRecord = searchResult.page.getElement(indexOfMaxRecord);
            if (sonNode != null)
            {
                searchResult.setRelation(new NodeParentRelation(searchResult.page, sonNode));
                return findMaximumRecord(searchResult);
            }
            searchResult.element = maxRecord;
            return searchResult;
        }

        BTreeRecord findMinimumRecord(Node node)
        {
            int indexOfMinimumRecord = 0;
            Node sonNode = node.getSonNode(node.getLeftPointerIndex(indexOfMinimumRecord));
            BTreeRecord minRecord = node.getElement(indexOfMinimumRecord);
            if (sonNode != null)
            {
                return findMinimumRecord(sonNode);
            }
            return minRecord;
        }

        BTreeSearchResult findMinimumRecord(BTreeSearchResult searchResult)
        {
            int indexOfMinimumRecord = 0;
            Node sonNode = searchResult.page.getSonNode(searchResult.page.getLeftPointerIndex(indexOfMinimumRecord));
            BTreeRecord minRecord = searchResult.page.getElement(indexOfMinimumRecord);
            if (sonNode != null)
            {
                searchResult.setRelation(new NodeParentRelation(searchResult.page, sonNode));
                return findMinimumRecord(searchResult);
            }
            searchResult.element = minRecord;
            return searchResult;
        }

        int findPlaceToAdd(Node page, int key)
        {            
            for (int i = 0; i < page.KeyCapacity; i++)
            {
                if (key < page.getElement(i).key)
                {
                    return i;
                }
            }
            return page.KeyCapacity;
        }        

        void shiftRecordsToLeft(ref Node page, int startingIndex)
        {
            if (startingIndex <= 0)
            {
                // TODO: throw exception; illegal action!
            }
            for (int i = startingIndex; i < page.KeyCapacity; i++)
            {
                page.setElement(page.getElement(i), i - 1);                
            }
        }

        void shiftRecordsWithPointersToLeft(Node page, int startingIndex)
        {
            if (startingIndex <= 0)
            {
                // TODO: throw exception; illegal action!
            }
            for (int i = startingIndex; i < page.KeyCapacity; i++)
            {
                page.setElement(page.getElement(i), i - 1);
                page.setSonNode(page.getSonNode(i + 1), i);
            }
        }

        void shiftRecordsToRight(ref Node page, int fromIndex)
        {
            int lastRecordIndex = page.KeyCapacity - 1;
            for (int i = lastRecordIndex; i >= fromIndex; i--)
            {
                page.setElement(page.getElement(i), i + 1);
            }
        }

        Result add(Node page, BTreeRecord element)
        {            
            int index = findPlaceToAdd(page, element.key);
            if (index == page.KeyCapacity)
            {                
                page.addElement(element);                
                return Result.Added;
            }
            shiftRecordsToRight(ref page, index);
            page.setElement(element, index);
            page.KeyCapacity++;
            return Result.Added;
        }

        int getPointerIndex(NodeParentRelation relation)
        {
            int pointersSize = relation.Father.getPointersCapacity();
            for (int i = 0; i < pointersSize; i++)
            {
                if (relation.Father.getSonNode(i) == relation.Son)
                {
                    return i;
                }
            }
            // TODO: throw exception - program shouldn't get here
            return -1;
        }
        
        Result compensateWithRightSibling(NodeParentRelation relation, CompensationIndexes indexes)
        {
            Node targetSibling = relation.Father.getSonNode(indexes.parentNodePointerIndex);
            NodeBufor temporaryBufor;
            int buforSize;
            int buforsMiddlePoint;
            if (targetSibling.isFull())
            {
                return Result.NotCompensated;
            }
            buforSize = relation.Son.KeyCapacity + 1 + targetSibling.KeyCapacity;
            temporaryBufor = new NodeBufor(buforSize);
            temporaryBufor.addNodesElements(relation.Son);
            relation.Son.nullElements();
            relation.Son.KeyCapacity = 0;
            temporaryBufor.addElement(relation.Father.getElement(indexes.parentElementIndex));
            temporaryBufor.addNodesElements(targetSibling);
            targetSibling.nullElements();
            targetSibling.KeyCapacity = 0;
            buforsMiddlePoint = (temporaryBufor.ElementsCapacity -1)/ 2 ;
            relation.Father.setElement(temporaryBufor.getElement(buforsMiddlePoint), indexes.parentElementIndex);
            for (int i = 0; i < buforsMiddlePoint; i++)
            {
                relation.Son.addElement(temporaryBufor.getElement(i));
            }
            for (int i = buforsMiddlePoint + 1; i < temporaryBufor.ElementsCapacity; i++)
            {
                targetSibling.addElement(temporaryBufor.getElement(i));
            }
            return Result.Compensated;
        }

        Result compensateWithLeftSibling(NodeParentRelation relation, CompensationIndexes indexes)
        {
            Node targetSibling = relation.Father.getSonNode(indexes.parentNodePointerIndex);
            NodeBufor temporaryBufor;
            int buforSize;
            int buforsMiddlePoint;            
            if (targetSibling.isFull())
            {
                return Result.NotCompensated;
            }
            buforSize = relation.Son.KeyCapacity + 1 + targetSibling.KeyCapacity;
            temporaryBufor = new NodeBufor(buforSize);
            temporaryBufor.addNodesElements(targetSibling);
            targetSibling.nullElements();
            targetSibling.KeyCapacity = 0;
            temporaryBufor.addElement(relation.Father.getElement(indexes.parentElementIndex));
            temporaryBufor.addNodesElements(relation.Son);
            relation.Son.nullElements();
            relation.Son.KeyCapacity = 0;
            buforsMiddlePoint = (temporaryBufor.ElementsCapacity - 1) / 2 ;
            relation.Father.setElement(temporaryBufor.getElement(buforsMiddlePoint), indexes.parentElementIndex);
            for (int i = 0; i < buforsMiddlePoint; i++)
            {
                targetSibling.addElement(temporaryBufor.getElement(i));
            }
            for (int i = buforsMiddlePoint + 1; i < temporaryBufor.ElementsCapacity; i++)
            {
                relation.Son.addElement(temporaryBufor.getElement(i));
            }
            return Result.Compensated;
        }

        public Result compensate(NodeParentRelation relation)
        {
            int parentPointerCapacity = relation.Father.getPointersCapacity();
            int sonPageIndex = getPointerIndex(relation);
            const int firstIndex = 0;
            int lastIndex = parentPointerCapacity - 1;            
            Node leftSibling, rightSibling;
            int leftSiblingIndex;
            int rightSiblingIndex;            
            if (sonPageIndex == firstIndex)
            {
                return compensateWithLeftSibling(relation, new CompensationIndexes(firstIndex, firstIndex + 1));                
            }
            if (sonPageIndex == lastIndex)
            {
                return compensateWithRightSibling(relation, new CompensationIndexes(lastIndex - 1, lastIndex - 1));
            }
            leftSiblingIndex = sonPageIndex - 1;
            rightSiblingIndex = sonPageIndex + 1;
            leftSibling = relation.Father.getSonNode(leftSiblingIndex);
            rightSibling = relation.Father.getSonNode(rightSiblingIndex);
            if (leftSibling.isFull())
            {
                return compensateWithRightSibling(relation, new CompensationIndexes(sonPageIndex, rightSiblingIndex));
            }
            else if (rightSibling.isFull())
            {
                return compensateWithLeftSibling(relation, new CompensationIndexes(leftSiblingIndex, leftSiblingIndex));
            }
            else
            {
                return Result.NotCompensated;
            }
        }

        void rootSplit(BTreeSearchResult searchResult)
        {
            const int singleRecord = 1;
            List<Node> pagePath = searchResult.pageSequence;
            const int lastItemIndex = 0;            
            NodeBufor midRecord = new NodeBufor(singleRecord);
            Node newPage = new Node();
            Node newRoot = new Node();            
            Node page = pagePath.ElementAt(lastItemIndex);
            int middleRecordIndex;
            if (page.getElement((page.KeyCapacity - 1) / 2).key < searchResult.element.key)
            {
                middleRecordIndex = (page.KeyCapacity - 1) / 2;
            }
            else
            {
                middleRecordIndex = page.KeyCapacity / 2;
            }
            midRecord.addElement(page.getElement(middleRecordIndex));
            midRecord.setPointer(newPage, 0);
            for (int i = middleRecordIndex + 1; i < page.KeyCapacity; i++)
            {
                newPage.addElement(page.getElement(i));
            }
            page.nullElementsFromTo(middleRecordIndex + 1, page.KeyCapacity);
            newRoot.setSonNode(page, 0);
            newRoot.insertElement(midRecord);            
            pagePath.RemoveAt(pagePath.Capacity - 1);
            root.Root = newRoot;
        }

        void rootSplit(int key)
        {
            const int singleRecord = 1;                        
            NodeBufor midRecord = new NodeBufor(singleRecord);
            Node newPage = new Node();
            Node newRoot = new Node();
            Node page = root.Root;
            int middleRecordIndex;
            if (page.getElement((page.KeyCapacity - 1) / 2).key < key)
            {
                middleRecordIndex = (page.KeyCapacity - 1) / 2;
            }
            else
            {
                middleRecordIndex = page.KeyCapacity / 2;
            }
            midRecord.addElement(page.getElement(middleRecordIndex));
            midRecord.setPointer(newPage, 0);
            for (int i = middleRecordIndex + 1; i < page.KeyCapacity; i++)
            {
                newPage.addElement(page.getElement(i));
            }
            page.nullElementsFromTo(middleRecordIndex + 1, page.KeyCapacity);
            newRoot.setSonNode(page, 0);
            newRoot.insertElement(midRecord);            
            root.Root = newRoot;
        }

        public void split(BTreeSearchResult searchResult)
        {
            const int singleRecord = 1;
            List < Node > pagePath = searchResult.pageSequence;
            int lastItemIndex;
            int penultimateItemIndex;
            NodeBufor midRecord = new NodeBufor(singleRecord);
            Node newPage = new Node();
            Node parent;
            Node page;
            int middleRecordIndex;
            if (searchResult.pageSequence.Count < 2)
            {               
                rootSplit(searchResult);
                return;
            }
            lastItemIndex = pagePath.Count - 1;
            penultimateItemIndex = pagePath.Count - 2;                
            page = searchResult.pageSequence.ElementAt(lastItemIndex);
            parent = pagePath.ElementAt(penultimateItemIndex);
            if (page.getElement((page.KeyCapacity - 1) / 2).key < searchResult.element.key)
            {
                middleRecordIndex = (page.KeyCapacity - 1) / 2;
            }
            else
            {
                middleRecordIndex = page.KeyCapacity / 2;
            }
            midRecord.addElement(page.getElement(middleRecordIndex));
            midRecord.setPointer(newPage, 0);
            newPage.setSonNode(page.getSonNode(middleRecordIndex + 1), 0);
            for (int i = middleRecordIndex+1, j = 1; i < page.KeyCapacity; i++, j++)
            {
                newPage.addElement(page.getElement(i));
                newPage.setSonNode(page.getSonNode(i + 1), j);
            }
            page.nullElementsFromTo(middleRecordIndex + 1, page.KeyCapacity);
            parent.insertElement(midRecord);
            pagePath.RemoveAt(pagePath.Capacity - 1);
            if (parent.isOverextended())
            {
                split(searchResult);
            }
        }

        public void split(NodeParentRelation relation, int key)
        {
            const int singleRecord = 1;                        
            NodeBufor midRecord = new NodeBufor(singleRecord);
            Node newPage = new Node();
            Node parent;
            Node page;
            int middleRecordIndex;
            if (relation.Son == root.Root)
            {
                rootSplit(key);
                return;
            }            
            page = relation.Son;
            parent = relation.Father;
            if (relation.Son.getElement((page.KeyCapacity - 1) / 2).key < key)
            {
                middleRecordIndex = (page.KeyCapacity - 1) / 2;
            }
            else
            {
                middleRecordIndex = page.KeyCapacity / 2;
            }
            midRecord.addElement(page.getElement(middleRecordIndex));
            midRecord.setPointer(newPage, 0);
            newPage.setSonNode(page.getSonNode(middleRecordIndex + 1), 0);
            for (int i = middleRecordIndex + 1, j = 1; i < page.KeyCapacity; i++, j++)
            {
                newPage.addElement(page.getElement(i));
                newPage.setSonNode(page.getSonNode(i + 1), j);
            }
            page.nullElementsFromTo(middleRecordIndex + 1, page.KeyCapacity);
            parent.insertElement(midRecord);            
        }

        public Result insert(BTreeRecord element)
        {
            int length;
            NodeParentRelation rootRelation = new NodeParentRelation( null, root.Root );
            BTreeSearchResult searchResult = new BTreeSearchResult();
            searchResult.setRelation(rootRelation);
            searchResult = findRecordInBtreePage( searchResult, element.key );
            if (searchResult.parent == null)
            {
                if (searchResult.page.isOverextended())
                {
                    rootSplit(element.key);
                }
                return add(searchResult.page, element);                
            }
            NodeParentRelation relation = new NodeParentRelation(searchResult.parent, searchResult.page);
            if (searchResult.result == Result.Found)
            {
                return Result.AlreadyExist;
            }
            if (!searchResult.page.isFull())
            {
                return add(searchResult.page,element);
            }
            if (compensate(relation) == Result.Compensated)
            {
                return add(searchResult.page, element);
            }
            split(searchResult);
            add(relation.Son, element);
            searchResult.pageSequence.RemoveAt(searchResult.pageSequence.Count - 1);
            if (searchResult.pageSequence.Count < 2)
            {
                relation = new NodeParentRelation(null, searchResult.pageSequence[searchResult.pageSequence.Count - 1]);
                if (relation.Son.isOverextended())
                {
                    split(relation, element.key);
                }
                return Result.Added;
            }
            relation = new NodeParentRelation(searchResult.pageSequence[searchResult.pageSequence.Count - 2], searchResult.pageSequence[searchResult.pageSequence.Count - 1]);
            length = searchResult.pageSequence.Count;
            for (int i = 0; i < length; i++)
            {                
                if (relation.Son.isOverextended())
                {
                    if (compensate(relation) == Result.Compensated)
                    {
                        return Result.Added;
                    }
                    split(relation, element.key);
                }
                if (searchResult.pageSequence.Count < 2)
                {
                    relation = new NodeParentRelation(null, searchResult.pageSequence[searchResult.pageSequence.Count - 1]);
                    if (relation.Son.isOverextended())
                    {
                        split(relation, element.key);
                    }
                    return Result.Added;
                }
                searchResult.pageSequence.RemoveAt(searchResult.pageSequence.Count - 1);
                relation = new NodeParentRelation(searchResult.pageSequence[searchResult.pageSequence.Count - 2], searchResult.pageSequence[searchResult.pageSequence.Count - 1]);
            }
            return Result.Added;
        }

        Result compensateDeletionWithRightSibling(NodeParentRelation relation, CompensationIndexes indexes)
        {
            Node targetSibling = relation.Father.getSonNode(indexes.parentNodePointerIndex);
            NodeBufor temporaryBufor;
            int buforSize;
            int buforsMiddlePoint;
            if (targetSibling.isMinimal() || targetSibling.isLack())
            {
                return Result.NotCompensated;
            }
            buforSize = relation.Son.KeyCapacity + 1 + targetSibling.KeyCapacity;
            temporaryBufor = new NodeBufor(buforSize);
            temporaryBufor.addNodesElements(relation.Son);
            relation.Son.nullElements();
            relation.Son.KeyCapacity = 0;
            temporaryBufor.addElement(relation.Father.getElement(indexes.parentElementIndex));
            temporaryBufor.addNodesElements(targetSibling);
            targetSibling.nullElements();
            targetSibling.KeyCapacity = 0;
            buforsMiddlePoint = (temporaryBufor.ElementsCapacity - 1) / 2;
            relation.Father.setElement(temporaryBufor.getElement(buforsMiddlePoint), indexes.parentElementIndex);
            for (int i = 0; i < buforsMiddlePoint; i++)
            {
                relation.Son.addElement(temporaryBufor.getElement(i));
            }
            for (int i = buforsMiddlePoint + 1; i < temporaryBufor.ElementsCapacity; i++)
            {
                targetSibling.addElement(temporaryBufor.getElement(i));
            }
            return Result.Compensated;
        }

        Result compensateDeletionWithLeftSibling(NodeParentRelation relation, CompensationIndexes indexes)
        {
            Node targetSibling = relation.Father.getSonNode(indexes.parentNodePointerIndex);
            NodeBufor temporaryBufor;
            int buforSize;
            int buforsMiddlePoint;
            if (targetSibling.isMinimal() || targetSibling.isLack())
            {
                return Result.NotCompensated;
            }
            buforSize = relation.Son.KeyCapacity + 1 + targetSibling.KeyCapacity;
            temporaryBufor = new NodeBufor(buforSize);
            temporaryBufor.addNodesElements(targetSibling);
            targetSibling.nullElements();
            targetSibling.KeyCapacity = 0;
            temporaryBufor.addElement(relation.Father.getElement(indexes.parentElementIndex));
            temporaryBufor.addNodesElements(relation.Son);
            relation.Son.nullElements();
            relation.Son.KeyCapacity = 0;
            buforsMiddlePoint = (temporaryBufor.ElementsCapacity - 1) / 2;
            relation.Father.setElement(temporaryBufor.getElement(buforsMiddlePoint), indexes.parentElementIndex);
            for (int i = 0; i < buforsMiddlePoint; i++)
            {
                targetSibling.addElement(temporaryBufor.getElement(i));
            }
            for (int i = buforsMiddlePoint + 1; i < temporaryBufor.ElementsCapacity; i++)
            {
                relation.Son.addElement(temporaryBufor.getElement(i));
            }
            return Result.Compensated;
        }

        public Result compensateDeletion(NodeParentRelation relation)
        {
            int parentPointerCapacity = relation.Father.getPointersCapacity();
            int sonPageIndex = getPointerIndex(relation);
            const int firstIndex = 0;
            int lastIndex = parentPointerCapacity - 1;
            Node leftSibling, rightSibling;
            int leftSiblingIndex;
            int rightSiblingIndex;
            if (sonPageIndex == firstIndex)
            {
                return compensateDeletionWithLeftSibling(relation, new CompensationIndexes(firstIndex, firstIndex + 1));
            }
            if (sonPageIndex == lastIndex)
            {
                return compensateDeletionWithRightSibling(relation, new CompensationIndexes(lastIndex - 1, lastIndex - 1));
            }
            leftSiblingIndex = sonPageIndex - 1;
            rightSiblingIndex = sonPageIndex + 1;
            leftSibling = relation.Father.getSonNode(leftSiblingIndex);
            rightSibling = relation.Father.getSonNode(rightSiblingIndex);
            if (leftSibling.isMinimal() || leftSibling.isLack())
            {
                return compensateDeletionWithRightSibling(relation, new CompensationIndexes(sonPageIndex, rightSiblingIndex));
            }
            else if (rightSibling.isMinimal() || rightSibling.isLack())
            {
                return compensateDeletionWithLeftSibling(relation, new CompensationIndexes(leftSiblingIndex, leftSiblingIndex));
            }
            else
            {
                return Result.NotCompensated;
            }
        }

        void mergeWithRightSibling(NodeParentRelation relation)
        {
            int parentsPageIndex = getPointerIndex(relation);
            int onePointerPosition = 1;
            BTreeRecord parentMiddlePointersRecord = relation.Father.getElement(parentsPageIndex);
            Node page = relation.Son;
            Node parent = relation.Father;
            Node targetSibling = relation.Father.getSonNode(parentsPageIndex + onePointerPosition);            
            NodeBufor temporaryPage = new NodeBufor(maxSizeOfElementsInPage);
            temporaryPage.addNodesElements(page);
            temporaryPage.addPointerNodes(page);      
            temporaryPage.addElement(parentMiddlePointersRecord);
            temporaryPage.addNodesElements(targetSibling);
            temporaryPage.addPointerNodes(targetSibling);
            page.nullElements();
            for (int i = 0; i < temporaryPage.ElementsCapacity; i++)
            {
                page.addElement(temporaryPage.getElement(i));
                page.setSonNode(temporaryPage.getPointer(i), i);
            }
            shiftRecordsWithPointersToLeft(parent, parentsPageIndex + 1);
        }

        void mergeWithLeftSibling(NodeParentRelation relation)
        {
            int parentsPageIndex = getPointerIndex(relation);
            int onePointerPosition = 1;
            BTreeRecord parentMiddlePointersRecord = relation.Father.getElement(parentsPageIndex - onePointerPosition);
            Node page = relation.Son;
            Node parent = relation.Father;
            Node targetSibling = relation.Father.getSonNode(parentsPageIndex - onePointerPosition);
            NodeBufor temporaryPage = new NodeBufor(maxSizeOfElementsInPage);
            temporaryPage.addNodesElements(targetSibling);
            temporaryPage.addPointerNodes(targetSibling);
            temporaryPage.addElement(parentMiddlePointersRecord);
            temporaryPage.addNodesElements(page);
            temporaryPage.addPointerNodes(page);
            targetSibling.nullElements();
            for (int i = 0; i < temporaryPage.ElementsCapacity; i++)
            {
                targetSibling.addElement(temporaryPage.getElement(i));
                targetSibling.setSonNode(temporaryPage.getPointer(i), i);
            }
            shiftRecordsWithPointersToLeft(parent, parentsPageIndex + 1);
        }

        void rootMerge(BTreeSearchResult searchResult)
        {
            const int onePointerPosition = 1;
            Node root = this.root.Root;
            Node page = searchResult.pageSequence[searchResult.pageSequence.Count - 1];
            Node parent = root;
            const int singleElementInNode = 1;
            NodeParentRelation relation;
            int parentsPageIndex;
            int firstIndex = 0;
            int lastIndex = root.getPointersCapacity() - 1;
            int leftSiblingIndex;
            int rightSiblingIndex;
            Node leftSibling;
            Node rightSibling;
            if (root.KeyCapacity == singleElementInNode)
            {

            }
            relation = new NodeParentRelation(root, page);
            parentsPageIndex = getPointerIndex(relation);
            if (parentsPageIndex == firstIndex)
            {
                mergeWithRightSibling(relation);
                return;
            }
            if (parentsPageIndex == lastIndex)
            {
                mergeWithLeftSibling(relation);
                return;
            }
            leftSiblingIndex = parentsPageIndex - onePointerPosition;
            rightSiblingIndex = parentsPageIndex + onePointerPosition;
            leftSibling = searchResult.parent.getSonNode(leftSiblingIndex);
            rightSibling = searchResult.parent.getSonNode(rightSiblingIndex);
            if (leftSibling.isMinimal())
            {
                mergeWithLeftSibling(relation);                
                return;
            }
            mergeWithRightSibling(relation);            
            return;

        }

        public void merge(BTreeSearchResult searchResult)
        {
            List<Node> pagePath = searchResult.pageSequence;            
            NodeBufor temporaryPage = new NodeBufor(maxSizeOfElementsInPage);
            int onePointerPosition = 1;
            NodeParentRelation relation;
            int parentsPageIndex;
            int leftSiblingIndex;
            int rightSiblingIndex;
            int firstIndex = 0;
            int lastIndex = searchResult.parent.getPointersCapacity() - onePointerPosition;
            Node leftSibling;
            Node rightSibling;
            if (pagePath.Count == 2)
            {
                rootMerge(searchResult);
                return;
            }
            relation = new NodeParentRelation(pagePath[pagePath.Count - 2], pagePath[pagePath.Count - 1]);
            parentsPageIndex = getPointerIndex(relation);

            if (parentsPageIndex == firstIndex)
            {
                mergeWithRightSibling(relation);
                pagePath.RemoveAt(pagePath.Capacity - 1);
                merge(searchResult);
                return;
            }
            if (parentsPageIndex == lastIndex)
            {
                mergeWithLeftSibling(relation);
                pagePath.RemoveAt(pagePath.Capacity - 1);
                merge(searchResult);
                return;
            }
            leftSiblingIndex = parentsPageIndex - onePointerPosition;
            rightSiblingIndex = parentsPageIndex + onePointerPosition;
            leftSibling = searchResult.parent.getSonNode(leftSiblingIndex);
            rightSibling = searchResult.parent.getSonNode(rightSiblingIndex);
            if (leftSibling.isMinimal())
            {
                mergeWithLeftSibling(relation);
                pagePath.RemoveAt(pagePath.Capacity - 1);
                merge(searchResult);
                return;
            }
            mergeWithRightSibling(relation);
            pagePath.RemoveAt(pagePath.Capacity - 1);
            merge(searchResult);
            return;
        }

        public Result delete(BTreeRecord element)
        {
            NodeParentRelation rootRelation = new NodeParentRelation(null, root.Root);
            BTreeSearchResult leafSearchResult = new BTreeSearchResult();
            int elementIndex;
            int foundElementsPageIndex;
            Node page;
            BTreeRecord recordToSwap;
            BTreeSearchResult searchResult = new BTreeSearchResult();
            searchResult.setRelation(rootRelation);
            searchResult = findRecordInBtreePage(searchResult, element.key);
            NodeParentRelation relation = new NodeParentRelation(searchResult.parent, searchResult.page);
            if (searchResult.result == Result.NotFound)
            {
                return Result.NotFound;
            }
            if (!relation.Son.isLeaf())
            {
                elementIndex = relation.Son.getElementIndex(element);
                foundElementsPageIndex = searchResult.pageSequence.Count - 1;
                relation = new NodeParentRelation(relation.Son,relation.Son.getSonNode(relation.Son.getLeftPointerIndex(elementIndex)));
                searchResult.setRelation(relation);
                searchResult = findMaximumRecord(searchResult);
                page = searchResult.page;
                recordToSwap = searchResult.element;
                searchResult.pageSequence[foundElementsPageIndex].setElement(recordToSwap, elementIndex);
                page.nullElement(page.getElementIndex(recordToSwap));
                recordToSwap = null;
                searchResult.element = null;
                if (!page.isLack())
                {
                    return Result.Deleted;
                }
                if (compensateDeletion(relation) == Result.Compensated)
                {
                    ;
                    // remove      
                    return Result.Deleted;                            
                }
                merge();               
            }
            page = relation.Son;
            elementIndex = page.getElementIndex(element);
            page.nullElement(elementIndex);
            if (page.isLack())
            {
                if (compensateDeletion(relation) == Result.Compensated)
                {
                    // remove
                    ;
                    return Result.Deleted;
                }
                merge();
            }
            return Result.Deleted;
        }
    }
}
