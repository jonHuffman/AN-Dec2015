using System;

namespace PriorityQueue
{
  class Program
  {
    static void Main(string[] args)
    {
      PriorityQueue<string> testQueue = new PriorityQueue<string>();

      testQueue.Enqueue("dog", 5);
      testQueue.Enqueue("cat", 1);
      testQueue.Enqueue("pig", 3);
      testQueue.Enqueue("cow", 7);
      testQueue.Enqueue("moose", 1);
      testQueue.Enqueue("giraffe", 12);
      testQueue.Enqueue("monkey", 3);
      testQueue.Enqueue("tiger", 4);
      testQueue.Enqueue("dolphin", 6);

      #region Initial Queue Order
      Console.WriteLine("The Queue should print in the following order:");
      Console.WriteLine("cat, moose, pig, monkey, tiger, dog, dolphin, cow, giraffe");
      Console.WriteLine("The Queue in order is:");
      Console.WriteLine(testQueue.ToString());
      Console.WriteLine();
      #endregion

      #region Remove two elements from the front of the queue
      testQueue.DequeueMin();
      testQueue.DequeueMin();

      Console.WriteLine("The front value of the Queue should be:");
      Console.WriteLine("pig");
      Console.WriteLine("The front value of the Queue is:");
      Console.WriteLine(testQueue.PeekMin());
      Console.WriteLine();
      #endregion

      #region Remove one element from the front and three from the back
      testQueue.DequeueMax();
      testQueue.DequeueMax();
      testQueue.DequeueMin();
      testQueue.DequeueMax();

      Console.WriteLine("The Queue should print in the following order:");
      Console.WriteLine("monkey, tiger, dog");
      Console.WriteLine("The Queue in order is:");
      Console.WriteLine(testQueue.ToString());
      Console.WriteLine();
      #endregion

      #region Remove two elements from the back of the Queue
      testQueue.DequeueMax();
      testQueue.DequeueMax();

      Console.WriteLine("The back value of the Queue should be:");
      Console.WriteLine("monkey");
      Console.WriteLine("The back value of the Queue is:");
      Console.WriteLine(testQueue.PeekMax());
      Console.WriteLine();
      Console.WriteLine("The Queue should print in the following order:");
      Console.WriteLine("monkey");
      Console.WriteLine("The Queue in order is:");
      Console.WriteLine(testQueue.ToString());
      Console.WriteLine();
      #endregion

      #region Remove the last element
      testQueue.DequeueMin();

      if (testQueue.Count > 0)
      {
        testQueue.DequeueMin();
      }
      Console.WriteLine("The Queue should print in the following order:");
      Console.WriteLine("");
      Console.WriteLine("The Queue in order is:");
      Console.WriteLine(testQueue.ToString());
      #endregion

      PressKeyContinue();
    }

    private static void PressKeyContinue()
    {
      //Just a stop to prevent the console application from automatically closing
      Console.WriteLine();
      Console.WriteLine("Press any key to continue...");
      Console.Read();
    }
  }
}
