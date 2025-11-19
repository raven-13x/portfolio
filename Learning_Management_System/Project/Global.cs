using D424_TL.Database;
using Microsoft.AspNetCore.Components.Web;

namespace D424_TL
{
  public static class Global
  {
    public static User? CurrUser { get; set; }

    // CSS
    public static string AppBarClass { get; set; } = "invisible";
    public static string DrawerClass { get; set; } = "invisible";
    public static bool DrawerOpen { get; set; } = false;
    public static string ContainerMaxWidth { get; set; } = "MaxWidth.Medium";
    public static string MainContentClass { get; set; } = "signInBackground";
    public static MaxWidth MainContentWidth { get; set; } = MaxWidth.Small;

    // Admin
    public static ObservableCollection<User>? AllUsers { get; set; } = new ObservableCollection<User>();
    public static ObservableCollection<Class>? AllClasses { get; set; } = new ObservableCollection<Class>();
    public static ObservableCollection<Announcement>? AllAnnouncements { get; set; } = new ObservableCollection<Announcement>();
    public static ObservableCollection<ScheduleEntry>? AllScheduleEntries { get; set; } = new ObservableCollection<ScheduleEntry>();
    public static ObservableCollection<Message> FlaggedMessages { get; set; } = new ObservableCollection<Message>();
    public static List<Message> AllMessages { get; set; } = new List<Message>();
    public static List<User> AllTeachers { get; set; } = new List<User>();
    public static List<Message> UnreadMessages { get; set; } = new List<Message>();

    // Student/Teacher
    public static List<User> Recipients { get; set; } = new List<User>();
    public static List<User> AvailableRecipients { get; set; } = new List<User>();
    public static int SelectedClassId_Coursework { get; set; }
    public static int SelectedClassGradingPeriod { get; set; }
    public static int SelectedClassYear { get; set; }
    public static int SelectedCourseworkId_Grades { get; set; }

