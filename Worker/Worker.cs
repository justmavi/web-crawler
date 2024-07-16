using System.Diagnostics;
using Services.Abstractions;

namespace Worker
{
    public class Worker(ILogger<Worker> logger, IHtmlParser parser, IUrlQueue queue, IVisitedUrlList visitedList) : BackgroundService
    {

        private static readonly Stopwatch _stopWatch = Stopwatch.StartNew();
        private static int _totalCount;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var startUrl = "https://atmk.mil.am";
            queue.Push(startUrl, stoppingToken);

            _stopWatch.Start();

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                logger.LogInformation("Starting a new crawl worker...");

                var id = i;

                Task.Factory.StartNew(() => Crawl(stoppingToken, id), 
                    stoppingToken, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default
                );
            }
        }

        public async Task Crawl(CancellationToken token, int threadId)
        {
            var count = 0;
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation("Crawl worker started");

            while (!token.IsCancellationRequested && queue.Pop(out var url, token))
            {
                if (!visitedList.Add(url)) continue;

                logger.LogInformation("Fetching and parsing {url}...", url);

                var newUrls = await parser.GetUrlsAsync(url, token);
                count += newUrls.Count;

                logger.LogInformation("Fetch done. Parsed {count} urls", newUrls.Count);

                foreach (var newUrl in newUrls)
                {
                    queue.Push(newUrl, token);
                }
            }

            if (stopwatch.IsRunning) stopwatch.Stop();

            Interlocked.Add(ref _totalCount, count);
            logger.LogInformation("In {sec} thread #{i} was parsed {count} URLs. Total: {total}", stopwatch.Elapsed.ToString(@"hh\:mm\:ss"), threadId, count, _totalCount);
        }
    }
}
