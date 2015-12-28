using System;
using System.Collections.Generic;

namespace PriorityQueue
{
  /// <summary>
  /// A node in the Binary Tree used by <see cref="PriorityQueue"/>
  /// </summary>
  /// <typeparam name="T">The type of the value being stored in the queue</typeparam>
  class PQNode<T> : IDisposable
  {
    public PQNode<T> lesser;
    public PQNode<T> greater;

    private int _priority;
    private LinkedList<T> _queue;

    #region Properties

    /// <summary>
    /// THe priority value assigned to this node
    /// </summary>
    public int priority
    {
      get { return _priority; }
    }

    /// <summary>
    /// The total number of queued items at this priority level
    /// </summary>
    public int QueueCount
    {
      get { return _queue.Count; }
    }

    /// <summary>
    /// The total number of queued items in this node and its children
    /// </summary>
    public int Count
    {
      get
      {
        int count = QueueCount;

        if (lesser != null)
        {
          count += lesser.Count;
        }

        if (greater != null)
        {
          count += greater.Count;
        }

        return count;
      }
    }
    #endregion

    public PQNode(int priority)
    {
      lesser = null;
      greater = null;

      _priority = priority;

      _queue = new LinkedList<T>();
    }

    /// <summary>
    /// Adds a value to this Node's queue
    /// </summary>
    /// <param name="value">The value to add to this Node's Queue</param>
    public void Enqueue(T value)
    {
      _queue.AddLast(value);
    }

    /// <summary>
    /// Retrieves and removes the value at the front of this Node's LinkedList
    /// </summary>
    /// <returns>The value at the front of this Node's queue</returns>
    public T DequeueMin()
    {
      if (_queue.First != null)
      {
        T item = _queue.First.Value;
        _queue.RemoveFirst();
        return item;
      }
      else
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }
    }

    /// <summary>
    /// Retrieves and removes the value at the back of this Node's LinkedList
    /// </summary>
    /// <returns>The value at the back of this Node's queue</returns>
    public T DequeueMax()
    {
      if (_queue.Last != null)
      {
        T item = _queue.Last.Value;
        _queue.RemoveLast();
        return item;
      }
      else
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }
    }

    /// <summary>
    /// Retrieves the value at the front of this Node's LinkedList without removing it
    /// </summary>
    /// <returns>The value at the front of this Node's queue</returns>
    public T PeekMin()
    {
      if (_queue.First != null)
      {
        return _queue.First.Value;
      }
      else
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to view values.");
      }
    }

    /// <summary>
    /// Retrieves the value at the back of this Node's LinkedList without removing it
    /// </summary>
    /// <returns>The value at the back of this Node's queue</returns>
    public T PeekMax()
    {
      if (_queue.Last != null)
      {
        return _queue.Last.Value;
      }
      else
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to view values.");
      }
    }

    /// <summary>
    /// Recursively searches through this node and its children to locate the node with the assigned priority value. 
    /// If no node with the specified priority is found, one is created and added to the Tree.
    /// </summary>
    /// <param name="priority">The priority of the node that is being sought</param>
    /// <returns>The instance of PQNode that meets the priority requirements. If no valid node is found one is created.</returns>
    public PQNode<T> GetNode(int priority)
    {
      if (priority == this.priority)
      {
        return this;
      }
      else if (priority < this.priority)
      {
        if (lesser == null)
        {
          lesser = new PQNode<T>(priority);
          return lesser;
        }

        return lesser.GetNode(priority);
      }
      else
      {
        if (greater == null)
        {
          greater = new PQNode<T>(priority);
          return greater;
        }

        return greater.GetNode(priority);
      }
    }

    /// <summary>
    /// Compiles the values returned by the ToString method of the values within the queue owned by this node.
    /// </summary>
    /// <returns>A comma seperated string of the values' ToString output</returns>
    public override string ToString()
    {
      string csvQueue = string.Empty;
      T[] qArray = new T[_queue.Count];
      _queue.CopyTo(qArray, 0);

      for(int i = 0; i < qArray.Length; i++)
      {
        //If there is nothing in the csvQueue then we don't want to add an unnecessary comma
        if (csvQueue == string.Empty)
        {
          csvQueue = qArray[i].ToString();
        }
        else
        {
          csvQueue = string.Format("{0}, {1}", csvQueue, qArray[i].ToString());
        }
      }

      return csvQueue;
    }

    /// <summary>
    /// Clears and nulls out the properties of this class
    /// </summary>
    public void Dispose()
    {
      lesser = null;
      greater = null;
      _queue.Clear();
      _queue = null;
    }
  }
}
