using System;
using System.Diagnostics;

namespace Dommel
{
    public static class Profiler
    {
        public static IDisposable Start(string reference = "")
        {
            var sw = Stopwatch.StartNew();
            return new DisposableAction(() =>
                                        {
                                            sw.Stop();
                                            Console.WriteLine("Stopwatch stopped. Reference: '{0}'. Elapsed: {1}ms", reference, sw.Elapsed.TotalMilliseconds);
                                        });
        }
    }

    internal class DisposableAction : IDisposable
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
