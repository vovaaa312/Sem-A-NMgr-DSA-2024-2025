using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem_A_NMgr_DSA_2024_2025.Graph
{
    public class PriorityQueue<T>
    {
        private List<T> data;
        private IComparer<T> comparer;


        public PriorityQueue(IComparer<T> comparer)
        {
            this.data = new List<T>();
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        //add item to queue
        public void Enqueue(T item)
        {
            BubbleUp(item);
        }

        //move item up
        private void BubbleUp(T item) { 
                  data.Add(item);
            int currentIndex = Count - 1;
            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;
                if (comparer.Compare(data[currentIndex], data[parentIndex]) >= 0) break;

                T tmp = data[currentIndex]; data[currentIndex] = data[parentIndex]; data[parentIndex] = tmp;

                Swap(currentIndex, parentIndex);
                currentIndex = parentIndex;
            }

        }

        private void Swap(int i, int j)
        {
            T temp = data[i];
            data[i] = data[j];
            data[j] = temp;
        }


        // Extract the element with the highest priority (lowest value)
        public T Dequeue()
        {
            int lastIndex = Count - 1;
            T frontItem = data[0];
            data[0] = data[lastIndex];
            data.RemoveAt(lastIndex);

            --lastIndex;
            int currentIndex = 0;
            while (true)
            {
                int leftChild = currentIndex * 2 + 1;
                if (leftChild > lastIndex) 
                    break;

                int rightChild = leftChild + 1;
                if (rightChild <= lastIndex && comparer.Compare(data[rightChild], data[leftChild]) < 0)
                    leftChild = rightChild;

                if (comparer.Compare(data[currentIndex], data[leftChild]) <= 0) 
                    break;

                T tmp = data[currentIndex]; 
                data[currentIndex] = data[leftChild]; 
                data[leftChild] = tmp;

                Swap(currentIndex, leftChild);
                currentIndex = leftChild;
            }
            return frontItem;
        }

        public int Count
        {
            get { return data.Count; }
        }
    }
}
