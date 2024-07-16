using System.Collections.Concurrent;
using Services.Abstractions;

namespace Services.Implementations
{
    public class VisitedUrlList : IVisitedUrlList
    {
        private readonly ConcurrentDictionary<string, bool> _dictionary = new();

        public bool Add(string url)
        {
            return _dictionary.TryAdd(url, true);
        }
    }
}
