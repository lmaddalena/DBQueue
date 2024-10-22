using DBQueue.Model;

namespace DBQueue;

public interface IPublisher
{
    public Task<Message> EnqueueAsync(string tag, string text);
    public Task<Message> EnqueueAsync(string text);
}