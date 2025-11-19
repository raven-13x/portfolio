namespace D424_TL.Database
{
  public class Message
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string? Content { get; set; }
    public string? Flagged { get; set; } // Yes, No
    public string? Read { get; set; } // Yes, No
    public DateTime Sent { get; set; }
    
    public Message() { }
  }
}
