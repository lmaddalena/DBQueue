using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBQueue.Model;

public abstract class IncomingMessageBody
{

    [Key]
    [Required]
    public Guid MessageId { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Text { get; set; } = "";

    public IncomingMessageBody()
    {
        
    }
           
    public IncomingMessageBody(Guid messageId, string text)
    {
        this.MessageId = messageId;
        this.Text = text;
    }

 }