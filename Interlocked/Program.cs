using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadCounterPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            var counters = new ICounter[]
            {
                new OneThreadCounter(),
                new MultiThreadCounterWithoutLock(),
                new MultiThreadCounterWithLock(),
                new InterlockedOneThreadCounter(),
                new InterlockedMultiThreadCounter()
            };

            var value = 100000000;

            foreach (var counter in counters)
            {
                IncrementAndLogTime(counter, value);
            }

            Console.ReadLine();
        }

        static void IncrementAndLogTime(ICounter counter, int value)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = counter.IncrementCounterTo(value).Get();

            stopwatch.Stop();

            var message = new StringBuilder();
            message.AppendLine($"\t {counter.GetType().Name}");
            message.AppendLine($"\t ===> Execution time: {stopwatch.ElapsedMilliseconds} ms. ");
            message.AppendLine($"\t Result: {result}");
            message.AppendLine();
            Console.WriteLine(message.ToString());
        }
    }

    interface ICounter
    {
        int Get();
        ICounter IncrementCounterTo(int value);
    }

    class OneThreadCounter : ICounter
    {
        private static int _counter;

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
        private static int _counter;

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

    class MultiThreadCounterWithLock : ICounter
    {
        private static int _counter;
        private static readonly object _lock = new object();

        public int Get() => _counter;

        public ICounter IncrementCounterTo(int value)
        {
            Parallel.For(0, Environment.ProcessorCount, _ =>
            {
                for (int i = 0; i < value; i++)
                {
                    lock (_lock)
                    {
                        _counter++;
                    }
                }
            });
            return this;
        }
    }

    class InterlockedOneThreadCounter : ICounter
    {
        private static int _counter;

        public int Get() => _counter;

        public ICounter IncrementCounterTo(int value)
        {
            for (int i = 0; i < value; i++)
            {
                Interlocked.Increment(ref _counter);
            }
            return this;
        }
    }

    class InterlockedMultiThreadCounter : ICounter
    {
        private static int _counter;

        public int Get() => _counter;

        public ICounter IncrementCounterTo(int value)
        {
            Parallel.For(0, Environment.ProcessorCount, _ =>
            {
                for (int i = 0; i < value; i++)
                {
                    Interlocked.Increment(ref _counter);
                }
            });
            return this;
        }
    }
}
