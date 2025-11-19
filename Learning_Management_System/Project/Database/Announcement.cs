namespace D424_TL.Database
{
  public class Announcement
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public string? Message { get; set; }
    public string? Visibility { get; set; } // All, Teachers, None
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }

    public Announcement() { }
  }
}
