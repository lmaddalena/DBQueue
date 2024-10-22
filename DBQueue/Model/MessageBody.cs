using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBQueue.Model;

public class MessageBody : IncomingMessageBody
{

    public MessageBody() : base()
    {
        
    }
           
    public MessageBody(Guid messageId, string text) : base(messageId, text)
    {

    }

 }