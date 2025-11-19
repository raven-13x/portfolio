namespace D424_TL.Database
{
  public class Log
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public string? Level { get; set; } // INFO, WARNING, ERROR
    public string? Message { get; set; }
    public DateTime Entered { get; set; }

    public Log() { }
  }
}