    // Data methods
    public static async Task GetAllUsers(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        AllUsers?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var users = await context.User.ToListAsync();
        foreach (var user in users) { AllUsers.Add(user); }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetAllUsers: {ex.Message}");
      }
    }
    public static async Task GetAllTeachers(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        AllTeachers?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        AllTeachers = await context.User.Where(u => u.Role == "Teacher").ToListAsync();
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetAllTeachers: {ex.Message}");
      }
    }
    public static async Task GetAllClasses(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        AllClasses?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var classes = await context.Class.ToListAsync();
        foreach (var _class in classes) { AllClasses.Add(_class); }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetAllClasses: {ex.Message}");
      }
    }
    public static async Task GetAllAnnouncements(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        AllAnnouncements?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var announcements = await context.Announcement.OrderByDescending(a => a.Created).ToListAsync();
        foreach (var announcement in announcements) { AllAnnouncements.Add(announcement); }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetAllAnnouncements: {ex.Message}");
      }
    }
    public static async Task GetAllScheduleEntries(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        AllScheduleEntries?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var scheduleEntries = await context.ScheduleEntry.OrderBy(s => s.Id).ToListAsync();
        foreach (var scheduleEntry in scheduleEntries) { AllScheduleEntries.Add(scheduleEntry); }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetAllScheduleEntries: {ex.Message}");
      }
    }
    public static async Task GetFlaggedMessages(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        FlaggedMessages?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var flaggedMessages = await context.Message.Where(m => m.Flagged == "Yes").OrderByDescending(m => m.Sent).ToListAsync();
        foreach (var flaggedMessage in flaggedMessages) { FlaggedMessages.Add(flaggedMessage); }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetFlaggedMessages: {ex.Message}");
      }
    }
    public static async Task GetRecipientsTeacher(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        Recipients.Clear();
        AvailableRecipients.Clear();
        int gradingPeriod = (int)Math.Ceiling(DateTime.Now.Month / 3.0);
        List<ScheduleEntry> studentScheduleEntries = new List<ScheduleEntry>();

        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var classes = await context.Class.Where(c => c.TeacherId == CurrUser.Id && c.Active == "Yes").ToListAsync();

        foreach (var _class in classes)
        {
          var scheduleEntries = await context.ScheduleEntry.Where(s => s.ClassId == _class.Id && s.Year == DateTime.Now.Year && s.GradingPeriod == gradingPeriod).ToListAsync();
          studentScheduleEntries.AddRange(scheduleEntries);
        }

        foreach (var entry in studentScheduleEntries)
        {
          User student = await context.User.Where(u => u.Role == "Student" && u.Id == entry.UserId).FirstOrDefaultAsync();
          if (student == null || Recipients.Contains(student)) { continue; }
          Recipients.Add(student);

          bool chatExists = (await context.Message.Where(s => (s.SenderId == CurrUser.Id && s.RecipientId == student.Id) || (s.SenderId == student.Id && s.RecipientId == CurrUser.Id)).FirstOrDefaultAsync() == null) ? false : true;
          if (chatExists) { continue; }

          AvailableRecipients.Add(student);
        }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetRecipientsTeacher: {ex.Message}");
      }
    }
    public static async Task GetRecipientsStudent(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      try
      {
        Recipients.Clear();
        int gradingPeriod = (int)Math.Ceiling(DateTime.Now.Month / 3.0);
        List<Class> enrolledClasses = new List<Class>();

        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var students = await context.User.Where(u => u.Role == "Teacher").ToListAsync();
        var schedule = await context.ScheduleEntry.Where(s => s.UserId == CurrUser.Id && s.GradingPeriod == gradingPeriod && s.Year == DateTime.Today.Year).ToListAsync();

        foreach (var entry in schedule)
        {
          var _class = await context.Class.Where(c => c.Id == entry.ClassId).FirstOrDefaultAsync();
          enrolledClasses.Add(_class);
        }

        foreach (var _class in enrolledClasses)
        {
          var teacher = await context.User.Where(u => u.Id == _class.TeacherId).FirstOrDefaultAsync();
          if (teacher == null) { continue; }
          Recipients.Add(teacher);

          bool chatExists = (await context.Message.Where(s => (s.SenderId == CurrUser.Id && s.RecipientId == teacher.Id) || (s.SenderId == teacher.Id && s.RecipientId == CurrUser.Id)).FirstOrDefaultAsync() == null) ? false : true;
          if (chatExists) { continue; }

          AvailableRecipients.Add(teacher);
        }
      }
      catch (Exception ex)
      {
        await Log(D424DataContextFactory, "ERROR", $"GetRecipientsStudent: {ex.Message}");
      }
    }
    public static async Task GetUnreadMessages(IDbContextFactory<D424DataContext> D424DataContextFactory)
    {
      UnreadMessages.Clear();
      D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
      UnreadMessages.AddRange(await context.Message.Where(m => m.RecipientId == CurrUser.Id && m.Read == "No").ToListAsync());
    }

    // Logging - Polymorphism
    public static async Task Log(IDbContextFactory<D424DataContext> contextFactory, string level, string message)
    {
      try
      {
        D424DataContext _context = await contextFactory.CreateDbContextAsync();
        
        var log = new Log
        {
          UserId = CurrUser.Id,
          Level = level,
          Message = message,
          Entered = DateTime.Now
        };

        _context.Log.Add(log);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Log error: {ex.Message}");
      }
    }
    public static async Task Log(IDbContextFactory<D424DataContext> contextFactory, string level, string message, User user)
    {
      try
      {
        D424DataContext _context = await contextFactory.CreateDbContextAsync();

        var log = new Log
        {
          UserId = user.Id,
          Level = level,
          Message = message,
          Entered = DateTime.Now
        };

        _context.Log.Add(log);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Log error: {ex.Message}");
      }
    }
  }
}
