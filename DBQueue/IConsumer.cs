using DBQueue.Model;

namespace DBQueue;

public interface IConsumer
{
    Task DequeueAsync(Guid messageId, bool keepJournal);
    Task DequeueAsync(Guid messageId);
    Task<Message?> GetMessageFromQueueAsync();
    Task<Message?> GetMessageFromQueueAsync(string? tag);
}