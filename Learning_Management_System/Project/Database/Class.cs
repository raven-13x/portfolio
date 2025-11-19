namespace D424_TL.Database
{
  public class Class
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TeacherId { get; set; } // FK in database, but cascading is not necessary.
    public string? Title { get; set; }
    public string? Subject { get; set; }
    public int GradeLevel { get; set; }
    public string? Active { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }

    public Class() { }
  }
}
