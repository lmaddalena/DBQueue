using Azure;
using DBQueue.Model;
using DBQueue.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Formats.Asn1;

namespace DBQueue;

internal class Consumer : IConsumer
{
    private readonly ILogger<Consumer> _logger;
    private readonly bool _keepJournal = false;
    private readonly IConfiguration _configuration;

    public Consumer(ILogger<Consumer> logger, IConfiguration configuration)
    {
        _logger = logger;    
        _configuration = configuration;

        // get KeepJournal property from appsettings
        var b = configuration.GetValue(typeof(bool), "DBQueue:KeepJournal");
        _keepJournal = b != null ? (bool)b : false;

    }
    public async Task DequeueAsync(Guid messageId, bool keepJournal)
    {
        try
        {
            _logger.LogDebug("ENTER: DequeueAsync. MessageId: {0}, keepJournal: {1}", messageId, keepJournal);

            using (DataContext dc = new DataContext())
            {


                MessageHeader? header = dc.MessageHeaders.Where(h => h.MessageId == messageId).SingleOrDefault();
                MessageBody? body = dc.MessageBodies.Where(b => b.MessageId == messageId).SingleOrDefault();


                if (header != null)
                {
                    header.DequeuedDT = DateTime.Now;
                    header.IsProcessed = true;
                    //header.LockDT = null;
                    //header.IsLocked = false;

                    if (body != null)
                    {
                        if (keepJournal)
                        {
                            MessageJournal j = new MessageJournal(body.MessageId, body.Text);
                            await dc.MessageJournals.AddAsync(j);
                        }

                        dc.MessageBodies.Remove(body);

                    }

                    await dc.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Message with id {messageId} not found.");
                }

            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task DequeueAsync(Guid messageId)
    {
        await DequeueAsync(messageId, _keepJournal);
    }

    public async Task<Message?> GetMessageFromQueueAsync()
    {
        return await GetMessageFromQueueAsync(null);
    }

    public async Task<Message?> GetMessageFromQueueAsync(string? tag)
    {
        int max_retry = 5;
        int current_try = 1;

        try
        {
            using (DataContext dc = new DataContext())
            {
                while (true)
                {
                    if (current_try > max_retry)
                    {
                        _logger.LogDebug("Exceeded the maximum number of tries getting message from the Queue due to concurrency exception.");
                        return null;
                    }


                    MessageHeader? header = null;
                    MessageBody? body = null;

                    try
                    {


                        IQueryable<MessageHeader> qh = (
                                                        from h in dc.MessageHeaders
                                                        where
                                                            (h.Tag == tag || tag == null) &&
                                                            h.IsLocked == false &&
                                                            h.IsProcessed == false
                                                        orderby h.EnqueuedDT
                                                        select h
                                                       ).Take(1);





                        header = await qh.SingleOrDefaultAsync();


                        // lock the message
                        if (header != null)
                        {
                            header.LockDT = DateTime.Now;
                            header.IsLocked = true;
                            await dc.SaveChangesAsync();


                            // get the message body
                            IQueryable<MessageBody> qb = (
                                                          from b in dc.MessageBodies
                                                          where b.MessageId == header.MessageId
                                                          select b);

                            body = await qb.SingleOrDefaultAsync();

                            return new Message(header, body);

                        }

                        return null;



                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogDebug("ConcurrenceException: The message was read by another process");
                        if (header != null)
                            dc.Entry<MessageHeader>(header).Reload();
                        current_try++;
                        await Task.Delay(500);
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        throw;
                    }
                }
            }



        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

    }
}