namespace D424_TL.Database
{
  public class Grade
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int CourseworkId { get; set; }
    public int Score { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public Grade() { }
  }
}
