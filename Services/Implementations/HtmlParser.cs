using Services.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Services.Implementations;

public partial class HtmlParser(ILogger<HtmlParser> logger) : IHtmlParser
{
    public async Task<ICollection<string>> GetUrlsAsync(string startUrl, CancellationToken token)
    {
        try
        {
            using var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(startUrl, token);

            // Use regex to find all URLs in the HTML
            var urlRegex = UrlParserRegex();
            var matches = urlRegex.Matches(html);

            var urls = new List<string>();
            foreach (var match in matches.Cast<Match>())
            {
                var url = match.Groups[2].Value;
                if (Uri.TryCreate(url, UriKind.Absolute, out _)) urls.Add(url);
            }

            return urls;
        }
        catch (Exception ex)
        {
            logger.LogCritical("Error fetching or parsing HTML: {message}", ex.Message);
            return [];
        }
    }

    [GeneratedRegex(@"(?i)<a\s+(?:[^>]*?\s+)?href=(['""])(.*?)\1", RegexOptions.None, "en-US")]
    private static partial Regex UrlParserRegex();
}