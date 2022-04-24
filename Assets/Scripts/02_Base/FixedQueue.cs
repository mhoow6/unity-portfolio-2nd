using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedQueue<T>
{
    Queue<T> m_Queue;
    int m_Size = 0;

    public int Count => m_Queue.Count;

    public FixedQueue(int size)
    {
        m_Size = size;
        m_Queue = new Queue<T>(size);
    }

    public void Enqueue(T item)
    {
        if (m_Queue.Count <= m_Size)
            m_Queue.Enqueue(item);
    }

    public T Dequeue()
    {
        return m_Queue.Dequeue();
    }

    public T Peek()
    {
        return m_Queue.Peek();
    }
}
