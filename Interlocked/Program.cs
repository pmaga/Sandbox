using System;
using System.Diagnostics;

namespace Interlocked
{
    class Program
    {
        static void Main(string[] args)
        {
            var counters = new ICounter[]
            {
                new OneThreadCounter()
            };

            var value = 100000000;

            foreach (var counter in counters)
            {
                LogTime(() => counter.IncrementCounterTo(value));
            }


            Console.ReadLine();
        }

        static void LogTime(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();

            Console.WriteLine($"\t ===> Execution time: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }

    interface ICounter
    {
        int Get();
        void IncrementCounterTo(int value);
    }

    class OneThreadCounter : ICounter
    {
        private int _counter;

        public int Get() => _counter;

        public void IncrementCounterTo(int value)
        {
            for (int i = 0; i < value; i++)
            {
                _counter++;
            }
        }
    }
}
