namespace Services.Abstractions
{
    public interface IHtmlParser
    {
        Task<ICollection<string>> GetUrlsAsync(string startUrl, CancellationToken token);
    }
}
