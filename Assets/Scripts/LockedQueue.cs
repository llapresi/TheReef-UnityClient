using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
 * Creates a C# queue with thread locking so two threads can't use it at the same time.
 * I'm pretty sure that the reason we're getting those errors when doing the queue stuff in Update() is that
 * the Socket thread is trying to add to it while Update() on main thread is removing stuff
 * and that causes threads to have a fit. I dunno, I'm not a threading expert and this probably
 * isn't the best way to do this but I think at the very least, it'll get rid of the errors
 * - Luke Lapresi
*/
public class ThreadLockedQueue<T>
{
    Queue<T> queue;
    private readonly object queueLock;

    public ThreadLockedQueue()
    {
        queue = new Queue<T>();
        queueLock = new object();
    }

    public void Enqueue(T toQueue)
    {
        lock(queueLock)
        {
            queue.Enqueue(toQueue);
        }
    }

    public T Dequeue()
    {
        lock (queueLock)
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }

            else
            {
                return default(T);
            }
        }
    }

    public int Count()
    {
        lock(queueLock)
        {
            return queue.Count;
        }
    }
}