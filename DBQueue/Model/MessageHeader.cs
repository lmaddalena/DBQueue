using System.ComponentModel.DataAnnotations;

namespace DBQueue.Model;

public class MessageHeader
{
    #region member variables

    private Guid _messageId;
    private string _tag = "n/a";
    private DateTime? _enqueuedDT;
    private DateTime? _dequeuedDT;
    private DateTime? _lockDT;
    private bool _isProcessed;
    private bool _isLocked;
    private Guid _Version;

    #endregion

    public MessageHeader(Guid messageId, string tag)
    {
        this.MessageId = messageId;
        this.Tag = tag;
    }

    public MessageHeader(string tag)
    {
        this.MessageId = Guid.NewGuid();
        this.Tag = tag;
    }

    public MessageHeader(Guid messageId)
    {
        this.MessageId = messageId;
        this.Tag = "n/a";
    }

    public MessageHeader()
    {
        this.MessageId = Guid.NewGuid();
        this.Tag = "n/a";
    }

    
    [Required]
    [Key]
    public Guid MessageId
    {
        get { return _messageId; }
        private set {                         
            _messageId = value;
            _Version = Guid.NewGuid (); 
        }
    }

    [Required]
    [MaxLength(255)]
    public string Tag
    {
        get { return _tag; }
        set 
        { 
            _tag = value; 
            _Version = Guid.NewGuid ();
        }
    }
        
    [Required]
    public DateTime? EnqueuedDT
    {
        get { return _enqueuedDT; }
        set 
        { 
            _enqueuedDT = value; 
            _Version = Guid.NewGuid ();
        }
    }

    public DateTime? DequeuedDT
    {
        get { return _dequeuedDT; }
        set 
        { 
            _dequeuedDT = value; 
            _Version = Guid.NewGuid ();
        }
    }
        
    public DateTime? LockDT
    {
        get { return _lockDT; }
        set 
        { 
            _lockDT = value; 
            _Version = Guid.NewGuid ();                        
        }
    }
        
    [Required]
    public bool IsProcessed
    {
        get { return _isProcessed; }
        set 
        { 
            _isProcessed = value; 
            _Version = Guid.NewGuid ();                        
        }
    }

    [Required]
    public bool IsLocked
    {
        get { return _isLocked; }
        set 
        { 
            _isLocked = value; 
            _Version = Guid.NewGuid ();
        }
    }

    [ConcurrencyCheck]
    public Guid Version
    {
        get { return _Version; }
        set { _Version = value; }
    }


    //public void Enqueue()
    //{
    //        this.EnqueuedDT = DateTime.Now;
    //}

    //public void Dequeue(bool keepJournal)
    //{
    //        this.DequeuedDT = DateTime.Now;
    //        this.IsProcessed = true;

    //        if(keepJournal && this.Body != null)
    //                this.Journal = new MessageJournal(this.Body.MessageId, this.Body.Text, this);

    //        this.Body = null!;

    //        Unlock();
    //}

}