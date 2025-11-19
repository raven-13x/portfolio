namespace D424_TL.Database
{
  public class Attendance
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int ScheduleEntryId { get; set; }
    public DateTime Date { get; set; }
    public string? Status { get; set; } // Present, Absent, Excused, Tardy
    public DateTime Entered { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public Attendance() { }
  }
}
