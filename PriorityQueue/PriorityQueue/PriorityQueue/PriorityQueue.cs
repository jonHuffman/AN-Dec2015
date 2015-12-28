using System;

namespace PriorityQueue
{
  /// <summary>
  /// A queue that handles adding elements in a priority order.
  /// Items inserted at the same priority level will be handled First In First Out
  /// </summary>
  /// <typeparam name="T">The type of the value being stored in the queue</typeparam>
  /// <remarks>The PriorityQueue works in conjunction with <see cref="PQNode"/> to create a searchable Binary Tree for quick addition and removal of values.</remarks>
  class PriorityQueue<T>
  {
    private PQNode<T> _head;

    #region Properties

    /// <summary>
    /// Gets the number of values actually queued in the PriorityQueue<T>
    /// </summary>
    public int Count
    {
      get
      {
        if (_head != null)
        {
          return _head.Count;
        }

        return 0;
      }
    }
    #endregion

    public PriorityQueue() { }

    /// <summary>
    /// Adds a value to the priority queue at the appropriate location
    /// </summary>
    /// <param name="value">Value to be added to the Queue</param>
    /// <param name="priority">The priority that the value should take in the Queue</param>
    public void Enqueue(T value, int priority)
    {
      PQNode<T> node;

      if (_head != null)
      {
        node = _head.GetNode(priority);
      }
      else
      {
        node = _head = new PQNode<T>(priority);
      }

      node.Enqueue(value);
    }

    /// <summary>
    /// Retrieves the value at the front of the Queue. Will also dispose the PQNode that contained the value if it was the last.
    /// </summary>
    /// <returns>The value at the front of the Priority Queue</returns>
    public T DequeueMin()
    {
      if (_head == null)
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }

      PQNode<T> node = _head;
      PQNode<T> prevNode = null;

      while (node.lesser != null)
      {
        prevNode = node;
        node = node.lesser;
      }

      T value = node.DequeueMin();

      //If the node no longer has values of its own we remove it from the tree and re-position its children if needed
      //Remember that this node is the left-most node on the tree and therfor does not have any lower priority children
      if (node.QueueCount == 0)
      {
        if (prevNode != null)
        {
          if (node.greater != null)
          {
            prevNode.lesser = node.greater;
          }
          else
          {
            prevNode.lesser = null;
          }
        }
        else
        {
          if (node.greater != null)
          {
            _head = node.greater;
          }
          else
          {
            _head = null;
          }
        }

        node.Dispose();
        node = null;
      }

      return value;
    }

    /// <summary>
    /// Retrieves the value at the back of the Queue. Will also dispose the PQNode that contained the value if it was the last.
    /// </summary>
    /// <returns>The value at the back of the Priority Queue</returns>
    public T DequeueMax()
    {
      if (_head == null)
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }

      PQNode<T> node = _head;
      PQNode<T> prevNode = null;

      while (node.greater != null)
      {
        prevNode = node;
        node = node.greater;
      }

      T value = node.DequeueMax();

      //If the node no longer has values of its own we remove it from the tree and re-position its children if needed
      //Remember that this node is the right-most node on the tree and therfor does not have any higher priority children
      if (node.QueueCount == 0)
      {
        if(prevNode != null)
        {
          if (node.lesser != null)
          {
            prevNode.greater = node.lesser;
          }
          else
          {
            prevNode.greater = null;
          }
        }
        else
        {
          if (node.lesser != null)
          {
            _head = node.lesser;
          }
          else
          {
            _head = null;
          }
        }

        node.Dispose();
        node = null;
      }

      return value;
    }

    /// <summary>
    /// Locates and returns the value at the front of the queue without removing it
    /// </summary>
    /// <returns>The value at the front of the Priority Queue</returns>
    public T PeekMin()
    {
      if (_head == null)
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }

      PQNode<T> node = _head;

      while (node.lesser != null)
      {
        node = node.lesser;
      }

      return node.PeekMin();
    }

    /// <summary>
    /// Locates and returns the value at the back of the queue without removing it
    /// </summary>
    /// <returns>The value at the back of the Priority Queue</returns>
    public T PeekMax()
    {
      if (_head == null)
      {
        throw new Exception("No value has been queued. Make sure to check Count before attempting to retrieve values.");
      }

      PQNode<T> node = _head;

      while (node.greater != null)
      {
        node = node.greater;
      }

      return node.PeekMax();
    }

    /// <summary>
    /// Gets a comma-seperated string containing the values in their queued order
    /// </summary>
    /// <returns>A comma-seperated string of the queued values in order</returns>
    public override string ToString()
    {
      string csvQueue = string.Empty;

      if (_head != null)
      {
        CompileQueueString(ref csvQueue, _head);
      }

      return csvQueue;
    }

    /// <summary>
    /// Recursively navigates the Priority Queue and compiles a string of comma-seperated values in the order of the queue
    /// </summary>
    /// <param name="queueString">A reference to a string to dump the data into</param>
    /// <param name="node">The node to navigate</param>
    private void CompileQueueString(ref string queueString, PQNode<T> node)
    {
      if (node.lesser != null)
      {
        CompileQueueString(ref queueString, node.lesser);
      }

      //If there is nothing in the csvQueue then we don't want to add an unnecessary comma
      if (queueString == string.Empty)
      {
        queueString = node.ToString();
      }
      else
      {
        queueString = string.Format("{0}, {1}", queueString, node.ToString());
      }

      if (node.greater != null)
      {
        CompileQueueString(ref queueString, node.greater);
      }
    }
  }
}
