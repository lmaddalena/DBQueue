using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBQueue.Model;

public class MessageJournal : IncomingMessageBody
{

    public MessageJournal() : base()
    {
        
    }
    
    public MessageJournal(Guid messageId, string text) : base(messageId, text)
    {

    }

}