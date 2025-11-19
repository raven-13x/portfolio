namespace D424_TL.Database
{
  public class Coursework
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ClassId { get; set; }
    public string? Type { get; set; } // Homework, Quiz, Test
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxScore { get; set; }
    public int Year { get; set; }
    public int GradingPeriod { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }

    public Coursework() { }
  }
}
