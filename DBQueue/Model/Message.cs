using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBQueue.Model
{
    public class Message
    {
        public MessageHeader Header { get; set; }
        public MessageBody? Body { get; set; }

        public Message()
        {
                this.Header = new MessageHeader();
        }

        public Message(MessageHeader header, MessageBody? body)
        {
            this.Header = header;
     
            if(body != null) 
                body.MessageId = header.MessageId;

            this.Body = body;

        }
    }
}
