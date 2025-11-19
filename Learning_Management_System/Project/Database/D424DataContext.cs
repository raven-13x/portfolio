namespace D424_TL.Database
{
  public class D424DataContext : DbContext
  {
    public DbSet<Announcement> Announcement { get; set; }
    public DbSet<Attendance> Attendance { get; set; }
    public DbSet<Class> Class { get; set; }
    public DbSet<Coursework> Coursework { get; set; }
    public DbSet<Grade> Grade { get; set; }
    public DbSet<Log> Log { get; set; }
    public DbSet<Message> Message { get; set; }
    public DbSet<ScheduleEntry> ScheduleEntry { get; set; }
    public DbSet<User> User { get; set; }
    
    protected readonly IConfiguration Configuration;

    public D424DataContext(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(Configuration.GetConnectionString("D424_DB"));
    }
  }
}
