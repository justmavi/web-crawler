namespace Services.Abstractions
{
    public interface IUrlQueue
    {
        bool Push(string url, CancellationToken token);
        bool Pop(out string item, CancellationToken token);
    }
}
