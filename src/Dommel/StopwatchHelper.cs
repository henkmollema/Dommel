using System;
using System.Diagnostics;

namespace Dommel
{
    public class StopwatchHelper
    {
        public static IDisposable Start(string reference = "")
        {
            Stopwatch sw = Stopwatch.StartNew();
            return new DisposableAction(() =>
                                        {
                                            sw.Stop();
                                            Console.WriteLine("Stopwatch stopped. Reference: '{0}'. Elapsed: {1}ms", reference, sw.Elapsed.TotalMilliseconds);
                                        });
        }
    }

    public class DisposableAction : IDisposable
    {
        private readonly Action _callback;

        public DisposableAction(Action callback)
        {
            _callback = callback;
        }

        public void Dispose()
        {
            _callback();
        }
    }
}
