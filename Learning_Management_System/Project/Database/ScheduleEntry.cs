namespace D424_TL.Database
{
  public class ScheduleEntry
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public int ClassId { get; set; }
    public int Period { get; set; }
    public int GradingPeriod { get; set; } // 1, 2, 3, or 4
    public int Year { get; set; } // Len = 4
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public ScheduleEntry() { }
  }
}