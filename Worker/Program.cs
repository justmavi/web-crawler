using Services.Abstractions;
using Services.Implementations;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddSingleton<IHtmlParser, HtmlParser>();
            builder.Services.AddSingleton<IVisitedUrlList, VisitedUrlList>();
            builder.Services.AddSingleton<IUrlQueue, UrlQueue>();
            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}