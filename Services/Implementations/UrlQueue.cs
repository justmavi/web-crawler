using System.Collections.Concurrent;
using System.Diagnostics;
using Services.Abstractions;

namespace Services.Implementations
{
    public class UrlQueue : IUrlQueue
    {
        private readonly BlockingCollection<string> _queue = new();

        public bool Push(string url, CancellationToken token)
        {
            _queue.Add(url, token);
            return true;
        }

        public bool Pop(out string item, CancellationToken token)
        {
            return _queue.TryTake(out item!, Timeout.Infinite, token);
        }
    }
}
