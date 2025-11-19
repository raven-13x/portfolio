namespace D424_TL.Components.Pages.Student_Teacher
{
  public partial class AttendancePage
  {
    ObservableCollection<Attendance>? AttendanceEntries { get; set; } = new ObservableCollection<Attendance>();
    List<Attendance> attendances = new List<Attendance>();
    List<ScheduleEntry> teacherSchedule = new List<ScheduleEntry>();
    List<Attendance> allAttendance = new List<Attendance>();

    Attendance? selectedAttendance = new Attendance();
    DateTime? SelectedDate
    {
      get => selectedDate;
      set
      {
        if (selectedDate != value)
        {
          selectedDate = (DateTime)value;
          if (Global.CurrUser.Role == "Student") { GetAttendanceStudent(); } else { GetAttendanceTeacher(); }
        }
      }

    }
    DateTime selectedDate = DateTime.Today;
    Timer? timer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
      if (firstRender)
      {
        if (Global.CurrUser == null || Global.CurrUser.Role == "Admin") 
        {
          Global.MainContentClass = "signInBackground";
          Global.DrawerOpen = false;
          Global.DrawerClass = "invisible";
          Global.AppBarClass = "invisible";
          Global.MainContentWidth = MaxWidth.Small;
          nav.NavigateTo("", true);
        }
        else 
        {
          await RefreshData();
        }
      }
    }
    async Task RefreshData()
    {
      if (Global.CurrUser.Role == "Student")
      {
        await GetAttendanceStudent();
      }
      else if (Global.CurrUser.Role == "Teacher")
      {
        await GetAttendanceTeacher();
      }
      await Global.GetAllUsers(D424DataContextFactory);
      await Global.GetUnreadMessages(D424DataContextFactory);
      StateHasChanged();
    }
    async Task UpdateAttendance(Attendance attendance)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        attendance.LastUpdate = DateTime.Now;
        context.Attendance.Update(attendance);
        await context.SaveChangesAsync();
        Console.WriteLine("Updated");

        selectedAttendance = null;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateAttendance: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void StartedEditingItem(Attendance attendance) => selectedAttendance = attendance;
    void CanceledEditingItem() => selectedAttendance = null;
    async Task GetAttendanceTeacher()
    {
      try
      {
        AttendanceEntries?.Clear();
        await GetAllAttendance();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        int gradingPeriod = (int)Math.Ceiling(selectedDate.Month / 3.0);
        var teacherSchedule = await context.ScheduleEntry.Where(s => s.UserId == Global.CurrUser.Id && s.Year == selectedDate.Year && s.GradingPeriod == gradingPeriod).OrderBy(s => s.Period).ToListAsync();
        
        if (teacherSchedule.Count == 0) { return; }

        foreach (var entry in teacherSchedule)
        {
          var classScheduleEntries = await context.ScheduleEntry.Where(s => s.ClassId == entry.ClassId && s.Year == selectedDate.Year && s.GradingPeriod == gradingPeriod).ToListAsync();

          foreach (var scheduleEntry in classScheduleEntries)
          {
            var attendance = await context.Attendance.Where(a => a.StudentId == scheduleEntry.UserId && a.ScheduleEntryId == scheduleEntry.Id && a.Date == selectedDate).FirstOrDefaultAsync();
            
            if (attendance == null && (await context.User.Where(u => u.Id == scheduleEntry.UserId).FirstAsync()).Role == "Student")
            {
              var newAttendance = new Attendance
              {
                Id = allAttendance.Count == 0 ? 1 : await context.Attendance.MaxAsync(a => a.Id) + 1,
                StudentId = scheduleEntry.UserId,
                ScheduleEntryId = scheduleEntry.Id,
                Status = "Present",
                Date = selectedDate,
                Entered = DateTime.Now,
                LastUpdate = DateTime.Now
              };
              context.Attendance.Add(newAttendance);
              await context.SaveChangesAsync();
              allAttendance.Add(newAttendance);
              AttendanceEntries.Add(newAttendance);
            }
            else if (attendance != null)
            {
              AttendanceEntries.Add(attendance);
            }
          }
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetAttendanceTeacher: {ex.Message}");
      }
    }
    async Task GetAttendanceStudent()
    {
      try
      {
        
        AttendanceEntries.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var attendance = await context.Attendance.Where(a => a.StudentId == Global.CurrUser.Id && a.Date == selectedDate).ToListAsync();

        if (attendance.Count == 0) { return; }
        foreach (var attendanceEntry in attendance) { AttendanceEntries.Add(attendanceEntry); }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetAttendanceStudent: {ex.Message}");
      }
    }
    async Task GetAllAttendance()
    {
      try
      {
        allAttendance.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        allAttendance = await context.Attendance.ToListAsync();
      }
      catch(Exception ex) 
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetAllAttendance: {ex.Message}");
      }
    }
    async Task<int> GetPeriodNbr(int scheduleEntryId)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        int periodNbr = (await context.ScheduleEntry.Where(s => s.Id == scheduleEntryId).FirstAsync()).Period;
        return periodNbr;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetPeriodNbr: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
        return 0;
      }
    }
    async Task<string> GetClassTitle(int scheduleEntryId)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var scheduleEntry = await context.ScheduleEntry.Where(s => s.Id == scheduleEntryId).FirstAsync();
        string classTitle = (await context.Class.Where(c => c.Id == scheduleEntry.ClassId).FirstAsync()).Title;
        return classTitle;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetClassTitle: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
        return "Error";
      }
    }
  }
}
