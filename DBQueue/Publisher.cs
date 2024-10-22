using DBQueue.Model;
using DBQueue.Repository;
using Microsoft.Extensions.Logging;

namespace DBQueue;

internal class Publisher : IPublisher
{
    private readonly ILogger<Publisher> _logger;


    public Publisher(ILogger<Publisher> logger)
    {
        _logger = logger;
    }

    public async Task<Message> EnqueueAsync(string tag, string text)
    {
        try
        {
            _logger.LogDebug("ENTER: EnqueueAsync");

            MessageHeader h = new MessageHeader(tag);
            h.EnqueuedDT = DateTime.Now;

            MessageBody b = new MessageBody(h.MessageId, text);

            using (DataContext dc = new DataContext())
            {
                await dc.MessageHeaders.AddAsync(h);
                await dc.MessageBodies.AddAsync(b);

                await dc.SaveChangesAsync();

            }


            _logger.LogInformation($"Message added to the queue with id {h.MessageId}.");
                       
            return new Message(h, b);            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

    }

    public async Task<Message> EnqueueAsync(string text)
    {
        try
        {
            return await EnqueueAsync("n/a", text);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

    }



}
