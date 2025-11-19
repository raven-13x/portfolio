namespace D424_TL.Database
{
  public class User
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; } // Student, Teacher, Admin
    public int GradeLevel { get; set; } // 0 for Admin
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int FailedAttempts { get; set; }
    public string? Locked { get; set; } // Yes, No
    public DateTime Created { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public User() { }
  }
}
