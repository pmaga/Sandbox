using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Interlocked
{
    class Program
    {
        static void Main(string[] args)
        {
            var counters = new ICounter[]
            {
                new OneThreadCounter(),
                new MultiThreadCounterWithoutLock()
            };

            var value = 100000000;

            foreach (var counter in counters)
            {
                LogTime(() => counter.IncrementCounterTo(value).Get());
            }


            Console.ReadLine();
        }

        static void LogTime(Func<int> action)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = action();

            stopwatch.Stop();

            Console.WriteLine($"\t ===> Execution time: {stopwatch.ElapsedMilliseconds} ms. \t Result: {result}");
        }
    }

    interface ICounter
    {
        int Get();
        ICounter IncrementCounterTo(int value);
    }

    class OneThreadCounter : ICounter
    {
        private int _counter;

        public int Get() => _counter;

        public ICounter IncrementCounterTo(int value)
        {
            for (int i = 0; i < value; i++)
            {
                _counter++;
            }
            return this;
        }
    }

    class MultiThreadCounterWithoutLock : ICounter
    {
        private int _counter;

        public int Get() => _counter;

        public ICounter IncrementCounterTo(int value)
        {
            Parallel.For(0, Environment.ProcessorCount, _ =>
            {
                for (int i = 0; i < value; i++)
                {
                    _counter++;
                }
            });
            return this;
        }
    }
}
